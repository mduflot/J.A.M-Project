using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Condition")]
public class ConditionSO : ScriptableObject
{
    [Header("Target")]
    public OutcomeData.OutcomeTarget target;
    
    [Header("Stat comparison (if target is a character)")]
    public OutcomeData.OutcomeTargetStat targetStat;
    public ConditionSystem.ComparisonOperator statComparison;
    public float statThreshold;
    
    [Header("Target Gauge (if target is a gauge)")]
    public SystemType targetGauge;
    
    [Header("Conditions")]
    public Condition BaseCondition;
    public SerializableDictionary<TraitsData.TraitOperator, Condition> supplementaryConditions;

    public ConditionSO[] additionnalConditions;
    
    public OutcomeSO outcomes;
}
