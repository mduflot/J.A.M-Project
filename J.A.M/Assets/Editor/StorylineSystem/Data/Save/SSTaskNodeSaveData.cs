using System;
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
        [field: SerializeField] public SpaceshipManager.RoomType Room { get; set; }
        [field: SerializeField] public bool IsPermanent { get; set; }
        [field: SerializeField] public string PreviewOutcome { get; set; }
    }
}