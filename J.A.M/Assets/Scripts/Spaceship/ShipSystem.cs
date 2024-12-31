﻿using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShipSystem {
    public SystemType type;
    public GameObject systemObject;
    public List<float> decreaseValues;
    [Range(0, 50)] public float gaugeValue;
    public float previewGaugeValue;
    public bool isBlocked;
    public float maxGauge = 50.0f;
}