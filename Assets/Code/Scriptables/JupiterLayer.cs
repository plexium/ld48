using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Jupiter Layer")]
public class JupiterLayer : ScriptableObject
{
    public string layerName;
    public float depth;
    public float temp;
    public float pressure;
    public float hydrogenDensity;
    public float peakSpeed;
    public Color bgColor;

    public JupiterLayer next;
    public JupiterLayer previous;
    
}
