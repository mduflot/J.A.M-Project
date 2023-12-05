using System;
using UnityEngine;

namespace SS.Data.Save
{
    [Serializable]
    public class SSChoiceTaskSaveData : SSChoiceSaveData
    {
        [field: SerializeField] public ConditionSO Condition { get; set; }
        [field: SerializeField] public string PreviewOutcome { get; set; }
    }
}