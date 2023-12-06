using System;
using System.Collections.Generic;
using System.Linq;
using SS.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace SS.Elements
{
    using Enumerations;
    using Windows;
    using Data.Save;
    
    public class SSNode : Node
    {
        public string ID { get; set; }
        public string NodeName { get; set; }
        public List<SSChoiceSaveData> Choices { get; set; }
        public SSNodeType NodeType { get; set; }
        public SSGroup Group { get; set; }

        protected SSGraphView graphView;
        
        private Color defaultBackgroundColor;

        public virtual void Initialize(string nodeName, SSGraphView ssGraphView, Vector2 position)
        {
            ID = Guid.NewGuid().ToString();
            NodeName = nodeName;
            Choices = new List<SSChoiceSaveData>();

            graphView = ssGraphView;
            defaultBackgroundColor = new Color(29f / 255f, 29 / 255f, 30 / 255f);

            SetPosition(new Rect(position, Vector2.zero));

            mainContainer.AddToClassList("ss-node_main-container");
            extensionContainer.AddToClassList("ss-node_extension-container");
        }

        public virtual void Draw()
        {
            /* TITLE CONTAINER */

            TextField nodeNameTextField = SSElementUtility.CreateTextField(NodeName, null, callback =>
            {
                TextField target = (TextField) callback.target;

                target.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();

                if (string.IsNullOrEmpty(target.value))
                {
                    if (!string.IsNullOrEmpty(NodeName))
                    {
                        ++graphView.NameErrorsAmount;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(NodeName))
                    {
                        --graphView.NameErrorsAmount;
                    }
                }

                if (Group == null)
                {
                    graphView.RemoveUngroupedNode(this);

                    NodeName = target.value;

                    graphView.AddUngroupedNode(this);

                    return;
                }

                SSGroup currentGroup = Group;

                graphView.RemoveGroupedNode(this, Group);

                NodeName = target.value;

                graphView.AddGroupedNode(this, currentGroup);
            });

            nodeNameTextField.AddClasses("ss-node__text-field", "ss-node__filename-text-field",
                "ss-node__text-field__hidden");

            titleContainer.Insert(0, nodeNameTextField);

            /* INPUT CONTAINER */

            Port inputPort = this.CreatePort("Node Connection", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);

            inputPort.portName = "Node Connection";

            inputContainer.Add(inputPort);
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

        public bool IsStartingNode()
        {
            Port inputPort = (Port) inputContainer.Children().First();

            return !inputPort.connected;
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