using System.Collections.Generic;
using UnityEngine;

namespace SS.ScriptableObjects
{
    using Data;
    using Enumerations;

    public class SSTaskNodeSO : SSNodeSO
    {
        [field: SerializeField] public string Description { get; set; }
        [field: SerializeField] public Sprite Icon { get; set; }
        [field: SerializeField] public float TimeLeft { get; set; }
        [field: SerializeField] public float Duration { get; set; }
        [field: SerializeField] public int MandatorySlots { get; set; }
        [field: SerializeField] public int OptionalSlots { get; set; }
        [field: SerializeField] public float TaskHelpFactor { get; set; }
        [field: SerializeField] public SpaceshipManager.RoomType Room { get; set; }
        [field: SerializeField] public bool IsPermanent { get; set; }
        [field: SerializeField] public string PreviewOutcome { get; set; }

        public void Initialize(string nodeName, List<SSNodeChoiceData> choices, SSNodeType nodeType,
            bool isStartingNode, string descriptionTask, Sprite taskIcon, float timeLeft, float duration,
            int mandatorySlots, int optionalSlots, float taskHelpFactor, SpaceshipManager.RoomType room,
            bool isPermanent, string previewOutcome)
        {
            NodeName = nodeName;
            Choices = choices;
            NodeType = nodeType;
            IsStartingNode = isStartingNode;
            Description = descriptionTask;
            Icon = taskIcon;
            TimeLeft = timeLeft;
            Duration = duration;
            MandatorySlots = mandatorySlots;
            OptionalSlots = optionalSlots;
            TaskHelpFactor = taskHelpFactor;
            Room = room;
            IsPermanent = isPermanent;
            PreviewOutcome = previewOutcome;
        }
    }
}