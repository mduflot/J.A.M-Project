using System;
using UnityEngine;

namespace SS.Data.Save
{
    [Serializable]
    public class SSTimeNodeSaveData : SSNodeSaveData
    {
        [field: SerializeField] public uint TimeToWait { get; set; }
    }
}