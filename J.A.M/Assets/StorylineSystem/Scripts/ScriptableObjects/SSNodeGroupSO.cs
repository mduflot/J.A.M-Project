using UnityEngine;

namespace SS.ScriptableObjects
{
    using Enumerations;

    public class SSNodeGroupSO : ScriptableObject
    {
        [field: SerializeField] public string GroupName { get; set; }
        [field: SerializeField] public SSStoryStatus StoryStatus { get; set; }

        public void Initialize(string groupName, SSStoryStatus storyStatus)
        {
            GroupName = groupName;
            StoryStatus = storyStatus;
        }
    }
}