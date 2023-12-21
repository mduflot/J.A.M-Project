using System;
using UnityEngine;

namespace SS.Data.Save
{
    [Serializable]
    public class SSSoundNodeSaveData : SSNodeSaveData
    {
        [field: SerializeField] public AudioClip AudioClip { get; set; }
    }
}