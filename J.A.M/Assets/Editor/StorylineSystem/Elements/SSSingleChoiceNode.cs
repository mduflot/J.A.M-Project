using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace SS.Elements
{
    using Enumerations;
    
    public class SSSingleChoiceNode : SSNode
    {
        public override void Initialize(Vector2 position)
        {
            base.Initialize(position);

            NodeType = SSNodeType.SingleChoice;
            
            Choices.Add("Next Node");
        }

        public override void Draw()
        {
            base.Draw();

            /* OUTPUT CONTAINER */
            
            foreach (string choice in Choices)
            {
                Port choicePort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single,
                    typeof(bool));

                choicePort.portName = choice;
                
                outputContainer.Add(choicePort);
            }
            
            RefreshExpandedState();
        }
    }
}