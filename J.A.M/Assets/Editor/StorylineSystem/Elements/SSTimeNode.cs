using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace SS.Elements
{
    using Data.Save;
    using Enumerations;
    using Utilities;
    using Windows;

    public class SSTimeNode : SSNode
    {
        public uint TimeToWait { get; set; }

        public override void Initialize(string nodeName, SSGraphView ssGraphView, Vector2 position)
        {
            base.Initialize(nodeName, ssGraphView, position);

            NodeType = SSNodeType.Time;
            TimeToWait = 1;

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

            /* EXTENSIONS CONTAINER */

            VisualElement customDataContainer = new();

            customDataContainer.AddToClassList("ss-node__custom-data-container");

            UnsignedIntegerField unsignedIntegerField = SSElementUtility.CreateUnsignedIntegerField(TimeToWait,
                "WaitingTime", callback => { TimeToWait = callback.newValue; });

            customDataContainer.Add(unsignedIntegerField);

            extensionContainer.Add(customDataContainer);

            RefreshExpandedState();
        }
    }
}