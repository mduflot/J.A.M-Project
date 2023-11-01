using UnityEngine;

namespace SS.Data.Save
{
    public class SSEventNodeSaveData : SSNodeSaveData
    {
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public int LeaderCount { get; set; }
    }
}