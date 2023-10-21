using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace SS.Windows
{
    using Data.Error;
    using Elements;
    using Enumerations;
    using Utilities;

    public class SSGraphView : GraphView
    {
        private SSEditorWindow editorWindow;
        private SSSearchWindow searchWindow;

        private SerializableDictionary<string, SSNodeErrorData> ungroupedNodes;
        private SerializableDictionary<string, SSGroupErrorData> groups;
        private SerializableDictionary<Group, SerializableDictionary<string, SSNodeErrorData>> groupedNodes;

        private int repeatedNamesAmount;

        public int RepeatedNamesAmount
        {
            get
            {
                return repeatedNamesAmount;
            }

            set
            {
                repeatedNamesAmount = value;

                if (repeatedNamesAmount == 0)
                {
                    editorWindow.EnableSaving();
                }

                if (repeatedNamesAmount == 1)
                {
                    editorWindow.DisableSaving();
                }
            }
        }
        
        public SSGraphView(SSEditorWindow ssEditorWindow)
        {
            editorWindow = ssEditorWindow;

            ungroupedNodes = new SerializableDictionary<string, SSNodeErrorData>();
            groups = new SerializableDictionary<string, SSGroupErrorData>();
            groupedNodes = new SerializableDictionary<Group, SerializableDictionary<string, SSNodeErrorData>>();
            
            AddManipulators();
            AddSearchWindow();
            AddGridBackground();

            OnElementsDeleted();
            OnGroupElementsAdded();
            OnGroupElementsRemoved();
            OnGroupRenamed();

            AddStyles();
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
            
            this.AddManipulator(CreateNodeContextualMenu("Add Node (Single Choice)", SSNodeType.SingleChoice));
            this.AddManipulator(CreateNodeContextualMenu("Add Node (Multiple Choice)", SSNodeType.MultipleChoice));

            this.AddManipulator(CreateGroupContextualMenu());
        }

        private IManipulator CreateGroupContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction("Add Group", actionEvent => CreateGroup("NodeGroup", GetLocalMousePosition(actionEvent.eventInfo.localMousePosition)))
            );
            
            return contextualMenuManipulator;
        }
        
        private IManipulator CreateNodeContextualMenu(string actionTitle, SSNodeType nodeType)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode(nodeType, GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
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

        public SSNode CreateNode(SSNodeType nodeType, Vector2 position)
        {
            Type nodeTypeSystem = Type.GetType($"SS.Elements.SS{nodeType}Node");
            SSNode node = (SSNode) Activator.CreateInstance(nodeTypeSystem ?? throw new InvalidOperationException());

            node.Initialize(this, position);
            node.Draw();

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
                        Edge edge = (Edge) element;
                        
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

                        SSNode groupNode = (SSNode) groupElement;
                        
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

                    SSGroup nodeGroup = (SSGroup) group;
                    SSNode node = (SSNode) element;
                    
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

                    SSNode node = (SSNode) element;

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
                
                RemoveGroup(ssGroup);

                ssGroup.oldTitle = ssGroup.title;
                
                AddGroup(ssGroup);
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
                ++RepeatedNamesAmount;
                
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
                --RepeatedNamesAmount;
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

            if (!groups.ContainsKey(groupName))
            {
                SSGroupErrorData groupErrorData = new SSGroupErrorData();
                
                groupErrorData.Groups.Add(group);
                
                groups.Add(groupName, groupErrorData);

                return;
            }

            List<SSGroup> groupsList = groups[groupName].Groups;
            
            groupsList.Add(group);

            Color errorColor = groups[groupName].ErrorData.Color;
            
            group.SetErrorStyle(errorColor);

            if (groupsList.Count == 2)
            {
                ++RepeatedNamesAmount;
                
                groupsList[0].SetErrorStyle(errorColor);
            }
        }
        
        private void RemoveGroup(SSGroup group)
        {
            string oldGroupName = group.oldTitle.ToLower();

            List<SSGroup> groupsList = groups[oldGroupName].Groups;
            
            groupsList.Remove(group);
            
            group.ResetStyle();

            if (groupsList.Count == 1)
            {
                --RepeatedNamesAmount;
                
                groupsList[0].ResetStyle();

                return;
            }

            if (groupsList.Count == 0)
            {
                groups.Remove(oldGroupName);
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
                ++RepeatedNamesAmount;
                
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
                --RepeatedNamesAmount;
                
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

            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
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
        
        #endregion
    }
}