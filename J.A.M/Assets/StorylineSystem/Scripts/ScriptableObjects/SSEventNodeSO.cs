using System.Collections.Generic;
using UnityEngine;

namespace SS.ScriptableObjects
{
    using Data;
    using Enumerations;
    
    public class SSEventNodeSO : SSNodeSO
    {
        [field: SerializeField] [field: TextArea()] public string Text { get; set; }
        
        public void Initialize(string nodeName, string text, List<SSNodeChoiceData> choices, SSNodeType nodeType, bool isStartingNode)
        {
            NodeName = nodeName;
            Text = text;
            Choices = choices;
            NodeType = nodeType;
            IsStartingNode = isStartingNode;
        }
    }
}