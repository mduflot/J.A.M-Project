using System;
using UnityEngine;

namespace SS.Data.Save
{
    [Serializable]
    public class SSCheckConditionNodeSaveData : SSNodeSaveData
    {
        [field: SerializeField] public ConditionSO Condition { get; set; }
    }
}