using UnityEngine;

namespace SS.Data.Save
{
    public class SSTaskNodeSaveData : SSNodeSaveData
    {
        [field: SerializeField] public TaskDataScriptable TaskData { get; set; }
    }
}