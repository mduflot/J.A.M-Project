using System.Collections.Generic;
using UnityEngine;

namespace SS.ScriptableObjects
{
    using Enumerations;

    public class SSNodeGroupSO : ScriptableObject
    {
        [field: SerializeField] public string GroupName { get; set; }
        [field: SerializeField] public SSStoryStatus StoryStatus { get; set; }
        [field: SerializeField] public SSStoryType StoryType { get; set; }
        [field: SerializeField] public List<ConditionSO> Conditions { get; set; }

        public void Initialize(string groupName, SSStoryType storyType, List<ConditionSO> conditions)
        {
            GroupName = groupName;
            StoryStatus = SSStoryStatus.Enabled;
            StoryType = storyType;
            Conditions = conditions;
        }
    }
}