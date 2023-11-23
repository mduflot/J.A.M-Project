using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SS.Elements
{
    using Data.Save;
    using Enumerations;
    using Windows;

    public class SSTaskNode : SSNode
    {
        public string descriptionTask { get; set; }
        public Sprite taskIcon { get; set; }
        public float timeLeft { get; set; }
        public float baseDuration { get; set; }
        public int mandatorySlots { get; set; }
        public int optionalSlots { get; set; }
        public float taskHelpFactor { get; set; }
        public SpaceshipManager.ShipRooms room { get; set; }
        public bool isPermanent { get; set; }

        private VisualElement customDataContainer = new();

        public override void Initialize(string nodeName, SSGraphView ssGraphView, Vector2 position)
        {
            base.Initialize(nodeName, ssGraphView, position);

            NodeType = SSNodeType.Task;
            descriptionTask = "Description Task";
            taskIcon = null;
            timeLeft = 0f;
            baseDuration = 0f;
            mandatorySlots = 1;
            optionalSlots = 0;
            taskHelpFactor = 0f;
            room = SpaceshipManager.ShipRooms.Bedrooms;
            isPermanent = false;

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

            Button addChoiceButton = ElementUtility.CreateButton("Add Choice", () =>
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

            Foldout textFoldout = ElementUtility.CreateFoldout("Description Task");

            TextField textTextField =
                ElementUtility.CreateTextArea(descriptionTask, null,
                    callback => { descriptionTask = callback.newValue; });

            textTextField.AddClasses("ss-node__text-field", "ss-node__quote-text-field");

            textFoldout.Add(textTextField);

            customDataContainer.Add(textFoldout);

            ObjectField iconObjectField = ElementUtility.CreateObjectField(taskIcon, typeof(Sprite), "Task Icon :",
                callback => { taskIcon = (Sprite)callback.newValue; });

            customDataContainer.Add(iconObjectField);

            FloatField timeLeftFloatField = ElementUtility.CreateFloatField(timeLeft, "Time Left :",
                callback => { timeLeft = callback.newValue; });

            customDataContainer.Add(timeLeftFloatField);

            FloatField baseDurationFloatField = ElementUtility.CreateFloatField(baseDuration, "Base Duration :",
                callback => { baseDuration = callback.newValue; });

            customDataContainer.Add(baseDurationFloatField);

            IntegerField mandatorySlotsIntegerField = ElementUtility.CreateIntegerField(mandatorySlots,
                "Mandatory Slots :",
                callback => { mandatorySlots = callback.newValue; });

            customDataContainer.Add(mandatorySlotsIntegerField);

            IntegerField optionalSlotsIntegerField = ElementUtility.CreateIntegerField(optionalSlots,
                "Optional Slots :",
                callback => { optionalSlots = callback.newValue; });

            customDataContainer.Add(optionalSlotsIntegerField);

            FloatField taskHelpFactorFloatField = ElementUtility.CreateFloatField(taskHelpFactor, "Task Help Factor :",
                callback => { taskHelpFactor = callback.newValue; });

            customDataContainer.Add(taskHelpFactorFloatField);

            EnumField roomEnumField = ElementUtility.CreateEnumField(room, "Room :",
                callback => { room = (SpaceshipManager.ShipRooms)callback.newValue; });

            customDataContainer.Add(roomEnumField);

            Toggle isPermanentToggle = ElementUtility.CreateToggle(isPermanent, "Is Permanent :",
                callback => { isPermanent = callback.newValue; });

            customDataContainer.Add(isPermanentToggle);

            extensionContainer.Add(customDataContainer);

            RefreshExpandedState();
        }

        #region Elements Creation

        private void CreateChoicePort(object userData)
        {
            Port choicePort = this.CreatePort();

            choicePort.userData = userData;

            SSChoiceTaskSaveData choiceData = (SSChoiceTaskSaveData)userData;

            /* CHOICE CONDITIONS CONTAINER */

            Foldout choiceConditionsFoldout = ElementUtility.CreateFoldout($"\"{choiceData.Text}\" :");

            Button addConditionButton = ElementUtility.CreateButton("Add Condition", () =>
            {
                choiceData.ChoiceTypes.Add(SSChoiceType.Assigned);
                VisualElement choiceConditionsDataContainer = new();

                var index = choiceData.ChoiceTypes.Count - 1;
                EnumField enumField = ElementUtility.CreateEnumField(choiceData.ChoiceTypes[index], "Condition :",
                    callback => { choiceData.ChoiceTypes[index] = (SSChoiceType)callback.newValue; });

                Button deleteConditionButton = ElementUtility.CreateButton("X", () =>
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

            for (int index = 0; index < choiceData.ChoiceTypes.Count; index++)
            {
                VisualElement choiceConditionsDataContainer = new();

                var choiceIndex = index;

                EnumField enumField = ElementUtility.CreateEnumField(choiceData.ChoiceTypes[choiceIndex], "Condition :",
                    callback => { choiceData.ChoiceTypes[choiceIndex] = (SSChoiceType)callback.newValue; });

                Button deleteConditionButton = ElementUtility.CreateButton("X", () =>
                {
                    if (choiceData.ChoiceTypes.Count == 1)
                    {
                        return;
                    }

                    choiceData.ChoiceTypes.RemoveAt(choiceIndex);
                    choiceConditionsFoldout.Remove(choiceConditionsDataContainer);
                });

                deleteConditionButton.AddToClassList("ss-node__button");

                choiceConditionsDataContainer.Add(deleteConditionButton);
                choiceConditionsDataContainer.Add(enumField);
                choiceConditionsFoldout.Add(choiceConditionsDataContainer);
            }

            customDataContainer.Insert(Choices.IndexOf(choiceData), choiceConditionsFoldout);

            Button deleteChoiceButton = ElementUtility.CreateButton("X", () =>
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

            TextField choiceTextField = ElementUtility.CreateTextField(choiceData.Text, null, callback =>
            {
                choiceData.Text = callback.newValue;
                choiceConditionsFoldout.text = $"\"{callback.newValue}\" :";
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