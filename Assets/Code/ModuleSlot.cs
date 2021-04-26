using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModuleSlot : MonoBehaviour
{
    public Module module;

    public Image icon;
    public Text nameText;
    public Text costText;

    public UI store;

    private void Update()
    {
        nameText.text = module.moduleName;
        icon.sprite = module.icon;
        if ( costText != null)
        {
            costText.text = "$" + module.price.ToString("n0");
        }
    }

    public void Purchase()
    {
        if (store.bathysphere.cash < module.price || store.bathysphere.modules.Count >= 5 ) return;
        store.InstallModule(module);
    }
}
