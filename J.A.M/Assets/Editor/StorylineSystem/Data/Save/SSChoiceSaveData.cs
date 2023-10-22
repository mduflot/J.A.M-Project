using System;
using UnityEngine;

namespace SS.Data.Save
{
    [Serializable]
    public class SSChoiceSaveData
    {
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public string NodeID { get; set; }
    }
}