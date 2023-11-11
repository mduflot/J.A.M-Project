using System.Collections.Generic;
using UnityEditor;

namespace SS.Inspectors
{
    using Utilities;
    using ScriptableObjects;

    [CustomEditor(typeof(SSNode))]
    public class SSInspector : Editor
    {
        /* UI GameObjects */
        private SerializedProperty currentStorylineProperty;
        private SerializedProperty spaceshipManagerProperty;

        /* Node Scriptable Objects */
        private SerializedProperty nodeContainerProperty;
        private SerializedProperty nodeGroupProperty;
        private SerializedProperty nodeProperty;

        /* Filters */
        private SerializedProperty groupedNodesProperty;
        private SerializedProperty startingNodesOnlyProperty;

        /* Indexes */
        private SerializedProperty selectedNodeGroupIndexProperty;
        private SerializedProperty selectedNodeIndexProperty;

        private void OnEnable()
        {
            currentStorylineProperty = serializedObject.FindProperty("currentStoryline");
            spaceshipManagerProperty = serializedObject.FindProperty("spaceshipManager");
            
            nodeContainerProperty = serializedObject.FindProperty("nodeContainer");
            nodeGroupProperty = serializedObject.FindProperty("nodeGroup");
            nodeProperty = serializedObject.FindProperty("node");

            groupedNodesProperty = serializedObject.FindProperty("groupedNodes");
            startingNodesOnlyProperty = serializedObject.FindProperty("startingNodesOnly");

            selectedNodeGroupIndexProperty = serializedObject.FindProperty("selectedNodeGroupIndex");
            selectedNodeIndexProperty = serializedObject.FindProperty("selectedNodeIndex");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            currentStorylineProperty.DrawPropertyField();
            spaceshipManagerProperty.DrawPropertyField();

            DrawNodeContainerArea();

            SSNodeContainerSO nodeContainer = (SSNodeContainerSO) nodeContainerProperty.objectReferenceValue;

            if (nodeContainer == null)
            { 
                StopDrawing("Select a Node Container to see the rest of the Inspector");

                return;
            }

            DrawFiltersArea();

            bool currentStartingNodesOnlyFilter = startingNodesOnlyProperty.boolValue;

            List<string> nodeNames;

            string nodeFolderPath = $"Assets/StorylineSystem/Storylines/{nodeContainer.FileName}";

            string nodeInfoMessage;

            if (groupedNodesProperty.boolValue)
            {
                List<string> nodeGroupNames = nodeContainer.GetNodeGroupNames();

                if (nodeGroupNames.Count == 0)
                {
                    StopDrawing("There are no Node Groups in this Node Container");

                    return;
                }

                DrawNodeGroupArea(nodeContainer, nodeGroupNames);

                SSNodeGroupSO nodeGroup = (SSNodeGroupSO) nodeGroupProperty.objectReferenceValue;

                nodeNames = nodeContainer.GetGroupedNodeNames(nodeGroup, currentStartingNodesOnlyFilter);

                nodeFolderPath += $"/Groups/{nodeGroup.GroupName}/Nodes";

                nodeInfoMessage = "There are no" + (currentStartingNodesOnlyFilter ? " Starting" : "" ) + " Nodes in this Node Group.";
            }
            else
            {
                nodeNames = nodeContainer.GetUngroupedNodeNames(currentStartingNodesOnlyFilter);

                nodeFolderPath += "/Global/Nodes";

                nodeInfoMessage = "There are no" + (currentStartingNodesOnlyFilter ? " Starting" : "" ) + " Ungrouped Nodes in this Node Container.";
            }

            if (nodeNames.Count == 0)
            {
                StopDrawing(nodeInfoMessage);

                return;
            }

            DrawNodeArea(nodeNames, nodeFolderPath);

            serializedObject.ApplyModifiedProperties();
        }

        #region Draw Methods

        private void DrawNodeContainerArea()
        {
            InspectorUtility.DrawHeader("Node Container");

            nodeContainerProperty.DrawPropertyField();

            InspectorUtility.DrawSpace();
        }

        private void DrawFiltersArea()
        {
            InspectorUtility.DrawHeader("Filters");

            groupedNodesProperty.DrawPropertyField();
            startingNodesOnlyProperty.DrawPropertyField();

            InspectorUtility.DrawSpace();
        }

