using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace SS.Elements
{
    using Data.Save;
    using Enumerations;
    using Utilities;
    using Windows;

    public class SSStartNode : SSNode
    {
        public SSLocationType LocationType { get; set; }
        public EnumField EnumField;

        public override void Initialize(string nodeName, SSGraphView ssGraphView, Vector2 position)
        {
            base.Initialize(nodeName, ssGraphView, position);

            NodeType = SSNodeType.Start;

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

            VisualElement customDataContainer = new VisualElement();

            customDataContainer.AddToClassList("ss-node__custom-data-container");

            EnumField = new EnumField()
            {
                value = LocationType
            };

            EnumField.Init(LocationType);

            EnumField.RegisterValueChangedCallback((value) =>
            {
                LocationType = (SSLocationType)value.newValue;
            });

            customDataContainer.Add(EnumField);

            extensionContainer.Add(customDataContainer);

            RefreshExpandedState();
        }
    }
}