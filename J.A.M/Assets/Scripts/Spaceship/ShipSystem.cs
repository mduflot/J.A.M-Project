using System;
using UnityEngine;

[Serializable]
public class ShipSystem
{
    public SystemType type;
    public GameObject systemObject;
    public float decreaseSpeed;
    [Range(0, 20)] public float gaugeValue;
}