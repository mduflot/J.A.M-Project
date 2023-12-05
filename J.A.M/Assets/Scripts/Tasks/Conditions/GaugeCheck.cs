using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class GaugeCheck
{
    public enum GaugeValue
    {
        Any,
        Low,
        Average,
        High
    }

    public static GaugeValue GetGaugeValue(SystemType gaugeType)
    {
        var gaugeValue = GameManager.Instance.SpaceshipManager.GetGaugeValue(gaugeType);
        var maxGaugeValue = 100.0f; //FIX : Arbitrary value
        var gaugePercent = gaugeValue / maxGaugeValue;
        
        if (gaugePercent >= .75f) return GaugeValue.High;

        if (gaugePercent < .75f && gaugePercent >= .3f) return GaugeValue.Average;

        return GaugeValue.Low;
    }
}
