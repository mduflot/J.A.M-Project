using System.Collections.Generic;

namespace SS
{
    using Enumerations;
    using ScriptableObjects;

    [System.Serializable]
    public class Storyline
    {
        public SSNodeContainerSO NodeContainer;
        public SSStorylineStatus Status;
        public List<SerializableTuple<SSStorylineStatus, SSNodeGroupSO>> NodeGroups;
    }
}