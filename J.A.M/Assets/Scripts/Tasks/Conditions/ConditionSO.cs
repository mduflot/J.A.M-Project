using UnityEngine;

[CreateAssetMenu(fileName = "Condition")]
public class ConditionSO : ScriptableObject
{
    [Header("Conditions")] public Condition BaseCondition;

    [Header("Stat comparison (if target is a character)")]
    public OutcomeData.OutcomeTargetStat targetStat;

    public ConditionSystem.ComparisonOperator statComparison;
    public float statThreshold;

    [Header("Target Gauge (if target is a gauge)")]
    public SystemType targetGauge;

    public SerializableDictionary<TraitsData.TraitOperator, ConditionSO> supplementaryConditions;

    public ConditionSO[] additionnalConditions;

    public OutcomeSO outcomes;
}