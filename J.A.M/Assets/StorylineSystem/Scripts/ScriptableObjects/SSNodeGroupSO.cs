using UnityEngine;

namespace SS.ScriptableObjects
{
    using Enumerations;

    public class SSNodeGroupSO : ScriptableObject
    {
        [field: SerializeField] public string GroupName { get; set; }
        [field: SerializeField] public SSStoryStatus StoryStatus { get; set; }
        [field: SerializeField] public bool IsFirstToPlay { get; set; }
        [field: SerializeField] public uint MinWaitTime { get; set; }
        [field: SerializeField] public uint MaxWaitTime { get; set; }
        [field: SerializeField] public bool TimeIsOverride { get; set; }
        [field: SerializeField] public uint OverrideWaitTime { get; set; }
        [field: SerializeField] public ConditionSO Condition { get; set; }

        public void Initialize(string groupName, SSStoryStatus storyStatus, bool isFirstToPlay, uint minWaitTime, uint maxWaitTime, bool timeIsOverride, uint overrideWaitTime, ConditionSO condition)
        {
            GroupName = groupName;
            StoryStatus = storyStatus;
            IsFirstToPlay = isFirstToPlay;
            MinWaitTime = minWaitTime;
            MaxWaitTime = maxWaitTime;
            TimeIsOverride = timeIsOverride;
            OverrideWaitTime = overrideWaitTime;
            Condition = condition;
        }
    }
}