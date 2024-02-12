using System;
using UnityEngine;

namespace SS.Data.Save
{
    using Enumerations;
    
    [Serializable]
    public class SSDialogueNodeSaveData : SSNodeSaveData
    {
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public SSSpeakerType SpeakerType { get; set; }
        [field: SerializeField] public float Duration { get; set; }
        [field: SerializeField] public SSBarkType BarkType { get; set; }
        [field: SerializeField] public bool IsDialogueTask { get; set; }
        [field: SerializeField] public int PercentageTask { get; set; }
        [field: SerializeField] public TraitsData.Job Job { get; set; }
        [field: SerializeField] public TraitsData.PositiveTraits PositiveTraits { get; set; }
        [field: SerializeField] public TraitsData.NegativeTraits NegativeTraits { get; set; }
    }
}