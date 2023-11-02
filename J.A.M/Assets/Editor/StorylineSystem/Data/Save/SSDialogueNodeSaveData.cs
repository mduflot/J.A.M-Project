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
    }
}