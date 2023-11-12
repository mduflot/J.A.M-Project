using System.Collections.Generic;
using UnityEngine;

namespace SS.ScriptableObjects
{
    using Enumerations;
    using Data;

    public class SSDialogueNodeSO : SSNodeSO
    {
        [field: SerializeField] [field: TextArea()] public string Text { get; set; }

        [field: SerializeField] public SSSpeakerType SpeakerType { get; set; }
        [field: SerializeField] public uint Duration { get; set; }
        [field: SerializeField] public bool IsDialogueTask { get; set; }
        [field: SerializeField] public int PercentageTask { get; set; }

        public void Initialize(string nodeName, string text, List<SSNodeChoiceData> choices, SSNodeType nodeType,
            bool isStartingNode, SSSpeakerType speakerType, uint duration, bool isDialogueTask, int percentageTask)
        {
            NodeName = nodeName;
            Text = text;
            Choices = choices;
            NodeType = nodeType;
            IsStartingNode = isStartingNode;
            SpeakerType = speakerType;
            Duration = duration;
            IsDialogueTask = isDialogueTask;
            PercentageTask = percentageTask;
        }
    }
}