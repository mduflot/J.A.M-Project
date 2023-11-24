using System.Collections.Generic;
using UnityEngine;

namespace SS.ScriptableObjects
{
    using Data;
    using Enumerations;

    public class SSTaskNodeSO : SSNodeSO
    {
        [field: SerializeField] public string DescriptionTask { get; set; }
        [field: SerializeField] public Sprite TaskIcon { get; set; }
        [field: SerializeField] public float TimeLeft { get; set; }
        [field: SerializeField] public float BaseDuration { get; set; }
        [field: SerializeField] public int MandatorySlots { get; set; }
        [field: SerializeField] public int OptionalSlots { get; set; }
        [field: SerializeField] public float TaskHelpFactor { get; set; }
        [field: SerializeField] public SpaceshipManager.ShipRooms Room { get; set; }
        [field: SerializeField] public bool IsPermanent { get; set; }

        public void Initialize(string nodeName, List<SSNodeChoiceData> choices, SSNodeType nodeType,
            bool isStartingNode, string descriptionTask, Sprite taskIcon, float timeLeft, float baseDuration,
            int mandatorySlots, int optionalSlots, float taskHelpFactor, SpaceshipManager.ShipRooms room,
            bool isPermanent)
        {
            NodeName = nodeName;
            Choices = choices;
            NodeType = nodeType;
            IsStartingNode = isStartingNode;
            DescriptionTask = descriptionTask;
            TaskIcon = taskIcon;
            TimeLeft = timeLeft;
            BaseDuration = baseDuration;
            MandatorySlots = mandatorySlots;
            OptionalSlots = optionalSlots;
            TaskHelpFactor = taskHelpFactor;
            Room = room;
            IsPermanent = isPermanent;
        }
    }
}