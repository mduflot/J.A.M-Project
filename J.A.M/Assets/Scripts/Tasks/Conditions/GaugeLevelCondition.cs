using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Task/TaskCondition/GaugeLevelCondition", fileName = "GaugeLevelCondition")]

public class GaugeLevelCondition : BaseTaskCondition
{
    public enum Operation
    {
        Higher, Smaller
    }
    
    public SystemType targetGauge;

    public Operation operation;

    [Range(0, 20)] public float threshold;

    public override bool Condition()
    {
        if (operation == Operation.Higher)
        {
            return GameManager.Instance.SpaceshipManager.GetGaugeValue(targetGauge) >= threshold;
        }
        else
        {
            return GameManager.Instance.SpaceshipManager.GetGaugeValue(targetGauge) <= threshold;
        }
    }
}
