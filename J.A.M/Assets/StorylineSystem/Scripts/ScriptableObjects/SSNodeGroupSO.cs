using System.Collections.Generic;
using UnityEngine;

namespace SS.ScriptableObjects
{
    using Enumerations;

    public class SSNodeGroupSO : ScriptableObject
    {
        [field: SerializeField] public string GroupName { get; set; }
        [field: SerializeField] public SSStoryStatus StoryStatus { get; set; }
        [field: SerializeField] public bool IsFirstToPlay { get; set; }
        [field: SerializeField] public ConditionSO Condition { get; set; }

        public void Initialize(string groupName, SSStoryStatus storyStatus, bool isFirstToPlay, ConditionSO condition)
        {
            GroupName = groupName;
            StoryStatus = storyStatus;
            IsFirstToPlay = isFirstToPlay;
            Condition = condition;
        }
    }
}