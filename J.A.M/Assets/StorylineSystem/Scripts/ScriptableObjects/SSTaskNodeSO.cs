using System.Collections.Generic;
using UnityEngine;

namespace SS.ScriptableObjects
{
    using Data;
    using Enumerations;
    
    public class SSTaskNodeSO : SSNodeSO
    {
        [field: SerializeField] [field: TextArea()] public string Text { get; set; }
        [field: SerializeField] public int LeaderCount { get; set; }
        [field: SerializeField] public float ResolutionTime { get; set; }
        
        public void Initialize(string nodeName, string text, List<SSNodeChoiceData> choices, SSNodeType nodeType, bool isStartingNode, int leaderCount, float resolutionTime)
        {
            NodeName = nodeName;
            Text = text;
            Choices = choices;
            NodeType = nodeType;
            IsStartingNode = isStartingNode;
            LeaderCount = leaderCount;
            ResolutionTime = resolutionTime;
        }
    }
}