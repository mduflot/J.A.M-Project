using System.Collections.Generic;
using UnityEngine;

namespace SS.ScriptableObjects
{
    using Data;
    using Enumerations;

    public class SSSoundNodeSO : SSNodeSO
    {
        [field: SerializeField] public AudioClip AudioClip { get; set; }

        public void Initialize(string nodeName, List<SSNodeChoiceData> choices, SSNodeType nodeType,
            bool isStartingNode, AudioClip audioClip)
        {
            NodeName = nodeName;
            Choices = choices;
            NodeType = nodeType;
            IsStartingNode = isStartingNode;
            AudioClip = audioClip;
        }
    }
}