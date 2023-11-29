using System.Collections.Generic;

namespace SS
{
    using Enumerations;
    using ScriptableObjects;

    [System.Serializable]
    public class Storyline
    {
        public SSNodeContainerSO StorylineContainer;
        public SSStoryStatus StoryStatus;
        public List<SerializableTuple<SSStoryStatus, SSNodeGroupSO>> ActivatableTimelines;
        public List<SerializableTuple<SSStoryStatus, SSNodeGroupSO>> DisabledTimelines;

        public Storyline(SSNodeContainerSO storylineContainer, SSStoryStatus storyStatus,
            List<SerializableTuple<SSStoryStatus, SSNodeGroupSO>> activatableTimelines, List<SerializableTuple<SSStoryStatus, SSNodeGroupSO>> disabledTimelines)
        {
            StorylineContainer = storylineContainer;
            StoryStatus = storyStatus;
            ActivatableTimelines = activatableTimelines;
            DisabledTimelines = disabledTimelines;
        }
    }
}