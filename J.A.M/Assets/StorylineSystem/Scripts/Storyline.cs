using System.Collections.Generic;

namespace SS
{
    using Enumerations;
    using ScriptableObjects;

    [System.Serializable]
    public class Storyline
    {
        public SSNodeContainerSO NodeContainer;
        public SSStatus Status;
        public List<SerializableTuple<SSStatus, SSNodeGroupSO>> NodeGroups;
    }
}