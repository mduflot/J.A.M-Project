using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace SS.Elements
{
    using Enumerations;
    using Utilities;
    
    public class SSNode : Node
    {
        public string NodeName { get; set; }
        public List<string> Choices { get; set; }
        public string Text { get; set; }
        public SSNodeType NodeType { get; set; }

        public virtual void Initialize(Vector2 position)
        {
            NodeName = "NodeName";
            Choices = new List<string>();
            Text = "Node text.";
            
            SetPosition(new Rect(position, Vector2.zero));
            
            mainContainer.AddToClassList("ss-node_main-container");
            extensionContainer.AddToClassList("ss-node_extension-container");
        }

        public virtual void Draw()
        {
            /* TITLE CONTAINER */

            TextField nodeNameTextField = SSElementUtility.CreateTextField(NodeName);

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
    }
}