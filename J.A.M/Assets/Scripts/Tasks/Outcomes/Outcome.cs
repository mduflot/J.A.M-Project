using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[Serializable]
public class Outcome
{
    [Tooltip("The desired outcome.")]
    public OutcomeData.OutcomeType OutcomeType;

    [Tooltip("Indicate if data has to be added or substracted / removed (Depending on whether it affects numerical values or traits")]
    public OutcomeData.OutcomeOperation OutcomeOperation;

    [Tooltip("Targeted stat to which value will be added / substracted (only affects Character stats)")]
    public OutcomeData.OutcomeTargetStat OutcomeTargetStat;
    
    [Tooltip("Numerical value that will be added / substracted (only affects Character stats or Gauge")]
    [Range(0,30)]
    public float value;
    
    [Tooltip("Trait that will be added / removed (only affects Trait")]
    public TraitsData.Traits OutcomeTargetTrait;
    
    [Tooltip("The element affected by the outcome")]
    public OutcomeData.OutcomeTarget OutcomeTarget;
}
