using System;
using UnityEngine;

namespace SS.Data.Save
{
    [Serializable]
    public class SSDialogueNodeSaveData : SSNodeSaveData
    {
        [field: SerializeField] public string Text { get; set; }
    }
}