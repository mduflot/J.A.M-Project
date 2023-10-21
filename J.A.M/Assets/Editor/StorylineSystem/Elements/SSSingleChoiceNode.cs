using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace SS.Elements
{
    using Enumerations;
    using Utilities;
    using Windows;
    
    public class SSSingleChoiceNode : SSNode
    {
        public override void Initialize(SSGraphView ssGraphView, Vector2 position)
        {
            base.Initialize(ssGraphView, position);

            NodeType = SSNodeType.SingleChoice;
            
            Choices.Add("Next Node");
        }

        public override void Draw()
        {
            base.Draw();

            /* OUTPUT CONTAINER */
            
            foreach (string choice in Choices)
            {
                Port choicePort = this.CreatePort(choice); 

                choicePort.portName = choice;
                
                outputContainer.Add(choicePort);
            }
            
            RefreshExpandedState();
        }
    }
}