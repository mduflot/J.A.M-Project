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
        [field: SerializeField] public TraitsData.Job Jobs { get; set; }
        [field: SerializeField] public TraitsData.PositiveTraits PositiveTraits { get; set; }
        [field: SerializeField] public TraitsData.NegativeTraits NegativeTraits { get; set; }
        [field: SerializeField] public bool IsUnlockStoryline { get; set; }
        [field: SerializeField] public bool IsUnlockTimeline { get; set; }
        [field: SerializeField] public List<SerializableTuple<SSStatus, SSNodeContainerSO>> StatusNodeContainers { get; set; }
        [field: SerializeField] public List<SerializableTuple<SSStatus, SSNodeGroupSO>> StatusNodeGroups { get; set; }
    }
}