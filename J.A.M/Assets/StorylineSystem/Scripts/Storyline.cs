using System.Collections.Generic;

namespace SS
{
    using ScriptableObjects;

    [System.Serializable]
    public class Storyline
    {
        public SSNodeContainerSO StorylineContainer;
        public List<SSNodeGroupSO> Timelines;

        public Storyline(SSNodeContainerSO storylineContainer, List<SSNodeGroupSO> timelines)
        {
            StorylineContainer = storylineContainer;
            Timelines = timelines;
        }
    }
}