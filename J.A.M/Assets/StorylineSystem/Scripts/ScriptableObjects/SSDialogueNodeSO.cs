using System.Collections.Generic;
using UnityEngine;

namespace SS.ScriptableObjects
{
    using Enumerations;
    using Data;

    public class SSDialogueNodeSO : SSNodeSO
    {
        [field: SerializeField] public SSDialogueType DialogueType { get; set; }
        [field: SerializeField] [field: TextArea()] public string Text { get; set; }
        [field: SerializeField] public SSSpeakerType SpeakerType { get; set; }
        [field: SerializeField] public float Duration { get; set; }
        [field: SerializeField] public SSBarkType BarkType { get; set; }
        [field: SerializeField] public bool IsDialogueTask { get; set; }
        [field: SerializeField] public int PercentageTask { get; set; }
        [field: SerializeField] public bool IsCompleted { get; set; }
        [field: SerializeField] public TraitsData.Job Job { get; set; }
        [field: SerializeField] public TraitsData.PositiveTraits PositiveTraits { get; set; }
        [field: SerializeField] public TraitsData.NegativeTraits NegativeTraits { get; set; }

        public void Initialize(string nodeName, SSDialogueType dialogueType, string text, List<SSNodeChoiceData> choices, SSNodeType nodeType,
            bool isStartingNode, SSSpeakerType speakerType, float duration, SSBarkType barkType, bool isDialogueTask, int percentageTask,
            TraitsData.Job job, TraitsData.PositiveTraits positiveTraits, TraitsData.NegativeTraits negativeTraits)
        {
            NodeName = nodeName;
            DialogueType = dialogueType;
            Text = text;
            Choices = choices;
            NodeType = nodeType;
            IsStartingNode = isStartingNode;
            SpeakerType = speakerType;
            Duration = duration;
            BarkType = barkType;
            IsDialogueTask = isDialogueTask;
            PercentageTask = percentageTask;
            IsCompleted = false;
            Job = job;
            PositiveTraits = positiveTraits;
            NegativeTraits = negativeTraits;
        }
    }
}