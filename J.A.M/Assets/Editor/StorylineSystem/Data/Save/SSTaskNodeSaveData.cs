using System;
using UnityEngine;

namespace SS.Data.Save
{
    using Enumerations;

    [Serializable]
    public class SSTaskNodeSaveData : SSNodeSaveData
    {
        [field: SerializeField] public string DescriptionTask { get; set; }
        [field: SerializeField] public SSTaskStatus TaskStatus { get; set; }
        [field: SerializeField] public SSTaskType TaskType { get; set; }
        [field: SerializeField] public Sprite TaskIcon { get; set; }
        [field: SerializeField] public float TimeLeft { get; set; }
        [field: SerializeField] public float BaseDuration { get; set; }
        [field: SerializeField] public int MandatorySlots { get; set; }
        [field: SerializeField] public int OptionalSlots { get; set; }
        [field: SerializeField] public float TaskHelpFactor { get; set; }
        [field: SerializeField] public RoomType Room { get; set; }
        [field: SerializeField] public FurnitureType Furniture { get; set; }
    }
}