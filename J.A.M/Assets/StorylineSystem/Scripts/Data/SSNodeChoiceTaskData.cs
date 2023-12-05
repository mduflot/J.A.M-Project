using System;
using UnityEngine;

namespace SS.Data
{
    [Serializable]
    public class SSNodeChoiceTaskData : SSNodeChoiceData
    {
        [field: SerializeField] public ConditionSO Condition { get; set; }
        [field: SerializeField] public string PreviewOutcome { get; set; }
    }
}