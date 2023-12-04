using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Condition")]
public class ConditionSO : ScriptableObject
{
    public OutcomeData.OutcomeTarget target;
    public Condition BaseCondition;
    public SerializableDictionary<TraitsData.TraitOperator, Condition> supplementaryConditions;

    public ConditionSO[] additionnalConditions;
    
    public OutcomeSO outcomes;
}
