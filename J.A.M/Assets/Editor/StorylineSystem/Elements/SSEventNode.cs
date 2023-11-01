using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace SS.Elements
{
    using Data.Save;
    using Enumerations;
    using Utilities;
    using Windows;

    public class SSEventNode : SSNode
    {
        public string Text { get; set; }
        public int LeaderCount { get; set; }

        public override void Initialize(string nodeName, SSGraphView ssGraphView, Vector2 position)
        {
            base.Initialize(nodeName, ssGraphView, position);

            NodeType = SSNodeType.Event;
            Text = "Node text.";
            LeaderCount = 1;

            SSChoiceSaveData choiceData = new SSChoiceSaveData()
            {
                Text = "New Choice"
            };

            Choices.Add(choiceData);
        }

        public override void Draw()
        {
            base.Draw();

            /* MAIN CONTAINER */

            Button addChoiceButton = SSElementUtility.CreateButton("Add Choice", () =>
            {
                SSChoiceSaveData choiceData = new SSChoiceSaveData()
                {
                    Text = "New Choice"
                };

                Choices.Add(choiceData);
                
                Port choicePort = CreateChoicePort(choiceData);

                outputContainer.Add(choicePort);
            });

            addChoiceButton.AddToClassList("ss-node__button");

            mainContainer.Insert(1, addChoiceButton);

            /* OUTPUT CONTAINER */

            foreach (SSChoiceSaveData choice in Choices)
            {
                Port choicePort = CreateChoicePort(choice);

                outputContainer.Add(choicePort);
            }

            /* EXTENSIONS CONTAINER */

            VisualElement customDataContainer = new VisualElement();
            
            customDataContainer.AddToClassList("ss-node__custom-data-container");

            Foldout textFoldout = SSElementUtility.CreateFoldout("Node Text");

            TextField textTextField = SSElementUtility.CreateTextArea(Text, null, callback =>
            {
                Text = callback.newValue;
            });

            textTextField.AddClasses("ss-node__text-field", "ss-node__quote-text-field");
            
            textFoldout.Add(textTextField);

            customDataContainer.Add(textFoldout);

            SliderInt sliderField = null;
            sliderField = SSElementUtility.CreateSliderField(LeaderCount, "Leaders : ", callback =>
            {
                LeaderCount = Mathf.Min(callback.newValue, 5);
                sliderField.label = "Leaders : " + callback.newValue;
            });

            customDataContainer.Add(sliderField);

            extensionContainer.Add(customDataContainer);

            RefreshExpandedState();
        }

        #region Elements Creation

        private Port CreateChoicePort(object userData)
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

            deleteChoiceButton.AddToClassList("ss-node__button");

            TextField choiceTextField = SSElementUtility.CreateTextField(choiceData.Text, null, callback =>
            {
                choiceData.Text = callback.newValue;
            });

            choiceTextField.AddClasses("ss-node__text-field", "ss-node__choice-text-field", "ss-node__text-field__hidden");

            choicePort.Add(choiceTextField);
            choicePort.Add(deleteChoiceButton);
            return choicePort;
        }

        #endregion
    }
}