using System.Collections.Generic;
using UnityEngine;

namespace SS.ScriptableObjects
{
    using Enumerations;

    public class SSNodeContainerSO : ScriptableObject
    {
        [field: SerializeField] public string FileName { get; set; }
        [field: SerializeField] public SSStatus Status { get; set; }
        [field: SerializeField] public SerializableDictionary<SSNodeGroupSO, List<SSNodeSO>> NodeGroups { get; set; }
        [field: SerializeField] public List<SSNodeSO> UngroupedNodes { get; set; }

        public void Initialize(string fileName, SSStatus status)
        {
            FileName = fileName;
            Status = status;
            NodeGroups = new SerializableDictionary<SSNodeGroupSO, List<SSNodeSO>>();
            UngroupedNodes = new List<SSNodeSO>();
        }

        public List<string> GetNodeGroupNames()
        {
            List<string> nodeGroupNames = new List<string>();

            foreach (SSNodeGroupSO nodeGroup in NodeGroups.Keys)
            {
                nodeGroupNames.Add(nodeGroup.GroupName);
            }

            return nodeGroupNames;
        }

        public List<string> GetGroupedNodeNames(SSNodeGroupSO nodeGroup, bool startingNodesOnly)
        {
            List<SSNodeSO> groupedNodes = NodeGroups[nodeGroup];

            List<string> groupedNodeNames = new List<string>();

            foreach (SSNodeSO groupedNode in groupedNodes)
            {
                if (startingNodesOnly && !groupedNode.IsStartingNode)
                {
                    continue;
                }

                groupedNodeNames.Add(groupedNode.NodeName);
            }

            return groupedNodeNames;
        }

        public List<string> GetUngroupedNodeNames(bool startingNodesOnly)
        {
            List<string> ungroupedNodeNames = new List<string>();

            foreach (SSNodeSO ungroupedNode in UngroupedNodes)
            {
                if (startingNodesOnly && !ungroupedNode.IsStartingNode)
                {
                    continue;
                }

                ungroupedNodeNames.Add(ungroupedNode.NodeName);
            }

            return ungroupedNodeNames;
        }
    }
}