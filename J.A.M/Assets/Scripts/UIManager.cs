using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Gauges[] gauges;
    private Dictionary<SpaceshipManager.System, Image> gaugeReferences = new Dictionary<SpaceshipManager.System, Image>();
    
    [Serializable] 
    public struct Gauges
    {
        public SpaceshipManager.System system;
        public Image gauge;
    }

    private void Start()
    {
        InitializeGauges();
    }

    private void InitializeGauges()
    {
        for (int i = 0; i < gauges.Length; i++)
        {
            gaugeReferences.Add(gauges[i].system, gauges[i].gauge);
        }
    }
    public void UpdateGauges(SpaceshipManager.System system, float value)
    {
        gaugeReferences[system].fillAmount = value/100;
    }
}
