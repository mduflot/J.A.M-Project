using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace SS.Windows
{
    using Data.Error;
    using Data.Save;
    using Elements;
    using Enumerations;

    public class SSGraphView : GraphView
    {
        public SSStatus Status;
        public SerializableDictionary<string, SSGroupErrorData> Groups;

        private SSEditorWindow editorWindow;
        private SSSearchWindow searchWindow;

        private MiniMap miniMap;

        private SerializableDictionary<string, SSNodeErrorData> ungroupedNodes;
        private SerializableDictionary<Group, SerializableDictionary<string, SSNodeErrorData>> groupedNodes;

        private int nameErrorsAmount;

        public int NameErrorsAmount
        {
            get => nameErrorsAmount;

            set
            {
                nameErrorsAmount = value;

                if (nameErrorsAmount == 0)
                {
                    editorWindow.EnableSaving();
                }

                if (nameErrorsAmount == 1)
                {
                    editorWindow.DisableSaving();
                }
            }
        }

        public SSGraphView(SSEditorWindow ssEditorWindow)
        {
            editorWindow = ssEditorWindow;

            ungroupedNodes = new SerializableDictionary<string, SSNodeErrorData>();
            Groups = new SerializableDictionary<string, SSGroupErrorData>();
            groupedNodes = new SerializableDictionary<Group, SerializableDictionary<string, SSNodeErrorData>>();

            AddManipulators();
            AddSearchWindow();
            AddMinimap();
            AddGridBackground();

            OnElementsDeleted();
            OnGroupElementsAdded();
            OnGroupElementsRemoved();
            OnGroupRenamed();
            OnGraphViewChanged();

            AddStyles();
            AddMiniMapStyles();
        }

        #region Overrided Methods

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort == port)
                {
                    return;
                }

                if (startPort.node == port.node)
                {
                    return;
                }

                if (startPort.direction == port.direction)
                {
                    return;
                }

                compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        #endregion

        #region Manipulators

        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            this.AddManipulator(CreateNodeContextualMenu("Add Node (Dialogue)", SSNodeType.Dialogue));
            this.AddManipulator(CreateNodeContextualMenu("Add Node (Task)", SSNodeType.Task));
            this.AddManipulator(CreateNodeContextualMenu("Add Node (Reward)", SSNodeType.Reward));
            this.AddManipulator(CreateNodeContextualMenu("Add Node (Time)", SSNodeType.Time));

            this.AddManipulator(CreateGroupContextualMenu());
        }

        private IManipulator CreateGroupContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction("Add Group",
                    actionEvent => CreateGroup("NodeGroup",
                        GetLocalMousePosition(actionEvent.eventInfo.localMousePosition)))
            );

            return contextualMenuManipulator;
        }

        private IManipulator CreateNodeContextualMenu(string actionTitle, SSNodeType nodeType)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(actionTitle,
                    actionEvent => AddElement(CreateNode($"{nodeType.ToString()}Node", nodeType,
                        GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
            );

            return contextualMenuManipulator;
        }

        #endregion

        #region Elements Creation

        public SSGroup CreateGroup(string title, Vector2 position)
        {
            SSGroup group = new SSGroup(title, position);

            AddGroup(group);

            AddElement(group);

            group.SetPosition(new Rect(position, Vector2.zero));

            foreach (GraphElement selectedElement in selection)
            {
                if (!(selectedElement is SSNode))
                {
                    continue;
                }

                SSNode node = (SSNode)selectedElement;

                group.AddElement(node);
            }

            return group;
        }

        public SSNode CreateNode(string nodeName, SSNodeType nodeType, Vector2 position, bool shouldDraw = true)
        {
            Type nodeTypeSystem = Type.GetType($"SS.Elements.SS{nodeType}Node");
            SSNode node = (SSNode)Activator.CreateInstance(nodeTypeSystem);

            switch (nodeType)
            {
                case SSNodeType.Dialogue:
                {
                    node = (SSDialogueNode)Activator.CreateInstance(nodeTypeSystem);
                    break;
                }
                case SSNodeType.Task:
                {
                    node = (SSTaskNode)Activator.CreateInstance(nodeTypeSystem);
                    break;
                }
                case SSNodeType.Reward:
                {
                    node = (SSRewardNode)Activator.CreateInstance(nodeTypeSystem);
                    break;
                }
                case SSNodeType.Time:
                {
                    node = (SSTimeNode)Activator.CreateInstance(nodeTypeSystem);
                    break;
                }
            }

            node.Initialize(nodeName, this, position);

            if (shouldDraw)
            {
                node.Draw();
            }

            AddUngroupedNode(node);

            return node;
        }

        #endregion

        #region Callbacks

        private void OnElementsDeleted()
        {
            deleteSelection = (operationName, askUser) =>
            {
                Type groupType = typeof(SSGroup);
                Type edgeType = typeof(Edge);

                List<SSGroup> groupsToDelete = new List<SSGroup>();
                List<Edge> edgesToDelete = new List<Edge>();
                List<SSNode> nodesToDelete = new List<SSNode>();

                foreach (GraphElement element in selection)
                {
                    if (element is SSNode node)
                    {
                        nodesToDelete.Add(node);

                        continue;
                    }

                    if (element.GetType() == edgeType)
                    {
                        Edge edge = (Edge)element;

                        edgesToDelete.Add(edge);

                        continue;
                    }

                    if (element.GetType() != groupType)
                    {
                        continue;
                    }

                    SSGroup group = (SSGroup)element;

                    groupsToDelete.Add(group);
                }

                foreach (SSGroup group in groupsToDelete)
                {
                    List<SSNode> groupNodes = new List<SSNode>();

                    foreach (GraphElement groupElement in group.containedElements)
                    {
                        if (!(groupElement is SSNode))
                        {
                            continue;
                        }

                        SSNode groupNode = (SSNode)groupElement;

                        groupNodes.Add(groupNode);
                    }

                    group.RemoveElements(groupNodes);

                    RemoveGroup(group);

                    RemoveElement(group);
                }

                DeleteElements(edgesToDelete);

                foreach (SSNode node in nodesToDelete)
                {
                    if (node.Group != null)
                    {
                        node.Group.RemoveElement(node);
                    }

                    RemoveUngroupedNode(node);

                    node.DisconnectAllPorts();

                    RemoveElement(node);
                }
            };
        }

        private void OnGroupElementsAdded()
        {
            elementsAddedToGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (!(element is SSNode))
                    {
                        continue;
                    }

                    SSGroup nodeGroup = (SSGroup)group;
                    SSNode node = (SSNode)element;

                    RemoveUngroupedNode(node);
                    AddGroupedNode(node, nodeGroup);
                }
            };
        }

        private void OnGroupElementsRemoved()
        {
            elementsRemovedFromGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (!(element is SSNode))
                    {
                        continue;
                    }

                    SSNode node = (SSNode)element;

                    RemoveGroupedNode(node, group);
                    AddUngroupedNode(node);
                }
            };
        }

        private void OnGroupRenamed()
        {
            groupTitleChanged = (group, newTitle) =>
            {
                SSGroup ssGroup = (SSGroup)group;

                ssGroup.title = newTitle.RemoveWhitespaces().RemoveSpecialCharacters();

                if (string.IsNullOrEmpty(ssGroup.title))
                {
                    if (!string.IsNullOrEmpty(ssGroup.OldTitle))
                    {
                        ++NameErrorsAmount;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(ssGroup.OldTitle))
                    {
                        --NameErrorsAmount;
                    }
                }

                RemoveGroup(ssGroup);

                ssGroup.OldTitle = ssGroup.title;

                AddGroup(ssGroup);
            };
        }

        private void OnGraphViewChanged()
        {
            graphViewChanged = (changes) =>
            {
                if (changes.edgesToCreate != null)
                {
                    foreach (Edge edge in changes.edgesToCreate)
                    {
                        SSNode nextNode = (SSNode)edge.input.node;

                        SSNode previousNode = (SSNode)edge.output.node;

                        SSChoiceSaveData choiceData = (SSChoiceSaveData)edge.output.userData;

                        choiceData.NextNodeID = nextNode.ID;
                    }
                }

                if (changes.elementsToRemove != null)
                {
                    Type edgeType = typeof(Edge);

                    foreach (GraphElement element in changes.elementsToRemove)
                    {
                        if (element.GetType() != edgeType)
                        {
                            continue;
                        }

                        Edge edge = (Edge)element;

                        SSChoiceSaveData choiceData = (SSChoiceSaveData)edge.output.userData;

                        choiceData.NextNodeID = "";
                    }
                }

                return changes;
            };
        }

        #endregion

        #region Repeated Elements

        public void AddUngroupedNode(SSNode node)
        {
            string nodeName = node.NodeName.ToLower();

            if (!ungroupedNodes.ContainsKey(nodeName))
            {
                SSNodeErrorData nodeErrorData = new SSNodeErrorData();

                nodeErrorData.Nodes.Add(node);

                ungroupedNodes.Add(nodeName, nodeErrorData);

                return;
            }

            List<SSNode> ungroupedNodesList = ungroupedNodes[nodeName].Nodes;

            ungroupedNodesList.Add(node);

            Color errorColor = ungroupedNodes[nodeName].ErrorData.Color;

            node.SetErrorStyle(errorColor);

            if (ungroupedNodesList.Count == 2)
            {
                ++NameErrorsAmount;

                ungroupedNodesList[0].SetErrorStyle(errorColor);
            }
        }

        public void RemoveUngroupedNode(SSNode node)
        {
            string nodeName = node.NodeName.ToLower();

            List<SSNode> ungroupedNodesList = ungroupedNodes[nodeName].Nodes;

            ungroupedNodesList.Remove(node);

            node.ResetStyle();

            if (ungroupedNodesList.Count == 1)
            {
                --NameErrorsAmount;
                ungroupedNodesList[0].ResetStyle();

                return;
            }

            if (ungroupedNodesList.Count == 0)
            {
                ungroupedNodes.Remove(nodeName);
            }
        }

        private void AddGroup(SSGroup group)
        {
            string groupName = group.title.ToLower();

            if (!Groups.ContainsKey(groupName))
            {
                SSGroupErrorData groupErrorData = new SSGroupErrorData();

                groupErrorData.Groups.Add(group);

                Groups.Add(groupName, groupErrorData);

                return;
            }

            List<SSGroup> groupsList = Groups[groupName].Groups;

            groupsList.Add(group);

            Color errorColor = Groups[groupName].ErrorData.Color;

            group.SetErrorStyle(errorColor);

            if (groupsList.Count == 2)
            {
                ++NameErrorsAmount;

                groupsList[0].SetErrorStyle(errorColor);
            }
        }

        private void RemoveGroup(SSGroup group)
        {
            string oldGroupName = group.OldTitle.ToLower();

            List<SSGroup> groupsList = Groups[oldGroupName].Groups;

            groupsList.Remove(group);

            group.ResetStyle();

            if (groupsList.Count == 1)
            {
                --NameErrorsAmount;

                groupsList[0].ResetStyle();

                return;
            }

            if (groupsList.Count == 0)
            {
                Groups.Remove(oldGroupName);
            }
        }

        public void AddGroupedNode(SSNode node, SSGroup group)
        {
            string nodeName = node.NodeName.ToLower();

            node.Group = group;

            if (!groupedNodes.ContainsKey(group))
            {
                groupedNodes.Add(group, new SerializableDictionary<string, SSNodeErrorData>());
            }

            if (!groupedNodes[group].ContainsKey(nodeName))
            {
                SSNodeErrorData nodeErrorData = new SSNodeErrorData();

                nodeErrorData.Nodes.Add(node);

                groupedNodes[group].Add(nodeName, nodeErrorData);

                return;
            }

            List<SSNode> groupedNodesList = groupedNodes[group][nodeName].Nodes;

            groupedNodesList.Add(node);

            Color errorColor = groupedNodes[group][nodeName].ErrorData.Color;

            node.SetErrorStyle(errorColor);

            if (groupedNodesList.Count == 2)
            {
                ++NameErrorsAmount;

                groupedNodesList[0].SetErrorStyle(errorColor);
            }
        }

        public void RemoveGroupedNode(SSNode node, Group group)
        {
            string nodeName = node.NodeName.ToLower();

            node.Group = null;

            List<SSNode> groupedNodesList = groupedNodes[group][nodeName].Nodes;

            groupedNodesList.Remove(node);

            node.ResetStyle();

            if (groupedNodesList.Count == 1)
            {
                --NameErrorsAmount;

                groupedNodesList[0].ResetStyle();

                return;
            }

            if (groupedNodesList.Count == 0)
            {
                groupedNodes[group].Remove(nodeName);

                if (groupedNodes[group].Count == 0)
                {
                    groupedNodes.Remove(group);
                }
            }
        }

        #endregion

        #region Elements Addition

        private void AddSearchWindow()
        {
            if (searchWindow == null)
            {
                searchWindow = ScriptableObject.CreateInstance<SSSearchWindow>();

                searchWindow.Initialize(this);
            }

            nodeCreationRequest = context =>
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
        }

        private void AddMinimap()
        {
            miniMap = new MiniMap()
            {
                anchored = true
            };

            miniMap.SetPosition(new Rect(15, 50, 200, 180));

            Add(miniMap);

            miniMap.visible = false;
        }

        private void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground();

            gridBackground.StretchToParentSize();

            Insert(0, gridBackground);
        }

        private void AddStyles()
        {
            this.AddStyleSheets("StorylineSystem/SSGraphViewStyles.uss", "StorylineSystem/SSNodeStyles.uss");
        }

        private void AddMiniMapStyles()
        {
            StyleColor backgroundColor = new StyleColor(new Color32(29, 29, 30, 255));
            StyleColor borderColor = new StyleColor(new Color32(51, 51, 51, 255));

            miniMap.style.backgroundColor = backgroundColor;
            miniMap.style.borderTopColor = borderColor;
            miniMap.style.borderRightColor = borderColor;
            miniMap.style.borderBottomColor = borderColor;
            miniMap.style.borderLeftColor = borderColor;
        }

        #endregion

        #region Utilities

        public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isSearchWindow = false)
        {
            Vector2 worldMousePosition = mousePosition;

            if (isSearchWindow)
            {
                worldMousePosition -= editorWindow.position.position;
            }

            Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);

            return localMousePosition;
        }

        public void ClearGraph()
        {
            graphElements.ForEach(graphElement => RemoveElement(graphElement));

            Groups.Clear();
            groupedNodes.Clear();
            ungroupedNodes.Clear();

            NameErrorsAmount = 0;
        }

        public void ToggleMiniMap()
        {
            miniMap.visible = !miniMap.visible;
        }

        #endregion
    }
}