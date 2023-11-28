using System.Collections.Generic;
using UnityEngine.Serialization;

namespace SS
{
    using Enumerations;
    using ScriptableObjects;

    [System.Serializable]
    public class Storyline
    {
        public SSNodeContainerSO NodeContainer;
        [FormerlySerializedAs("Status")] public SSStoryStatus storyStatus;
        public List<SerializableTuple<SSStoryStatus, SSNodeGroupSO>> NodeGroups;

        public Storyline(SSNodeContainerSO nodeContainer, SSStoryStatus storyStatus,
            List<SerializableTuple<SSStoryStatus, SSNodeGroupSO>> nodeGroups)
        {
            NodeContainer = nodeContainer;
            this.storyStatus = storyStatus;
            NodeGroups = nodeGroups;
        }
    }
}