using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SS.Elements
{
    using Data.Save;
    using Enumerations;
    using Utilities;
    using Windows;

    public class SSCheckConditionNode : SSNode
    {
        public ConditionSO Condition { get; set; }

        public override void Initialize(string nodeName, SSGraphView ssGraphView, Vector2 position)
        {
            base.Initialize(nodeName, ssGraphView, position);

            NodeType = SSNodeType.CheckCondition;

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

            ObjectField conditionField = SSElementUtility.CreateObjectField(Condition, typeof(ConditionSO),
                "Condition :", callback => { Condition = (ConditionSO)callback.newValue; });

            customDataContainer.Add(conditionField);

            extensionContainer.Add(customDataContainer);

            RefreshExpandedState();
        }
    }
}