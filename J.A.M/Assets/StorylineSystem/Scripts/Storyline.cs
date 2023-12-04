using System.Collections.Generic;
using SS.Enumerations;

namespace SS
{
    using ScriptableObjects;

    [System.Serializable]
    public class Storyline
    {
        public SSNodeContainerSO StorylineContainer;
        public SSStoryStatus StoryStatus;
        public List<SSNodeGroupSO> EnabledTimelines;
        public List<SSNodeGroupSO> DisabledTimelines;

        public Storyline(SSNodeContainerSO storylineContainer, SSStoryStatus storyStatus, List<SSNodeGroupSO> enabledTimelines, List<SSNodeGroupSO> disabledTimelines)
        {
            StorylineContainer = storylineContainer;
            StoryStatus = storyStatus;
            EnabledTimelines = enabledTimelines;
            DisabledTimelines = disabledTimelines;
        }
    }
}