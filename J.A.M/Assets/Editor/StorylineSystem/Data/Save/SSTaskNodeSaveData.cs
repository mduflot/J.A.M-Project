using System;
using System.Collections.Generic;
using SS.Enumerations;
using SS.ScriptableObjects;
using UnityEngine;

namespace SS.Data.Save
{
    [Serializable]
    public class SSTaskNodeSaveData : SSNodeSaveData
    {
        [field: SerializeField] public string DescriptionTask { get; set; }
        [field: SerializeField] public Sprite TaskIcon { get; set; }
        [field: SerializeField] public float TimeLeft { get; set; }
        [field: SerializeField] public float BaseDuration { get; set; }
        [field: SerializeField] public int MandatorySlots { get; set; }
        [field: SerializeField] public int OptionalSlots { get; set; }
        [field: SerializeField] public float TaskHelpFactor { get; set; }
        [field: SerializeField] public SpaceshipManager.ShipRooms Room { get; set; }
        [field: SerializeField] public bool IsPermanent { get; set; }
        [field: SerializeField] public bool IsUnlockStoryline { get; set; }
        [field: SerializeField] public bool IsUnlockTimeline { get; set; }
        [field: SerializeField] public List<SerializableTuple<SSStatus, SSNodeContainerSO>> StatusNodeContainers { get; set; }
        [field: SerializeField] public List<SerializableTuple<SSStatus, SSNodeGroupSO>> StatusNodeGroups { get; set; }
    }
}