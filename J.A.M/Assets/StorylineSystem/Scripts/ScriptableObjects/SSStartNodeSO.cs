using System.Collections.Generic;
using UnityEngine;

namespace SS.ScriptableObjects
{
    using Enumerations;
    using Data;
    
    public class SSStartNodeSO : SSNodeSO
    {
        [field: SerializeField] public SSLocationType LocationType { get; set; }

        public void Initialize(string nodeName, string text, List<SSNodeChoiceData> choices, SSNodeType nodeType, bool isStartingNode, SSLocationType locationType)
        {
            NodeName = nodeName;
            Text = text;
            Choices = choices;
            NodeType = nodeType;
            IsStartingNode = isStartingNode;
            LocationType = locationType;
        }
    }
}