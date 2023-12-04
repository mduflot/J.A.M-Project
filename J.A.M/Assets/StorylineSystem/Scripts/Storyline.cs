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
        public List<SSNodeGroupSO> Timelines;

        public Storyline(SSNodeContainerSO storylineContainer, List<SSNodeGroupSO> timelines)
        {
            StorylineContainer = storylineContainer;
            StoryStatus = SSStoryStatus.Enabled;
            Timelines = timelines;
        }
    }
}