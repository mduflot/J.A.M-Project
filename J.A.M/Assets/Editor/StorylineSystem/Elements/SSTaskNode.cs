using System.Collections.Generic;
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

    public class SSTaskNode : SSNode
    {
        public TaskDataScriptable TaskData { get; set; }
        
        private VisualElement customDataContainer = new();

        public override void Initialize(string nodeName, SSGraphView ssGraphView, Vector2 position)
        {
            base.Initialize(nodeName, ssGraphView, position);

            NodeType = SSNodeType.Task;

            SSChoiceSaveData choiceAssignedData = new SSChoiceSaveData()
            {
                Text = "Assigned"
            };

            Choices.Add(choiceAssignedData);
        }

        public override void Draw()
        {
            base.Draw();

            customDataContainer.AddToClassList("ss-node__custom-data-container");
            
            /* OUTPUT CONTAINER */

            foreach (SSChoiceSaveData choice in Choices)
            {
                CreateChoicePort(choice);
            }

            /* EXTENSIONS CONTAINER */

            ObjectField objectField = SSElementUtility.CreateObjectField(TaskData, typeof(TaskDataScriptable), "TaskData :", callback =>
            {
                TaskData = (TaskDataScriptable)callback.newValue;
            });
            
            customDataContainer.Add(objectField);

            extensionContainer.Add(customDataContainer);

            RefreshExpandedState();
        }

        #region Elements Creation

        private void CreateChoicePort(object userData)
        {
            Port choicePort = this.CreatePort();

            choicePort.userData = userData;

            SSChoiceSaveData choiceData = (SSChoiceSaveData) userData;

            Button deleteChoiceButton = SSElementUtility.CreateButton("X", () =>
            {
                if (Choices.Count == 1)
                {
                    return;
                }

                if (choicePort.connected)
                {
                    graphView.DeleteElements(choicePort.connections);
                }

                Choices.Remove(choiceData);

                graphView.RemoveElement(choicePort);
            });

            deleteChoiceButton.AddToClassList("ds-node__button");

            TextField choiceTextField = SSElementUtility.CreateTextField(choiceData.Text, null, callback =>
            {
                choiceData.Text = callback.newValue;
            });

            choiceTextField.AddClasses(
                "ss-node__text-field",
                "ss-node__text-field__hidden",
                "ss-node__choice-text-field"
            );

            choicePort.Add(choiceTextField);
            choicePort.Add(deleteChoiceButton);

            outputContainer.Add(choicePort);
        }

        #endregion
    }
}