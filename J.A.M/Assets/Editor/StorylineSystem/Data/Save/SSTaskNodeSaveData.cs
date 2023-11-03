using System;
using UnityEngine;

namespace SS.Data.Save
{
    [Serializable]
    public class SSTaskNodeSaveData : SSNodeSaveData
    {
        [field: SerializeField] public TaskDataScriptable TaskData { get; set; }
    }
}