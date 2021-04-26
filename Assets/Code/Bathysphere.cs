using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;

public class Bathysphere : MonoBehaviour
{
    public float depth;
    public float speed;
    public float temp;
    public float pressure;
    public float hullStrength; // in bars
    public float hullHealth;
    public float power;
    public float cash;
    public float storage;
    public float direction;
    public float internalTemp;
    public float heatLoss;

    public float maxStorage;
    public float maxPower;

    public float powerDemand;

    public float maxSpeed;

    public float powerConversion;
    public float hydrogenPrice;
    public float idlePowerDemand;
    public float movingPowerDemand;

    public bool skippedEnd;

    public List<Module> modules;

    public JupiterLayer currentLayer;
    public JupiterLayer startingLayer;
    public Coroutine piloting;

    public ShipStatus status;

    public string emergencyMessage;

    Animator animator;
    Camera camera;

    public enum ShipStatus
    {
        DOCKED,
        IDLE,
        UPGRADING,
        DESCENDING,
        ASCENDING,
        DEAD,
        SURFACE,
        CORE
    }

    private void Awake()
    {
        currentLayer = startingLayer;
        depth = startingLayer.depth;
        speed = 1;
        temp = startingLayer.temp;
        pressure = startingLayer.pressure;
        power = 100;
        direction = 0;
        hullHealth = 100;
        hullStrength = 10;
        powerConversion = 6.0f;
        hydrogenPrice = 3.2f;
        internalTemp = 70;
        emergencyMessage = null;
        heatLoss = 0.05f;
        skippedEnd = false;

        status = ShipStatus.DOCKED;

        idlePowerDemand = 0.01f;
        movingPowerDemand = 0.01f;

        maxStorage = 100;
        maxPower = 100;

        animator = GetComponent<Animator>();
        animator.SetFloat("health", hullHealth);

        camera = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        piloting = StartCoroutine(Piloting());
    }

    // Update is called once per frame
    void Update()
    {
        depth += direction * (speed * Time.deltaTime);
    }

    float GetLerpRange(float from, float current, float to)
    {
        if (from == current) return 0.0f;
        if (to == current) return 1.0f;
        return (current - from) / (to - from);
    }

    bool HasPower()
    {
        return (power > float.Epsilon);
    }

    public void SelfDestruct()
    {
        Implode();
    }

    public void StartDescent()
    {
        if (!HasPower()) return;
        status = ShipStatus.DESCENDING;
        depth -= 1;
        direction = -1;
    }

    public void StartAscent()
    {
        status = ShipStatus.ASCENDING;
        direction = 10;
    }

    public void Stop()
    {
        speed = 0;
        direction = 0;
        status = ShipStatus.IDLE;
    }

    public void Recharge()
    {
        if (power == maxPower) return;

        power += storage * powerConversion;
        storage = 0;

        if ( power > maxPower)
        {
            float surplus = power - maxPower;
            power = maxPower;
            storage = surplus / powerConversion;
        }
    }

    public void SellStorage()
    {
        cash += hydrogenPrice * storage;
        storage = 0;
    }

    void OutOfPower()
    {
        Stop();
    }

    void Dock()
    {
        status = ShipStatus.DOCKED;
        speed = startingLayer.peakSpeed;
        depth = startingLayer.depth;
        temp = startingLayer.temp;
        pressure = startingLayer.pressure;

        direction = 0;
        emergencyMessage = "";

        RecalculateStartingBuffs();
    }

    public void RecalculateStartingBuffs()
    {
        maxPower = 100;
        maxStorage = 100;
        hullStrength = 100;
        heatLoss = 0.05f;

        //recalulate power max//
        foreach (Module module in modules)
        {
            maxPower += module.maxPowerBuff;
            maxStorage += module.storageBuff;
            hullStrength += module.hullBuff;
            heatLoss += module.tempBuff;
        }
    }

    void Implode()
    {
        StopCoroutine(piloting);
        speed = 0;
        hullHealth = 0;
        status = ShipStatus.DEAD;
    }

    void StopAtCore()
    {
        status = ShipStatus.CORE;
    }

    public IEnumerator Piloting()
    {
        while (true)
        {
            animator.SetFloat("health", hullHealth);

            CheckForDock();

            if (status != ShipStatus.DOCKED && status != ShipStatus.DEAD)
            {
                //reset powerdemand on each cycle
                powerDemand = idlePowerDemand;
                CheckForTransitions();
                UpdateDepthEffects();
                CaptureHydrogen();
                CheckAirCon();
                CheckHullHealth();
                UpdatePowerUsage();
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    void CheckAirCon()
    {
        if (HasPower())
        {
            float tempDiff = Mathf.Abs(temp - 70);
            powerDemand += tempDiff * 0.001f;
            if (internalTemp == 70) return;

            if (internalTemp > 70)
            {
                internalTemp--;
            }
            else
            {
                internalTemp++;
            }
            return;
        }

        if (temp > internalTemp)
        {
            emergencyMessage = "NO POWER\nTEMP RISING\nDAMAGE AT 120";
            internalTemp += heatLoss;
        }
        else if(temp < internalTemp)
        {
            emergencyMessage = "NO POWER\nTEMP FALLING\nDAMAGE AT -20";
            internalTemp -= heatLoss;
        }
    }

    void UpdateDepthEffects()
    {
        if (status == ShipStatus.IDLE || status == ShipStatus.CORE) return;

        float layerProgress = GetLerpRange(currentLayer.depth, depth, currentLayer.next.depth);
        temp = Mathf.Lerp(currentLayer.temp, currentLayer.next.temp, layerProgress);
        pressure = Mathf.Lerp(currentLayer.pressure, currentLayer.next.pressure, layerProgress);
        speed = Mathf.Lerp(currentLayer.peakSpeed, currentLayer.next.peakSpeed, layerProgress);
        camera.backgroundColor = Color.Lerp(currentLayer.bgColor, currentLayer.next.bgColor, layerProgress);

        //moving uses power//
        powerDemand += movingPowerDemand;
    }
    

    void CheckForTransitions()
    {
        if (!skippedEnd && depth <= 0)
        {
            Stop();
            skippedEnd = true;
            status = ShipStatus.SURFACE;
            return;
        }

        if (depth < currentLayer.next.depth)
        {
            currentLayer = currentLayer.next;

            if (currentLayer.next == null)
            {
                StopAtCore();
            }
        }
        else if (depth > currentLayer.previous.depth )
        {
            currentLayer = currentLayer.previous;
        }
    }

    void CheckForDock()
    {
        if ( depth >= 3000 )
        {
            Dock();
        }
    }

    void UpdatePowerUsage()
    {
        if (status == ShipStatus.ASCENDING || status == ShipStatus.DEAD) return;

        if (power > 0 )
        {
            power -= powerDemand;
        }
        else
        {
            power = 0;
            OutOfPower();
        }
    }

    void CaptureHydrogen()
    {
        if (storage < maxStorage)
        {
            storage += Random.Range(0,currentLayer.hydrogenDensity);
        }
    }

    void CheckHullHealth()
    {
        if (pressure > hullStrength)
        {
            hullHealth -= (pressure - hullStrength) * 0.1f;
        }

        if ( internalTemp > 120 || internalTemp < -20)
        {
            hullHealth -= internalTemp * 0.005f;
        }

        if (hullHealth <= float.Epsilon)
        {
            Implode();
        }
    }

}

