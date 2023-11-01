using System.Collections.Generic;
using UnityEngine;

namespace SS.ScriptableObjects
{
    using Enumerations;
    using Data;
    
    public class SSStartNodeSO : SSNodeSO
    {
        [field: SerializeField] public SSLocationType LocationType { get; set; }

        public void Initialize(string nodeName, List<SSNodeChoiceData> choices, SSNodeType nodeType, bool isStartingNode, SSLocationType locationType)
        {
            NodeName = nodeName;
            Choices = choices;
            NodeType = nodeType;
            IsStartingNode = isStartingNode;
            LocationType = locationType;
        }
    }
}