        private void DrawNodeGroupArea(SSNodeContainerSO nodeContainer, List<string> nodeGroupNames)
        {
            InspectorUtility.DrawHeader("Node Group");

            int oldSelectedNodeGroupIndex = selectedNodeGroupIndexProperty.intValue;

            SSNodeGroupSO oldNodeGroup = (SSNodeGroupSO) nodeGroupProperty.objectReferenceValue;

            bool isOldNodeGroupNull = oldNodeGroup == null;

            string oldNodeGroupName = isOldNodeGroupNull ? "" : oldNodeGroup.GroupName;

            UpdateIndexOnNamesListUpdate(nodeGroupNames, selectedNodeGroupIndexProperty, oldSelectedNodeGroupIndex, oldNodeGroupName, isOldNodeGroupNull);

            selectedNodeGroupIndexProperty.intValue = InspectorUtility.DrawPopup("Node Group", selectedNodeGroupIndexProperty, nodeGroupNames.ToArray());

            string selectedNodeGroupName = nodeGroupNames[selectedNodeGroupIndexProperty.intValue];

            SSNodeGroupSO selectedNodeGroup = SSIOUtility.LoadAsset<SSNodeGroupSO>(
                $"Assets/StorylineSystem/Storylines/{nodeContainer.FileName}/Groups/{selectedNodeGroupName}", selectedNodeGroupName);

            nodeGroupProperty.objectReferenceValue = selectedNodeGroup;

            InspectorUtility.DrawDisabledFields(() => nodeGroupProperty.DrawPropertyField());

            InspectorUtility.DrawSpace();
        }

        private void DrawNodeArea(List<string> nodeNames, string nodeFolderPath)
        {
            InspectorUtility.DrawHeader("Node");

            int oldSelectedNodeIndex = selectedNodeIndexProperty.intValue;

            SSNodeSO oldNode = (SSNodeSO) nodeProperty.objectReferenceValue;

            bool isOldNodeNull = oldNode == null;

            string oldNodeName = isOldNodeNull ? "" : oldNode.NodeName;

            UpdateIndexOnNamesListUpdate(nodeNames, selectedNodeIndexProperty, oldSelectedNodeIndex, oldNodeName, isOldNodeNull);

            selectedNodeIndexProperty.intValue = InspectorUtility.DrawPopup("Node", selectedNodeIndexProperty, nodeNames.ToArray());

            string selectedNodeName = nodeNames[selectedNodeIndexProperty.intValue];

            SSNodeSO selectedNode = SSIOUtility.LoadAsset<SSNodeSO>(nodeFolderPath, selectedNodeName);

            nodeProperty.objectReferenceValue = selectedNode;

            InspectorUtility.DrawDisabledFields(() => nodeProperty.DrawPropertyField());
        }

        private void StopDrawing(string reason, MessageType messageType = MessageType.Info)
        {
            InspectorUtility.DrawHelpBox(reason, messageType);

            InspectorUtility.DrawSpace();

            InspectorUtility.DrawHelpBox("You need to select a Node for this component to work properly at Runtime!", MessageType.Warning);

            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Index Methods

        private void UpdateIndexOnNamesListUpdate(List<string> optionNames, SerializedProperty indexProperty,int oldSelectedPropertyIndex, string oldPropertyName, bool isOldPropertyNull)
        {
            if (isOldPropertyNull)
            {
                indexProperty.intValue = 0;

                return;
            }

            bool oldIndexIsOutOfBoundsOfNamesListCount = oldSelectedPropertyIndex > optionNames.Count - 1;
            bool oldNameIsDifferentThanSelectedName = oldIndexIsOutOfBoundsOfNamesListCount ||
                                                      oldPropertyName != optionNames[oldSelectedPropertyIndex];
            
            if (oldNameIsDifferentThanSelectedName)
            {
                if (optionNames.Contains(oldPropertyName))
                {
                    indexProperty.intValue = optionNames.IndexOf(oldPropertyName);
                }
                else
                {
                    indexProperty.intValue = 0;
                }
            }
            
        }

        #endregion
    }
}