using System.Collections.Generic;
using UnityEngine;

namespace SS.ScriptableObjects
{
    using Enumerations;

    public class SSNodeContainerSO : ScriptableObject
    {
        [field: SerializeField] public string FileName { get; set; }
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public SSStoryStatus StoryStatus { get; set; }
        [field: SerializeField] public SSStoryType StoryType { get; set; }
        [field: SerializeField] public bool IsFirstToPlay { get; set; }
        [field: SerializeField] public bool IsReplayable { get; set; }
        [field: SerializeField] public ConditionSO Condition { get; set; }
        [field: SerializeField] public SerializableDictionary<SSNodeGroupSO, List<SSNodeSO>> NodeGroups { get; set; }
        [field: SerializeField] public List<SSNodeSO> UngroupedNodes { get; set; }

        public void Initialize(string fileName, string id, SSStoryStatus storyStatus, SSStoryType storyType, bool isFirstToPlay, bool isReplayable, ConditionSO condition)
        {
            FileName = fileName;
            ID = id;
            StoryStatus = storyStatus;
            StoryType = storyType;
            IsFirstToPlay = isFirstToPlay;
            IsReplayable = isReplayable;
            Condition = condition;
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