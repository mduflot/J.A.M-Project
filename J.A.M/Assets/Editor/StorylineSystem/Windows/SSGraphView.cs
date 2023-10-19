using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace SS.Windows
{
    using Elements;
    using Enumerations;
    using Utilities;

    public class SSGraphView : GraphView
    {
        public SSGraphView()
        {
            AddManipulators();
            AddGridBackground();

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
                menuEvent => menuEvent.menu.AppendAction("Add Group", actionEvent => AddElement(CreateGroup("NodeGroup", actionEvent.eventInfo.localMousePosition)))
            );
            
            return contextualMenuManipulator;
        }
        
        private IManipulator CreateNodeContextualMenu(string actionTitle, SSNodeType nodeType)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode(nodeType, actionEvent.eventInfo.localMousePosition)))
            );

            return contextualMenuManipulator;
        }
        
        #endregion

        #region Elements Creation
        
        private GraphElement CreateGroup(string title, Vector2 position)
        {
            Group group = new Group()
            {
                title = title
            };
            
            group.SetPosition(new Rect(position, Vector2.zero));

            return group;
        }
        
        private SSNode CreateNode(SSNodeType nodeType, Vector2 position)
        {
            Type nodeTypeSystem = Type.GetType($"SS.Elements.SS{nodeType}Node");
            SSNode node = (SSNode) Activator.CreateInstance(nodeTypeSystem ?? throw new InvalidOperationException());

            node.Initialize(position);
            node.Draw();

            return node;
        }
        
        #endregion

        #region Elements Addition
        
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
    }
}