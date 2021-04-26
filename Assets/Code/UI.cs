using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Bathysphere bathysphere;

    [Header("store")]
    public List<Module> moduleStore;
    
    public Text depth;
    public Text layer;
    public Text temp;
    public Text pressure;
    public Text power;
    public Text health;
    public Text h2collection;
    public Text cash;

    public GameObject messagePanel;

    public GameObject dockedPanel;

    public GameObject storePanel;

    public GameObject gameover;
    public GameObject youwon;
    public GameObject dropButton;
    public GameObject reelButton;
    public GameObject refuelButton;
    public GameObject sellButton;
    public GameObject stopButton;
    public GameObject buyBattButton;
    public GameObject modulesPanel;

    public GameObject powerLevel1;
    public GameObject powerLevel2;
    public GameObject powerLevel3;

    public GameObject messagePrefab;
    public GameObject modulePrefab;
    public GameObject modulePurchasePrefab;

    GameObject alertMessage = null;

    private void Start()
    {
        //setup store//
        foreach (Module moduleItem in moduleStore)
        {
            GameObject go = Instantiate(modulePurchasePrefab, storePanel.transform);
            go.GetComponent<ModuleSlot>().module = moduleItem;
            go.GetComponent<ModuleSlot>().store = this;
        }
    }

    public void InstallModule(Module module)
    {
        bathysphere.cash -= module.price;
        bathysphere.modules.Add(module);

        GameObject go = Instantiate(modulePrefab, modulesPanel.transform);
        go.GetComponent<ModuleSlot>().module = module;
    }

    // Update is called once per frame
    void Update()
    {
        depth.text = bathysphere.depth.ToString("n0") + "km";
        layer.text = bathysphere.currentLayer.layerName;
        temp.text = bathysphere.temp.ToString("n0") + "° / " + bathysphere.internalTemp.ToString("n0") + "°" ;
        pressure.text = bathysphere.pressure.ToString("n2") + " bars";
        power.text = bathysphere.power.ToString("n1") + " kWh";
        health.text = "Hull Integrity " + bathysphere.hullHealth.ToString("n0") + "%";
        h2collection.text = bathysphere.storage.ToString("n0");
        cash.text = "$" + bathysphere.cash.ToString("n0");
        
        if ( bathysphere.powerDemand > 10 )
        {
            powerLevel1.SetActive(true);
            powerLevel2.SetActive(true);
            powerLevel3.SetActive(true);
        }
        else if (bathysphere.powerDemand > 1)
        {
            powerLevel1.SetActive(true);
            powerLevel2.SetActive(true);
            powerLevel3.SetActive(false);
        }
        else if (bathysphere.powerDemand > 0.1)
        {
            powerLevel1.SetActive(true);
            powerLevel2.SetActive(false);
            powerLevel3.SetActive(false);
        }
        else
        {
            powerLevel1.SetActive(false);
            powerLevel2.SetActive(false);
            powerLevel3.SetActive(false);
        }

        if ( bathysphere.emergencyMessage != null && alertMessage == null) 
        {
            alertMessage = Instantiate(messagePrefab,messagePanel.transform);
            alertMessage.GetComponent<Message>().SetMessage(bathysphere.emergencyMessage);
        }

        if (bathysphere.emergencyMessage == "" && alertMessage != null)
        {
            Destroy(alertMessage);
            alertMessage = null;
        }

        switch (bathysphere.status)
        {
            case Bathysphere.ShipStatus.DEAD:
                gameover.SetActive(true);
                break;
            case Bathysphere.ShipStatus.DOCKED:
                reelButton.SetActive(false);
                stopButton.SetActive(false);
                dropButton.SetActive(true);
                dockedPanel.SetActive(true);
                break;
            case Bathysphere.ShipStatus.IDLE:
                reelButton.SetActive(true);
                stopButton.SetActive(false);
                dropButton.SetActive(true);
                dockedPanel.SetActive(false);
                break;
            case Bathysphere.ShipStatus.DESCENDING:
                reelButton.SetActive(true);
                stopButton.SetActive(true);
                dropButton.SetActive(false);
                dockedPanel.SetActive(false);
                break;
            case Bathysphere.ShipStatus.ASCENDING:
                stopButton.SetActive(true);
                reelButton.SetActive(false);
                dropButton.SetActive(false);
                dockedPanel.SetActive(false);
                break;
            case Bathysphere.ShipStatus.SURFACE:
                youwon.SetActive(true);
                break;
            case Bathysphere.ShipStatus.CORE:
                youwon.SetActive(true);
                break;
        }
    }

    public void RestartGame()
    {
        JupiterDrop.instance.SetupGame();
    }

    public void SendStopSignal()
    {
        bathysphere.Stop();
    }

    public void SendSelfDestructSignal()
    {
        bathysphere.SelfDestruct();
    }

    public void SendDropSignal()
    {
        bathysphere.StartDescent();
    }
    public void SendReelSignal()
    {
        bathysphere.StartAscent();
    }

    public void SendRefuelSignal()
    {
        bathysphere.Recharge();
    }
    public void SendSellSignal()
    {
        bathysphere.SellStorage();
    }

    public void ContinueGame()
    {
        bathysphere.Stop();
        youwon.SetActive(false);
        
    }
}

