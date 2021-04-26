using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    public Bathysphere bathysphere;
    public GameObject lerpTo;
    public float speed;
    public float depthFrom;
    public float depthTo;
    public bool loop;
    
    float startDepth;

    public Vector3 pos = new Vector3();

    void Start()
    {
        startDepth = depthFrom;    
        pos = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (lerpTo != null)
        {
            float lerp = GetLerpRange(depthFrom, bathysphere.depth, depthTo);
            gameObject.transform.position = Vector3.Lerp(pos, lerpTo.transform.position, lerp);
            if (loop && lerp > 1.0)
            {
                gameObject.transform.position = pos;
                depthFrom -= 200;
                depthTo -= 200;
            }
        }
    }
    float GetLerpRange(float from, float current, float to)
    {
        if (from == current) return 0.0f;
        if (to == current) return 1.0f;
        return (current - from) / (to - from);
    }
}
