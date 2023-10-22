using System;
using System.Collections.Generic;
using System.Linq;
using SS.Data;
using UnityEditor;
using UnityEngine;

namespace SS.Utilities
{
    using Data.Save;
    using Elements;
    using ScriptableObjects;
    using Windows;
    
    public static class SSIOUtility
    {
        private static SSGraphView graphView;
        
        private static string graphFileName;
        private static string containerFolderPath;

        private static List<SSGroup> groups;
        private static List<SSNode> nodes;

        private static Dictionary<string, SSNodeGroupSO> createdNodeGroups;
        private static Dictionary<string, SSNodeSO> createdNodes;
        
        public static void Initialize(SSGraphView ssGraphView, string graphName)
        {
            graphView = ssGraphView;
            
            graphFileName = graphName;
            containerFolderPath = $"Assets/StorylineSystem/Storylines/{graphFileName}";

            groups = new List<SSGroup>();
            nodes = new List<SSNode>();

            createdNodeGroups = new Dictionary<string, SSNodeGroupSO>();
        }
        
        #region Save Methods
        
        public static void Save()
        {
            CreateStaticFolder();

            GetElementsFromGraphView();

            SSGraphSaveDataSO graphData = CreateAsset<SSGraphSaveDataSO>("Assets/Editor/StorylineSystem/Graphs", $"{graphFileName}Graph");
            
            graphData.Initialize(graphFileName);

            SSNodeContainerSO nodeContainer = CreateAsset<SSNodeContainerSO>(containerFolderPath, graphFileName);
            
            nodeContainer.Initialize(graphFileName);

            SaveGroups(graphData, nodeContainer);
            SaveNodes(graphData, nodeContainer);
            
            SaveAsset(graphData);
            SaveAsset(nodeContainer);
        }

        #region Groups

        private static void SaveGroups(SSGraphSaveDataSO graphData, SSNodeContainerSO nodeContainer)
        {
            List<string> groupNames = new List<string>();
            
            foreach (SSGroup group in groups)
            {
                SaveGroupToGraph(group, graphData);
                SaveGroupToScriptableObject(group, nodeContainer);
                
                groupNames.Add(group.title);
            }

            UpdateOldGroups(groupNames, graphData);
        }

        private static void SaveGroupToGraph(SSGroup group, SSGraphSaveDataSO graphData)
        {
            SSGroupSaveData groupData = new SSGroupSaveData()
            {
                ID = group.ID,
                Name = group.title,
                Position = group.GetPosition().position
                
            };
            
            graphData.Groups.Add(groupData);
        }

        private static void SaveGroupToScriptableObject(SSGroup group, SSNodeContainerSO nodeContainer)
        {
            string groupName = group.title; 
            
            CreateFolder($"{containerFolderPath}/Groups", groupName);
            CreateFolder($"{containerFolderPath}/Groups/{groupName}", "Storylines");

            SSNodeGroupSO nodeGroup = CreateAsset<SSNodeGroupSO>($"{containerFolderPath}/Groups/{groupName}", groupName);
            
            nodeGroup.Initialize(groupName);
            
            createdNodeGroups.Add(group.ID, nodeGroup);
            
            nodeContainer.NodeGroups.Add(nodeGroup, new List<SSNodeSO>());

            SaveAsset(nodeGroup);
        }
        
        private static void UpdateOldGroups(List<string> currentGroupNames, SSGraphSaveDataSO graphData)
        {
            if (graphData.OldGroupNames != null && graphData.OldGroupNames.Count != 0)
            {
                List<string> groupsToRemove = graphData.OldGroupNames.Except(currentGroupNames).ToList();

                foreach (string groupToRemove in groupsToRemove)
                {
                    RemoveFolder($"{containerFolderPath}/Groups/{groupToRemove}");
                }
            }

            graphData.OldGroupNames = new List<string>(currentGroupNames);
        }

        #endregion
        
