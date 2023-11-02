using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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
        public string Text { get; set; }
        public int LeaderCount { get; set; }
        private VisualElement customDataContainer = new();

        public override void Initialize(string nodeName, SSGraphView ssGraphView, Vector2 position)
        {
            base.Initialize(nodeName, ssGraphView, position);

            NodeType = SSNodeType.Task;
            Text = "Node text.";
            LeaderCount = 1;

            SSChoiceTaskSaveData choiceAssignedData = new SSChoiceTaskSaveData()
            {
                Text = "Assigned",
                ChoiceTypes = new List<SSChoiceType>()
                {
                    SSChoiceType.Assigned
                }
            };
            
            SSChoiceTaskSaveData choiceNotAssignedData = new SSChoiceTaskSaveData()
            {
                Text = "Not Assigned",
                ChoiceTypes = new List<SSChoiceType>()
                {
                    SSChoiceType.NotAssigned
                }
            };

            Choices.Add(choiceAssignedData);
            Choices.Add(choiceNotAssignedData);
        }

        public override void Draw()
        {
            base.Draw();

            customDataContainer.AddToClassList("ss-node__custom-data-container");

            /* MAIN CONTAINER */

            Button addChoiceButton = SSElementUtility.CreateButton("Add Choice", () =>
            {
                /* OUTPUT CONTAINER */

                SSChoiceTaskSaveData choiceData = new SSChoiceTaskSaveData()
                {
                    Text = "New Choice",
                    ChoiceTypes = new List<SSChoiceType>()
                    {
                        SSChoiceType.Assigned
                    }
                };

                Choices.Add(choiceData);

                CreateChoicePort(choiceData);
            });

            addChoiceButton.AddToClassList("ss-node__button");

            mainContainer.Insert(1, addChoiceButton);

            /* OUTPUT CONTAINER */

            foreach (SSChoiceTaskSaveData choice in Choices)
            {
                CreateChoicePort(choice);
            }

            /* EXTENSIONS CONTAINER */

            /* TEXT CONTAINER */

            Foldout textFoldout = SSElementUtility.CreateFoldout("Description :");

            TextField textTextField = SSElementUtility.CreateTextArea(Text, null, callback =>
            {
                Text = callback.newValue;
            });

            textTextField.AddClasses("ss-node__text-field", "ss-node__quote-text-field");
            
            textFoldout.Add(textTextField);

            customDataContainer.Add(textFoldout);

            /* SLIDER CONTAINER */
            
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

        private void CreateChoicePort(object userData)
        {
            Port choicePort = this.CreatePort();

            choicePort.userData = userData;

            SSChoiceTaskSaveData choiceData = (SSChoiceTaskSaveData) userData;
            
            /* CHOICE CONDITIONS CONTAINER */

            Foldout choiceConditionsFoldout = SSElementUtility.CreateFoldout($"\"{choiceData.Text}\" :");

            Button addConditionButton = SSElementUtility.CreateButton("Add Condition", () =>
            {
                choiceData.ChoiceTypes.Add(SSChoiceType.Assigned);
                VisualElement choiceConditionsDataContainer = new();

                EnumField enumField = SSElementUtility.CreateEnumField(choiceData.ChoiceTypes[^1], "Condition :", callback =>
                {
                    choiceData.ChoiceTypes[^1] = (SSChoiceType)callback.newValue;
                });
                
                Button deleteConditionButton = SSElementUtility.CreateButton("X", () =>
                {
                    if (choiceData.ChoiceTypes.Count == 1)
                    {
                        return;
                    }
                
                    choiceData.ChoiceTypes.RemoveAt(choiceData.ChoiceTypes.Count - 1);
                    choiceConditionsFoldout.Remove(choiceConditionsDataContainer);
                });
                
                deleteConditionButton.AddToClassList("ss-node__button");
                
                choiceConditionsDataContainer.Add(deleteConditionButton);
                choiceConditionsDataContainer.Add(enumField);

                choiceConditionsFoldout.Add(choiceConditionsDataContainer);
            });
            
            addConditionButton.AddToClassList("ss-node__button");
                
            choiceConditionsFoldout.Add(addConditionButton);

            for(int i = 0; i < choiceData.ChoiceTypes.Count; i++)
            {
                var choiceTypeIndex = i;
                
                VisualElement choiceConditionsDataContainer = new();
                
                EnumField enumField = SSElementUtility.CreateEnumField(choiceData.ChoiceTypes[choiceTypeIndex], "Condition :", callback =>
                {
                    choiceData.ChoiceTypes[choiceTypeIndex] = (SSChoiceType)callback.newValue;
                });
                
                Button deleteConditionButton = SSElementUtility.CreateButton("X", () =>
                {
                    if (choiceData.ChoiceTypes.Count == 1)
                    {
                        return;
                    }
                
                    choiceData.ChoiceTypes.RemoveAt(choiceTypeIndex);
                    choiceConditionsFoldout.Remove(choiceConditionsDataContainer);
                });
                
                deleteConditionButton.AddToClassList("ss-node__button");
                
                choiceConditionsDataContainer.Add(deleteConditionButton);
                choiceConditionsDataContainer.Add(enumField);
                choiceConditionsFoldout.Add(choiceConditionsDataContainer);
            }

            customDataContainer.Insert(Choices.IndexOf(choiceData), choiceConditionsFoldout);

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

                choiceData.ChoiceTypes.Clear();
                
                Choices.Remove(choiceData);
                
                graphView.RemoveElement(choicePort);
                customDataContainer.Remove(choiceConditionsFoldout);
            });

            deleteChoiceButton.AddToClassList("ss-node__button");

            TextField choiceTextField = SSElementUtility.CreateTextField(choiceData.Text, null, callback =>
            {
                choiceData.Text = callback.newValue;
                choiceConditionsFoldout.text = callback.newValue;
            });

            choiceTextField.AddClasses("ss-node__text-field", "ss-node__choice-text-field", "ss-node__text-field__hidden");

            choicePort.Add(choiceTextField);
            choicePort.Add(deleteChoiceButton);
            
            outputContainer.Add(choicePort);
        }

        #endregion
    }
}