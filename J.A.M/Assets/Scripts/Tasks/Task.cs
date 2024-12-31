using System;
using System.Collections.Generic;
using SS.Enumerations;
using CharacterSystem;
using SS.ScriptableObjects;
using UnityEngine;

namespace Tasks {
    public class Task {
        public string Name;
        public string Description;
        public string NameStoryline;
        public SSTaskType TaskType;
        public Sprite Icon;
        public float TimeLeft;
        public float Duration;
        public float BaseDuration;
        public int MandatorySlots;
        public int OptionalSlots;
        public float HelpFactor;
        public RoomType Room;
        public bool IsTaskTutorial;
        public List<Tuple<ConditionSO, string>> Conditions;
        public int conditionIndex = 0;
        public List<CharacterBehaviour> leaderCharacters = new();
        public List<CharacterBehaviour> assistantCharacters = new();
        public string previewText;
        public bool IsStarted;

        public Task(SSTaskNodeSO node, string nameStoryline, List<Tuple<ConditionSO, string>> conditions,
            bool isStarted = false) {
            Name = node.name;
            Description = node.Description;
            NameStoryline = nameStoryline;
            TaskType = node.TaskType;
            Icon = node.Icon;
            TimeLeft = node.TimeLeft;
            Duration = node.Duration;
            BaseDuration = node.Duration;
            MandatorySlots = node.MandatorySlots;
            OptionalSlots = node.OptionalSlots;
            HelpFactor = node.TaskHelpFactor;
            Room = node.Room;
            IsTaskTutorial = node.IsTaskTutorial;
            Conditions = conditions;
            previewText = "";
            IsStarted = isStarted;
        }
    }
}