        #region Nodes
        
        private static void SaveNodes(SSGraphSaveDataSO graphData, SSNodeContainerSO nodeContainer)
        {
            SerializableDictionary<string, List<string>> groupedNodeNames =
                new SerializableDictionary<string, List<string>>();
            List<string> ungroupedNodeNames = new List<string>();
            
            foreach (SSNode node in nodes)
            {
                SaveNodeToGraph(node, graphData);
                SaveNodeToScriptableObject(node, nodeContainer);

                if (node.Group != null)
                {
                    groupedNodeNames.AddItem(node.Group.title, node.NodeName);

                    continue;
                }
                
                ungroupedNodeNames.Add(node.NodeName);
            }

            UpdateNodeChoicesConnections();

            UpdateOldGroupedNodes(groupedNodeNames, graphData);
            UpdateOldUngroupedNodes(ungroupedNodeNames, graphData);
        }

        private static void SaveNodeToGraph(SSNode node, SSGraphSaveDataSO graphData)
        {
            List<SSChoiceSaveData> choices = new List<SSChoiceSaveData>();

            foreach (SSChoiceSaveData choice in node.Choices)
            {
                SSChoiceSaveData choiceData = new SSChoiceSaveData()
                {
                    Text = choice.Text,
                    NodeID = choice.NodeID
                };
                
                choices.Add(choiceData);
            }
            
            SSNodeSaveData nodeData = new SSNodeSaveData()
            {
                ID = node.ID,
                Name = node.NodeName,
                Choices = choices,
                Text = node.Text,
                GroupID = node.Group?.ID,
                NodeType = node.NodeType,
                Position = node.GetPosition().position
            };
            
            graphData.Nodes.Add(nodeData);
        }

        private static void SaveNodeToScriptableObject(SSNode node, SSNodeContainerSO nodeContainer)
        {
            SSNodeSO nodeSO;

            if (node.Group != null)
            {
                nodeSO = CreateAsset<SSNodeSO>($"{containerFolderPath}/Groups/{node.Group.title}/Nodes", node.NodeName);
                
                nodeContainer.NodeGroups.AddItem(createdNodeGroups[node.Group.ID], nodeSO);
            }
            else
            {
                nodeSO = CreateAsset<SSNodeSO>($"{containerFolderPath}/Global/Nodes", node.NodeName);
                
                nodeContainer.UngroupedNodes.Add(nodeSO);
            }
            
            nodeSO.Initialize(node.NodeName, node.Text, ConvertNodeChoicesToNodeChoices(node.Choices), node.NodeType, node.IsStartingNode());
            
            createdNodes.Add(node.ID, nodeSO);
            
            SaveAsset(nodeSO);
        }

        private static List<SSNodeChoiceData> ConvertNodeChoicesToNodeChoices(List<SSChoiceSaveData> nodeChoices)
        {
            List<SSNodeChoiceData> nodeChoicesData = new List<SSNodeChoiceData>();

            foreach (SSChoiceSaveData nodeChoice in nodeChoices)
            {
                SSNodeChoiceData choiceData = new SSNodeChoiceData()
                {
                    Text = nodeChoice.Text
                };
                
                nodeChoicesData.Add(choiceData);
            }

            return nodeChoicesData;
        }
        
        private static void UpdateNodeChoicesConnections()
        {
            foreach (SSNode node in nodes)
            {
                SSNodeSO nodeSO = createdNodes[node.ID];

                for (int choiceIndex = 0; choiceIndex < node.Choices.Count; ++choiceIndex)
                {
                    SSChoiceSaveData nodeChoice = node.Choices[choiceIndex];

                    if (string.IsNullOrEmpty(nodeChoice.NodeID))
                    {
                        continue;
                    }

                    nodeSO.Choices[choiceIndex].NextNode = createdNodes[nodeChoice.NodeID];
                    
                    SaveAsset(nodeSO);
                }
            }
        }
        
