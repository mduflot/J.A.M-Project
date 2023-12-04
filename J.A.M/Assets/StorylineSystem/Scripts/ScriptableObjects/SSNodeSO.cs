using System.Collections.Generic;
using UnityEngine;

namespace SS.ScriptableObjects
{
    using Enumerations;
    using Data;
    
    public class SSNodeSO : ScriptableObject
    {
        [field: SerializeField] public string NodeName { get; set; }
        [field: SerializeReference] public List<SSNodeChoiceData> Choices { get; set; }
        [field: SerializeField] public SSNodeType NodeType { get; set; }
        [field: SerializeField] public bool IsStartingNode { get; set; }

        public void Initialize(string nodeName, List<SSNodeChoiceData> choices, SSNodeType nodeType,
            bool isStartingNode)
        {
            NodeName = nodeName;
            Choices = choices;
            NodeType = nodeType;
            IsStartingNode = isStartingNode;
        }
    }
}