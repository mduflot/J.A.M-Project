using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace SS.Elements
{
    using Enumerations;
    using Utilities;
    using Windows;
    
    public class SSNode : Node
    {
        public string NodeName { get; set; }
        public List<string> Choices { get; set; }
        public string Text { get; set; }
        public SSNodeType NodeType { get; set; }
        public SSGroup Group { get; set; }

        private SSGraphView graphView;
        private Color defaultBackgroundColor;

        public virtual void Initialize(SSGraphView ssGraphView, Vector2 position)
        {
            NodeName = "NodeName";
            Choices = new List<string>();
            Text = "Node text.";

            graphView = ssGraphView;
            defaultBackgroundColor = new Color(29f / 255f, 29 / 255f, 30 / 255f);
            
            SetPosition(new Rect(position, Vector2.zero));
            
            mainContainer.AddToClassList("ss-node_main-container");
            extensionContainer.AddToClassList("ss-node_extension-container");
        }

        public virtual void Draw()
        {
            /* TITLE CONTAINER */

            TextField nodeNameTextField = SSElementUtility.CreateTextField(NodeName, callback =>
            {
                if (Group == null)
                {
                    graphView.RemoveUngroupedNode(this);

                    NodeName = callback.newValue;
                
                    graphView.AddUngroupedNode(this);

                    return;
                }

                SSGroup currentGroup = Group;
                
                graphView.RemoveGroupedNode(this, Group);

                NodeName = callback.newValue;
                
                graphView.AddGroupedNode(this, currentGroup);
            });

            nodeNameTextField.AddClasses("ss-node__text-field", "ss-node__filename-text-field",
                "ss-node__text-field__hidden");

            titleContainer.Insert(0, nodeNameTextField);

            /* INPUT CONTAINER */

            Port inputPort = this.CreatePort("Node Connection", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);

            inputPort.portName = "Node Connection";

            inputContainer.Add(inputPort);

            /* EXTENSIONS CONTAINER */
            
            VisualElement customDataContainer = new VisualElement();
            
            customDataContainer.AddToClassList("ss-node__custom-data-container");

            Foldout textFoldout = SSElementUtility.CreateFoldout("Node Text");

            TextField textTextField = SSElementUtility.CreateTextArea(Text);

            textTextField.AddClasses("ss-node__text-field", "ss-node__quote-text-field");
            
            textFoldout.Add(textTextField);
            
            customDataContainer.Add(textFoldout);
            
            extensionContainer.Add(customDataContainer);
        }
        
        #region Overrided Methods
        
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Disconnect Input Ports", actionEvent => DisconnectInputPorts());
            evt.menu.AppendAction("Disconnect Input Ports", actionEvent => DisconnectOutputPorts());
            
            base.BuildContextualMenu(evt);
        }
        
        #endregion

        #region Utility Methods

        public void DisconnectAllPorts()
        {
            DisconnectInputPorts();
            DisconnectOutputPorts();
        }

        private void DisconnectInputPorts()
        {
            DisconnectPorts(inputContainer);
        }
        
        private void DisconnectOutputPorts()
        {
            DisconnectPorts(outputContainer);
        }
        
        private void DisconnectPorts(VisualElement container)
        {
            foreach (Port port in container.Children())
            {
                if (!port.connected)
                {
                    continue;
                }
                
                graphView.DeleteElements(port.connections);
            }
        }

        public void SetErrorStyle(Color color)
        {
            mainContainer.style.backgroundColor = color;
        }

        public void ResetStyle()
        {
            mainContainer.style.backgroundColor = defaultBackgroundColor;
        }
        
        #endregion
    }
}