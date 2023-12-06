using System.Collections.Generic;
using UnityEngine;

namespace SS.ScriptableObjects
{
    using Data;
    using Enumerations;

    public class SSTaskNodeSO : SSNodeSO
    {
        [field: SerializeField] public string Description { get; set; }
        [field: SerializeField] public SSTaskStatus TaskStatus { get; set; }
        [field: SerializeField] public SSTaskType TaskType { get; set; }
        [field: SerializeField] public Sprite Icon { get; set; }
        [field: SerializeField] public float TimeLeft { get; set; }
        [field: SerializeField] public float Duration { get; set; }
        [field: SerializeField] public int MandatorySlots { get; set; }
        [field: SerializeField] public int OptionalSlots { get; set; }
        [field: SerializeField] public float TaskHelpFactor { get; set; }
        [field: SerializeField] public RoomType Room { get; set; }

        public void Initialize(string nodeName, List<SSNodeChoiceData> choices, SSNodeType nodeType,
            bool isStartingNode, string descriptionTask, SSTaskStatus taskStatus, SSTaskType taskType, Sprite taskIcon, float timeLeft, float duration,
            int mandatorySlots, int optionalSlots, float taskHelpFactor, RoomType room)
        {
            NodeName = nodeName;
            Choices = choices;
            NodeType = nodeType;
            IsStartingNode = isStartingNode;
            Description = descriptionTask;
            TaskStatus = taskStatus;
            TaskType = taskType;
            Icon = taskIcon;
            TimeLeft = timeLeft;
            Duration = duration;
            MandatorySlots = mandatorySlots;
            OptionalSlots = optionalSlots;
            TaskHelpFactor = taskHelpFactor;
            Room = room;
        }
    }
}