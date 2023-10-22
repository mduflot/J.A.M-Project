using System.Collections.Generic;
using UnityEngine;

namespace SS.ScriptableObjects
{
    public class SSNodeContainerSO : ScriptableObject
    {
        [field: SerializeField] public string FileName { get; set; }
        [field: SerializeField] public SerializableDictionary<SSNodeGroupSO, List<SSNodeSO>> NodeGroups { get; set; }
        [field: SerializeField] public List<SSNodeSO> UngroupedNodes { get; set; }

        public void Initialize(string fileName)
        {
            FileName = fileName;

            NodeGroups = new SerializableDictionary<SSNodeGroupSO, List<SSNodeSO>>();
            UngroupedNodes = new List<SSNodeSO>();
        }
    }
}