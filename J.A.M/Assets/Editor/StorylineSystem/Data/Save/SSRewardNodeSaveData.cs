using System;
using UnityEngine;

namespace SS.Data.Save
{
    using Enumerations;
    
    [Serializable]
    public class SSRewardNodeSaveData : SSNodeSaveData
    {
        [field: SerializeField] public SSRewardType RewardType { get; set; }
    }
}