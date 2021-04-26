using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Module")]
public class Module : ScriptableObject
{
    public string moduleName;
    public float price;
    public Sprite icon;
    public float maxPowerBuff;
    public float storageBuff;
    public float hullBuff;
    public float tempBuff;
    public ModuleType type;

    public enum ModuleType
    {
        HULL,
        TEMP,
        STORAGE,
        POWER,
        CONSUMABLE,
    }
}
