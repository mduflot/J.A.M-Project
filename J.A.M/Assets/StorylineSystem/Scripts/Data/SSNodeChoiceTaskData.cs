using System;
using System.Collections.Generic;
using UnityEngine;

namespace SS.Data
{
    using Enumerations;
    using ScriptableObjects;

    [Serializable]
    public class SSNodeChoiceTaskData : SSNodeChoiceData
    {
        [field: SerializeField] public ConditionSO Condition { get; set; }
        [field: SerializeField] public bool IsUnlockStoryline { get; set; }
        [field: SerializeField] public bool IsUnlockTimeline { get; set; }
        [field: SerializeField] public List<SerializableTuple<SSStoryStatus, SSNodeContainerSO>> StatusNodeContainers { get; set; }
        [field: SerializeField] public List<SerializableTuple<SSStoryStatus, SSNodeGroupSO>> StatusNodeGroups { get; set; }
    }
}