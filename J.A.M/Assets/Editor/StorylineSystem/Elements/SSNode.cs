using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace SS.Elements
{
    using Enumerations;
    
    public class SSNode : Node
    {
        public string NodeName { get; set; }
        public List<string> Choices { get; set; }
        public string Text { get; set; }
        public SSNodeType NodeType { get; set; }

        public void Initialize(Vector2 position)
        {
            NodeName = "NodeName";
            Choices = new List<string>();
            Text = "Node text.";
            
            SetPosition(new Rect(position, Vector2.zero));
        }

        public void Draw()
        {
            /* TITLE CONTAINER */
            
            TextField nodeNameTextField = new TextField()
            {
                value = NodeName
            };
            
            titleContainer.Insert(0, nodeNameTextField);

            /* INPUT CONTAINER */

            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));

            inputPort.portName = "Node Connection";

            inputContainer.Add(inputPort);

            /* EXTENSIONS CONTAINER */
            
            VisualElement customDataContainer = new VisualElement();

            Foldout textFoldout = new Foldout()
            {
                text = "Node Text."
            };

            TextField textTextField = new TextField()
            {
                value = Text
            };
            
            textFoldout.Add(textTextField);
            
            customDataContainer.Add(textFoldout);
            
            extensionContainer.Add(customDataContainer);
            
            RefreshExpandedState();
        }
    }
}