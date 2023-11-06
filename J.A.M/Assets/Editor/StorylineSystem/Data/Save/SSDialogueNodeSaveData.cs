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
        [field: SerializeField] public uint TimeToWait { get; set; }
        [field: SerializeField] public bool IsDialogueTask { get; set; }
        [field: SerializeField] public int PercentageTask { get; set; }
    }
}