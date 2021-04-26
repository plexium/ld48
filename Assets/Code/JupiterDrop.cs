using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JupiterDrop : MonoBehaviour
{
    public static JupiterDrop instance = null;

    public JupiterLayer startingLayer;

    public GameObject topStage;
    public GameObject bottomStage;
    public GameObject shipSpawn;

    public GameObject dropShipPrefab;
    public GameObject bathyspherePrefab;
    public GameObject backgroundPrefab;
    public GameObject wispPrefab;
    public GameObject uiPrefab;

    public Bathysphere bathysphere;
    public UI ui;

    private void Awake()
    {
        if ( instance != null && instance != this )
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetupGame();
    }

    private void Update()
    {
        if ( Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }
    public void SetupGame()
    {
        if (bathysphere != null)
        {
            Destroy(bathysphere.gameObject);
            bathysphere = null;
        }

        if (ui != null)
        {
            Destroy(ui.gameObject);
            ui = null;
        }

        bathysphere = Instantiate(bathyspherePrefab, shipSpawn.transform).GetComponent<Bathysphere>();
        ui = Instantiate(uiPrefab).GetComponent<UI>();
        ui.bathysphere = bathysphere;

        Prop ship = Instantiate(dropShipPrefab, shipSpawn.transform).GetComponent<Prop>();
        ship.lerpTo = topStage;
        ship.bathysphere = bathysphere;

        Prop bg = Instantiate(backgroundPrefab, shipSpawn.transform).GetComponent<Prop>();
        bg.lerpTo = topStage;
        bg.bathysphere = bathysphere;

        Prop wisp = Instantiate(wispPrefab, bottomStage.transform).GetComponent<Prop>();
        wisp.lerpTo =topStage;
        wisp.bathysphere = bathysphere;

    }
}
