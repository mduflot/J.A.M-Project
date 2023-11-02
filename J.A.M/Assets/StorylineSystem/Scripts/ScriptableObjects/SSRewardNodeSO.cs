using System.Collections.Generic;
using UnityEngine;

namespace SS.ScriptableObjects
{
    using Enumerations;
    using Data;
    
    public class SSRewardNodeSO : SSNodeSO
    {
        [field: SerializeField] public SSRewardType RewardType { get; set; }
        
        public void Initialize(string nodeName, List<SSNodeChoiceData> choices, SSNodeType nodeType, bool isStartingNode, SSRewardType rewardType)
        {
            NodeName = nodeName;
            Choices = choices;
            NodeType = nodeType;
            IsStartingNode = isStartingNode;
            RewardType = rewardType;
        }
    }
}