using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace SS.Utilities
{
    using Data;
    using Data.Save;
    using Elements;
    using Enumerations;
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

        private static Dictionary<string, SSGroup> loadedGroups;
        private static Dictionary<string, SSNode> loadedNodes;

        public static void Initialize(SSGraphView ssGraphView, string graphName)
        {
            graphView = ssGraphView;

            graphFileName = graphName;
            containerFolderPath = $"Assets/StorylineSystem/Storylines/{graphFileName}";

            groups = new List<SSGroup>();
            nodes = new List<SSNode>();

            createdNodeGroups = new Dictionary<string, SSNodeGroupSO>();
            createdNodes = new Dictionary<string, SSNodeSO>();

            loadedGroups = new Dictionary<string, SSGroup>();
            loadedNodes = new Dictionary<string, SSNode>();
        }

        #region Save Methods

        public static void Save()
        {
            CreateStaticFolder();

            GetElementsFromGraphView();

            SSGraphSaveDataSO graphData =
                CreateAsset<SSGraphSaveDataSO>("Assets/Editor/StorylineSystem/Graphs", $"{graphFileName}Graph");

            graphData.Initialize(graphFileName, graphView.StoryStatus, graphView.StoryType, graphView.IsFirstToPlay, graphView.Condition);

            SSNodeContainerSO nodeContainer = CreateAsset<SSNodeContainerSO>(containerFolderPath, graphFileName);

            nodeContainer.Initialize(graphFileName, graphView.StoryStatus, graphView.StoryType, graphView.IsFirstToPlay, graphView.Condition);

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
                StoryStatus = group.StoryStatus,
                IsFirstToPlay = group.IsFirstToPlay,
                Condition = group.Condition,
                Position = group.GetPosition().position
            };

            graphData.Groups.Add(groupData);
        }

        private static void SaveGroupToScriptableObject(SSGroup group, SSNodeContainerSO nodeContainer)
        {
            string groupName = group.title;

            CreateFolder($"{containerFolderPath}/Groups", groupName);
            CreateFolder($"{containerFolderPath}/Groups/{groupName}", "Nodes");

            SSNodeGroupSO nodeGroup =
                CreateAsset<SSNodeGroupSO>($"{containerFolderPath}/Groups/{groupName}", groupName);

            nodeGroup.Initialize(groupName, group.StoryStatus, group.IsFirstToPlay, group.Condition);

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
            List<SSChoiceSaveData> choices = CloneNodeChoices(node.Choices);

            if (node is SSDialogueNode dialogueNode)
            {
                SSDialogueNodeSaveData nodeData = new SSDialogueNodeSaveData()
                {
                    ID = dialogueNode.ID,
                    Name = dialogueNode.NodeName,
                    Choices = choices,
                    GroupID = dialogueNode.Group?.ID,
                    NodeType = dialogueNode.NodeType,
                    Position = dialogueNode.GetPosition().position,
                    Text = dialogueNode.Text,
                    SpeakerType = dialogueNode.SpeakerType,
                    Duration = dialogueNode.Duration,
                    IsDialogueTask = dialogueNode.IsDialogueTask,
                    PercentageTask = dialogueNode.PercentageTask
                };

                graphData.Nodes.Add(nodeData);
            }
            else if (node is SSTaskNode taskNode)
            {
                SSTaskNodeSaveData nodeData = new SSTaskNodeSaveData()
                {
                    ID = taskNode.ID,
                    Name = taskNode.NodeName,
                    Choices = choices,
                    GroupID = taskNode.Group?.ID,
                    NodeType = taskNode.NodeType,
                    Position = taskNode.GetPosition().position,
                    DescriptionTask = taskNode.DescriptionTask,
                    TaskStatus = taskNode.TaskStatus,
                    TaskType = taskNode.TaskType,
                    TaskIcon = taskNode.TaskIcon,
                    TimeLeft = taskNode.TimeLeft,
                    BaseDuration = taskNode.BaseDuration,
                    MandatorySlots = taskNode.MandatorySlots,
                    OptionalSlots = taskNode.OptionalSlots,
                    TaskHelpFactor = taskNode.TaskHelpFactor,
                    Room = taskNode.Room
                };

                graphData.Nodes.Add(nodeData);
            }
            else if (node is SSTimeNode timeNode)
            {
                SSTimeNodeSaveData nodeData = new SSTimeNodeSaveData()
                {
                    ID = timeNode.ID,
                    Name = timeNode.NodeName,
                    Choices = choices,
                    GroupID = timeNode.Group?.ID,
                    NodeType = timeNode.NodeType,
                    Position = timeNode.GetPosition().position,
                    TimeToWait = timeNode.TimeToWait
                };

                graphData.Nodes.Add(nodeData);
            }
        }

        private static void SaveNodeToScriptableObject(SSNode node, SSNodeContainerSO nodeContainer)
        {
            if (node is SSDialogueNode dialogueNode)
            {
                SSDialogueNodeSO nodeSO;

                if (dialogueNode.Group != null)
                {
                    nodeSO = CreateAsset<SSDialogueNodeSO>(
                        $"{containerFolderPath}/Groups/{dialogueNode.Group.title}/Nodes", dialogueNode.NodeName);

                    nodeContainer.NodeGroups.AddItem(createdNodeGroups[dialogueNode.Group.ID], nodeSO);
                }
                else
                {
                    nodeSO = CreateAsset<SSDialogueNodeSO>($"{containerFolderPath}/Global/Nodes",
                        dialogueNode.NodeName);

                    nodeContainer.UngroupedNodes.Add(nodeSO);
                }

                nodeSO.Initialize(dialogueNode.NodeName, dialogueNode.Text,
                    ConvertNodeChoicesToNodeChoicesData(dialogueNode.Choices), dialogueNode.NodeType,
                    dialogueNode.IsStartingNode(), dialogueNode.SpeakerType, dialogueNode.Duration,
                    dialogueNode.IsDialogueTask, dialogueNode.PercentageTask);

                createdNodes.Add(dialogueNode.ID, nodeSO);

                SaveAsset(nodeSO);
            }
            else if (node is SSTaskNode taskNode)
            {
                SSTaskNodeSO nodeSO;

                if (taskNode.Group != null)
                {
                    nodeSO = CreateAsset<SSTaskNodeSO>($"{containerFolderPath}/Groups/{taskNode.Group.title}/Nodes",
                        taskNode.NodeName);

                    nodeContainer.NodeGroups.AddItem(createdNodeGroups[taskNode.Group.ID], nodeSO);
                }
                else
                {
                    nodeSO = CreateAsset<SSTaskNodeSO>($"{containerFolderPath}/Global/Nodes", taskNode.NodeName);

                    nodeContainer.UngroupedNodes.Add(nodeSO);
                }

                nodeSO.Initialize(taskNode.NodeName, ConvertNodeChoicesToNodeChoicesData(taskNode.Choices),
                    taskNode.NodeType,
                    taskNode.IsStartingNode(), taskNode.DescriptionTask, taskNode.TaskStatus, taskNode.TaskType, taskNode.TaskIcon, taskNode.TimeLeft,
                    taskNode.BaseDuration, taskNode.MandatorySlots, taskNode.OptionalSlots, taskNode.TaskHelpFactor,
                    taskNode.Room);

                createdNodes.Add(taskNode.ID, nodeSO);

                SaveAsset(nodeSO);
            }
            else if (node is SSTimeNode timeNode)
            {
                SSTimeNodeSO nodeSO;

                if (timeNode.Group != null)
                {
                    nodeSO = CreateAsset<SSTimeNodeSO>($"{containerFolderPath}/Groups/{timeNode.Group.title}/Nodes",
                        timeNode.NodeName);

                    nodeContainer.NodeGroups.AddItem(createdNodeGroups[timeNode.Group.ID], nodeSO);
                }
                else
                {
                    nodeSO = CreateAsset<SSTimeNodeSO>($"{containerFolderPath}/Global/Nodes", timeNode.NodeName);

                    nodeContainer.UngroupedNodes.Add(nodeSO);
                }

                nodeSO.Initialize(timeNode.NodeName, ConvertNodeChoicesToNodeChoicesData(timeNode.Choices),
                    timeNode.NodeType,
                    timeNode.IsStartingNode(), timeNode.TimeToWait);

                createdNodes.Add(timeNode.ID, nodeSO);

                SaveAsset(nodeSO);
            }
        }

        private static List<SSNodeChoiceData> ConvertNodeChoicesToNodeChoicesData(List<SSChoiceSaveData> nodeChoices)
        {
            List<SSNodeChoiceData> nodeChoicesData = new List<SSNodeChoiceData>();

            foreach (SSChoiceSaveData nodeChoice in nodeChoices)
            {
                SSNodeChoiceData choiceData;

                if (nodeChoice is SSChoiceTaskSaveData choiceTask)
                {
                    choiceData = new SSNodeChoiceTaskData()
                    {
                        Text = choiceTask.Text,
                        Condition = choiceTask.Condition,
                        PreviewOutcome = choiceTask.PreviewOutcome
                    };

                    nodeChoicesData.Add(choiceData);

                    continue;
                }

                choiceData = new SSNodeChoiceData()
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

                    if (string.IsNullOrEmpty(nodeChoice.NextNodeID))
                    {
                        continue;
                    }

                    nodeSO.Choices[choiceIndex].NextNode = createdNodes[nodeChoice.NextNodeID];

                    SaveAsset(nodeSO);
                }
            }
        }

        private static void UpdateOldGroupedNodes(SerializableDictionary<string, List<string>> currentGroupedNodeNames,
            SSGraphSaveDataSO graphData)
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

        #region Load Methods

        public static void Load()
        {
            SSGraphSaveDataSO graphData =
                LoadAsset<SSGraphSaveDataSO>("Assets/Editor/StorylineSystem/Graphs", graphFileName);

            if (graphData == null)
            {
                EditorUtility.DisplayDialog("Couldn't load the file!",
                    "The file at the following path could not be found:\n\n" +
                    $"Assets/Editor/StorylineSystem/Graphs/{graphFileName}\n\n" +
                    "Make sure you chose the right file and it's placed at the folder path mentioned above.",
                    "Thanks!");

                return;
            }

            SSEditorWindow.UpdateFileName(graphData.FileName);

            LoadGroups(graphData.Groups);
            LoadNodes(graphData.Nodes);
            LoadNodesConnections();
            
            graphView.IsFirstToPlay = graphData.IsFirstToPlay;
            graphView.StoryStatus = graphData.StoryStatus;
            graphView.StoryType = graphData.StoryType;
            graphView.Condition = graphData.Condition;
        }

        private static void LoadGroups(List<SSGroupSaveData> groups)
        {
            foreach (SSGroupSaveData groupData in groups)
            {
                SSGroup group = graphView.CreateGroup(groupData.Name, groupData.Position);

                group.ID = groupData.ID;
                group.StoryStatus = groupData.StoryStatus;
                group.IsFirstToPlay = groupData.IsFirstToPlay;
                group.Condition = groupData.Condition;

                loadedGroups.Add(group.ID, group);
            }
        }

        private static void LoadNodes(List<SSNodeSaveData> nodes)
        {
            foreach (SSNodeSaveData nodeData in nodes)
            {
                List<SSChoiceSaveData> choices = CloneNodeChoices(nodeData.Choices);
                SSNode node = graphView.CreateNode(nodeData.Name, nodeData.NodeType, nodeData.Position, false);

                node.ID = nodeData.ID;
                node.Choices = choices;

                if (nodeData.NodeType == SSNodeType.Dialogue)
                {
                    ((SSDialogueNode)node).Text = ((SSDialogueNodeSaveData)nodeData).Text;
                    ((SSDialogueNode)node).SpeakerType = ((SSDialogueNodeSaveData)nodeData).SpeakerType;
                    ((SSDialogueNode)node).Duration = ((SSDialogueNodeSaveData)nodeData).Duration;
                    ((SSDialogueNode)node).IsDialogueTask = ((SSDialogueNodeSaveData)nodeData).IsDialogueTask;
                    ((SSDialogueNode)node).PercentageTask = ((SSDialogueNodeSaveData)nodeData).PercentageTask;
                }
                else if (nodeData.NodeType == SSNodeType.Task)
                {
                    ((SSTaskNode)node).DescriptionTask = ((SSTaskNodeSaveData)nodeData).DescriptionTask;
                    ((SSTaskNode)node).TaskStatus = ((SSTaskNodeSaveData)nodeData).TaskStatus;
                    ((SSTaskNode)node).TaskType = ((SSTaskNodeSaveData)nodeData).TaskType;
                    ((SSTaskNode)node).TaskIcon = ((SSTaskNodeSaveData)nodeData).TaskIcon;
                    ((SSTaskNode)node).TimeLeft = ((SSTaskNodeSaveData)nodeData).TimeLeft;
                    ((SSTaskNode)node).BaseDuration = ((SSTaskNodeSaveData)nodeData).BaseDuration;
                    ((SSTaskNode)node).MandatorySlots = ((SSTaskNodeSaveData)nodeData).MandatorySlots;
                    ((SSTaskNode)node).OptionalSlots = ((SSTaskNodeSaveData)nodeData).OptionalSlots;
                    ((SSTaskNode)node).TaskHelpFactor = ((SSTaskNodeSaveData)nodeData).TaskHelpFactor;
                    ((SSTaskNode)node).Room = ((SSTaskNodeSaveData)nodeData).Room;
                }
                else if (nodeData.NodeType == SSNodeType.Time)
                {
                    ((SSTimeNode)node).TimeToWait = ((SSTimeNodeSaveData)nodeData).TimeToWait;
                }

                node.Draw();

                graphView.AddElement(node);

                loadedNodes.Add(node.ID, node);

                if (string.IsNullOrEmpty(nodeData.GroupID))
                {
                    continue;
                }

                SSGroup group = loadedGroups[nodeData.GroupID];

                node.Group = group;

                group.AddElement(node);
            }
        }

        private static void LoadNodesConnections()
        {
            foreach (KeyValuePair<string, SSNode> loadedNode in loadedNodes)
            {
                foreach (Port choicePort in loadedNode.Value.outputContainer.Children())
                {
                    SSChoiceSaveData choiceData = (SSChoiceSaveData)choicePort.userData;

                    if (string.IsNullOrEmpty(choiceData.NextNodeID))
                    {
                        continue;
                    }

                    SSNode nextNode = loadedNodes[choiceData.NextNodeID];

                    Port nextNodeInputPort = (Port)nextNode.inputContainer.Children().First();

                    Edge edge = choicePort.ConnectTo(nextNodeInputPort);

                    graphView.Add(edge);

                    loadedNode.Value.RefreshPorts();
                }
            }
        }

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
            CreateFolder($"{containerFolderPath}/Global", "Nodes");
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

        public static void CreateFolder(string path, string folderName)
        {
            if (AssetDatabase.IsValidFolder($"{path}/{folderName}"))
            {
                return;
            }

            AssetDatabase.CreateFolder(path, folderName);
        }

        public static void RemoveFolder(string fullPath)
        {
            FileUtil.DeleteFileOrDirectory($"{fullPath}.meta");
            FileUtil.DeleteFileOrDirectory($"{fullPath}/");
        }

        public static T CreateAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";

            T asset = LoadAsset<T>(path, assetName);

            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();

                AssetDatabase.CreateAsset(asset, fullPath);
            }

            return asset;
        }

        public static T LoadAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";

            return AssetDatabase.LoadAssetAtPath<T>(fullPath);
        }

        private static void RemoveAsset(string path, string assetName)
        {
            AssetDatabase.DeleteAsset($"{path}/{assetName}.asset");
        }

        public static void SaveAsset(UnityEngine.Object asset)
        {
            EditorUtility.SetDirty(asset);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static List<SSChoiceSaveData> CloneNodeChoices(List<SSChoiceSaveData> nodeChoices)
        {
            List<SSChoiceSaveData> choices = new List<SSChoiceSaveData>();

            foreach (SSChoiceSaveData choice in nodeChoices)
            {
                SSChoiceSaveData choiceData;

                if (choice is SSChoiceTaskSaveData choiceTask)
                {
                    choiceData = new SSChoiceTaskSaveData()
                    {
                        Text = choiceTask.Text,
                        NextNodeID = choiceTask.NextNodeID,
                        Condition = choiceTask.Condition,
                        PreviewOutcome = choiceTask.PreviewOutcome
                    };
                }
                else
                {
                    choiceData = new SSChoiceSaveData()
                    {
                        Text = choice.Text,
                        NextNodeID = choice.NextNodeID
                    };
                }

                choices.Add(choiceData);
            }

            return choices;
        }

        #endregion
    }
}