        private static void UpdateOldGroupedNodes(SerializableDictionary<string, List<string>> currentGroupedNodeNames, SSGraphSaveDataSO graphData)
        {
            if (graphData.OldGroupedNodeNames != null && graphData.OldGroupedNodeNames.Count != 0)
            {
                foreach (KeyValuePair<string, List<string>> oldGroupedNode in graphData.OldGroupedNodeNames)
                {
                    List<string> nodesToRemove = new List<string>();

                    if (currentGroupedNodeNames.ContainsKey(oldGroupedNode.Key))
                    {
                        nodesToRemove = oldGroupedNode.Value.Except(currentGroupedNodeNames[oldGroupedNode.Key])
                            .ToList();
                    }

                    foreach (string nodeToRemove in nodesToRemove)
                    {
                        RemoveAsset($"{containerFolderPath}/Groups/{oldGroupedNode.Key}/Nodes", nodeToRemove);
                    }
                }
            }

            graphData.OldGroupedNodeNames = new SerializableDictionary<string, List<string>>(currentGroupedNodeNames);
        }
        
        private static void UpdateOldUngroupedNodes(List<string> currentUngroupedNodeNames, SSGraphSaveDataSO graphData)
        {
            if (graphData.OldUngroupedNodeNames != null && graphData.OldUngroupedNodeNames.Count != 0)
            {
                List<string> nodesToRemove = graphData.OldUngroupedNodeNames.Except(currentUngroupedNodeNames).ToList();

                foreach (string nodeToRemove in nodesToRemove)
                {
                    RemoveAsset($"{containerFolderPath}/Global/Nodes", nodeToRemove);
                }
            }

            graphData.OldUngroupedNodeNames = new List<string>(currentUngroupedNodeNames);
        }

        #endregion

        #endregion

        #region Creation Methods
        
        private static void CreateStaticFolder()
        {
            CreateFolder("Assets/Editor/StorylineSystem", "Graphs");
            
            CreateFolder("Assets", "StorylineSystem");
            CreateFolder("Assets/StorylineSystem", "Storylines");
            
            CreateFolder("Assets/StorylineSystem/Storylines", graphFileName);
            CreateFolder(containerFolderPath, "Global");
            CreateFolder(containerFolderPath, "Groups");
            CreateFolder($"{containerFolderPath}/Global", "Storylines");
        }
        
        #endregion
        
        #region Fetch Methods
        
        private static void GetElementsFromGraphView()
        {
            Type groupType = typeof(SSGroup);
            
            graphView.graphElements.ForEach(graphElement =>
            {
                if (graphElement is SSNode node)
                {
                    nodes.Add(node);

                    return;
                }

                if (graphElement.GetType() == groupType)
                {
                    SSGroup group = (SSGroup)graphElement;
                    
                    groups.Add(group);

                    return;
                }
            });
        }
        
        #endregion
        
        #region Utility Methods

        private static void CreateFolder(string path, string folderName)
        {
            if (AssetDatabase.IsValidFolder($"{path}/{folderName}"))
            {
                return;
            }

            AssetDatabase.CreateFolder(path, folderName);
        }
        
        private static void RemoveFolder(string fullPath)
        {
            FileUtil.DeleteFileOrDirectory($"{fullPath}.meta");
            FileUtil.DeleteFileOrDirectory($"{fullPath}/");
        }
        
        private static T CreateAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";

            T asset = AssetDatabase.LoadAssetAtPath<T>(fullPath);

            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();

                AssetDatabase.CreateAsset(asset, fullPath);
            }
            
            return asset;
        }
        
        private static void RemoveAsset(string path, string assetName)
        {
            AssetDatabase.DeleteAsset($"{path}/{assetName}.asset");
        }
        
        private static void SaveAsset(UnityEngine.Object asset)
        {
            EditorUtility.SetDirty(asset);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        #endregion
    }
}