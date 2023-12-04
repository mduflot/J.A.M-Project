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
        [field: SerializeField] public bool IsUnlockStoryline { get; set; }
        [field: SerializeField] public bool IsUnlockTimeline { get; set; }
        [field: SerializeField] public List<SerializableTuple<SSStoryStatus, SSNodeContainerSO>> StatusNodeContainers { get; set; }
        [field: SerializeField] public List<SerializableTuple<SSStoryStatus, SSNodeGroupSO>> StatusNodeGroups { get; set; }
    }
}