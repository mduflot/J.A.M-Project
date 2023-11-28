using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Condition")]
public class ConditionSO : ScriptableObject
{
    [SerializeField] public Condition BaseCondition;
    public SerializableDictionary<TraitsData.TraitOperator, Condition> additionnalConditions;
}
