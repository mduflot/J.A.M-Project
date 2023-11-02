using System;
using System.Collections.Generic;
using UnityEngine;

namespace SS.Data.Save
{
    using Enumerations;

    [Serializable]
    public class SSRewardNodeSaveData : SSNodeSaveData
    {
        [field: SerializeField] public List<SSRewardType> RewardTypes { get; set; }
    }
}