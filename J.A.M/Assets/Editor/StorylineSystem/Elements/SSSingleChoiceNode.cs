using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace SS.Elements
{
    using Data.Save;
    using Enumerations;
    using Utilities;
    using Windows;
    
    public class SSSingleChoiceNode : SSNode
    {
        public override void Initialize(SSGraphView ssGraphView, Vector2 position)
        {
            base.Initialize(ssGraphView, position);

            NodeType = SSNodeType.SingleChoice;

            SSChoiceSaveData choiceData = new SSChoiceSaveData()
            {
                Text = "Next Node"
            };
            
            Choices.Add(choiceData);
        }

        public override void Draw()
        {
            base.Draw();

            /* OUTPUT CONTAINER */
            
            foreach (SSChoiceSaveData choice in Choices)
            {
                Port choicePort = this.CreatePort(choice.Text);

                choicePort.userData = choice;
                
                outputContainer.Add(choicePort);
            }
            
            RefreshExpandedState();
        }
    }
}