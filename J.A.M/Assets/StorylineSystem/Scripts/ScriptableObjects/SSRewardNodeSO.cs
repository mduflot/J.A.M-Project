using System.Collections.Generic;
using UnityEngine;

namespace SS.ScriptableObjects
{
    using Enumerations;
    using Data;
    
    public class SSRewardNodeSO : SSNodeSO
    {
        [field: SerializeField] public List<SSRewardType> RewardTypes { get; set; }
        
        public void Initialize(string nodeName, List<SSNodeChoiceData> choices, SSNodeType nodeType, bool isStartingNode, List<SSRewardType> rewardTypes)
        {
            NodeName = nodeName;
            Choices = choices;
            NodeType = nodeType;
            IsStartingNode = isStartingNode;
            RewardTypes = rewardTypes;
        }
    }
}