using System.Collections.Generic;
using UnityEngine;

namespace SS.ScriptableObjects
{
    using Data;
    using Enumerations;
    
    public class SSTaskNodeSO : SSNodeSO
    {
        [field: SerializeField] public TaskDataScriptable TaskData { get; set; }
        
        public void Initialize(string nodeName, List<SSNodeChoiceData> choices, SSNodeType nodeType, bool isStartingNode, TaskDataScriptable taskData)
        {
            NodeName = nodeName;
            Choices = choices;
            NodeType = nodeType;
            IsStartingNode = isStartingNode;
            TaskData = taskData;
        }
    }
}