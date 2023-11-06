using System.Collections.Generic;
using UnityEngine;

namespace SS.ScriptableObjects
{
    using Data;
    using Enumerations;
    
    public class SSTimeNodeSO : SSNodeSO
    {
        [field: SerializeField] public uint TimeToWait { get; set; }
        
        public void Initialize(string nodeName, List<SSNodeChoiceData> choices, SSNodeType nodeType, bool isStartingNode, uint timeToWait)
        {
            NodeName = nodeName;
            Choices = choices;
            NodeType = nodeType;
            IsStartingNode = isStartingNode;
            TimeToWait = timeToWait;
        }   
    }
}