using System.Collections.Generic;
using UnityEngine;

namespace SS.ScriptableObjects
{
    using Enumerations;
    using Data;
    
    public class SSDialogueNodeSO : SSNodeSO
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