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
        [field: SerializeField] public TraitsData.Job Jobs { get; set; }
        [field: SerializeField] public TraitsData.PositiveTraits PositiveTraits { get; set; }
        [field: SerializeField] public TraitsData.NegativeTraits NegativeTraits { get; set; }
        [field: SerializeField] public bool IsUnlockStoryline { get; set; }
        [field: SerializeField] public bool IsUnlockTimeline { get; set; }
        [field: SerializeField] public List<SerializableTuple<SSStatus, SSNodeContainerSO>> StatusNodeContainers { get; set; }
        [field: SerializeField] public List<SerializableTuple<SSStatus, SSNodeGroupSO>> StatusNodeGroups { get; set; }

        // TODO : Maybe think about a way to store the conditions of the storylines and timelines
        // Maybe a class that stores the conditions

        // TODO : List of SerializableTuple of conditions and outcomes
        // [field: SerializeField] public List<SerializableTuple<>>
    }
}