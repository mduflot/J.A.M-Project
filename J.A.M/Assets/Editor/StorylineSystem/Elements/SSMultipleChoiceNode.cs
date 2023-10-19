using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace SS.Elements
{
    using Enumerations;
    
    public class SSMultipleChoiceNode :SSNode
    {
        public override void Initialize(Vector2 position)
        {
            base.Initialize(position);

            NodeType = SSNodeType.MultipleChoice;
            
            Choices.Add("New Choice");
        }

        public override void Draw()
        {
            base.Draw();

            /* MAIN CONTAINER */
            
            Button addChoiceButton = new Button()
            {
                text = "Add Choice"
            };
            
            addChoiceButton.AddToClassList("ss-node__button");
            
            mainContainer.Insert(1, addChoiceButton);
            
            /* OUTPUT CONTAINER */

            foreach (string choice in Choices)
            {
                Port choicePort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single,
                    typeof(bool));

                choicePort.portName = "";

                Button deleteChoiceButton = new Button()
                {
                    text = "X"
                };
                
                deleteChoiceButton.AddToClassList("ss-node__button");

                TextField choiceTextField = new TextField()
                {
                    value = choice
                };
                
                choiceTextField.AddToClassList("ss-node__text-field");
                choiceTextField.AddToClassList("ss-node__choice-text-field");
                choiceTextField.AddToClassList("ss-node__text-field__hidden");
                
                choicePort.Add(choiceTextField);
                choicePort.Add(deleteChoiceButton);
                
                outputContainer.Add(choicePort);
            }
            
            RefreshExpandedState();
        }
    }
}

