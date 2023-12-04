using System;
using System.Collections.Generic;
using SS.Enumerations;
using SS.ScriptableObjects;
using UnityEngine;

namespace SS.Data.Save
{
    [Serializable]
    public class SSChoiceTaskSaveData : SSChoiceSaveData
    {
        [field: SerializeField] public ConditionSO Condition { get; set; }
    }
}