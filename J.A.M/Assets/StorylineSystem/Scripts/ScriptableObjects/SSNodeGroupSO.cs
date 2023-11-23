using UnityEngine;

namespace SS.ScriptableObjects
{
    using Enumerations;

    public class SSNodeGroupSO : ScriptableObject
    {
        [field: SerializeField] public string GroupName { get; set; }
        [field: SerializeField] public SSStatus Status { get; set; }

        public void Initialize(string groupName, SSStatus status)
        {
            GroupName = groupName;
            Status = status;
        }
    }
}