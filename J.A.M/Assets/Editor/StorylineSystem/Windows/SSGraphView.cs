using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace SS.Windows
{
    using Elements;
    using Enumerations;

    public class SSGraphView : GraphView
    {
        public SSGraphView()
        {
            AddManipulators();
            AddGridBackground();

            AddStyles();
        }

        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            
            this.AddManipulator(CreateNodeContextualMenu("Add Node (Single Choice)", SSNodeType.SingleChoice));
            this.AddManipulator(CreateNodeContextualMenu("Add Node (Multiple Choice)", SSNodeType.MultipleChoice));
        }

        private IManipulator CreateNodeContextualMenu(string actionTitle, SSNodeType nodeType)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode(nodeType, actionEvent.eventInfo.localMousePosition)))
            );

            return contextualMenuManipulator;
        }

        private SSNode CreateNode(SSNodeType nodeType, Vector2 position)
        {
            Type nodeTypeSystem = Type.GetType($"SS.Elements.SS{nodeType}Node");
            SSNode node = (SSNode) Activator.CreateInstance(nodeTypeSystem);

            node.Initialize(position);
            node.Draw();

            return node;
        }

        private void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground();

            gridBackground.StretchToParentSize();

            Insert(0, gridBackground);
        }

        private void AddStyles()
        {
            StyleSheet graphViewStyleSheet = (StyleSheet)EditorGUIUtility.Load("StorylineSystem/SSGraphViewStyles.uss");
            StyleSheet nodeStyleSheet = (StyleSheet)EditorGUIUtility.Load("StorylineSystem/SSNodeStyles.uss");

            styleSheets.Add(graphViewStyleSheet);
            styleSheets.Add(nodeStyleSheet);
        }
    }
}