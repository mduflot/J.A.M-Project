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
        public string DescriptionTask { get; set; }
        public SSTaskStatus TaskStatus { get; set; }
        public SSTaskType TaskType { get; set; }
        public Sprite TaskIcon { get; set; }
        public float TimeLeft { get; set; }
        public float BaseDuration { get; set; }
        public int MandatorySlots { get; set; }
        public int OptionalSlots { get; set; }
        public float TaskHelpFactor { get; set; }
        public RoomType Room { get; set; }
        public FurnitureType Furniture { get; set; }
        public bool IsTaskTutorial { get; set; }

        private VisualElement customDataContainer = new();

        public override void Initialize(string nodeName, SSGraphView ssGraphView, Vector2 position)
        {
            base.Initialize(nodeName, ssGraphView, position);

            NodeType = SSNodeType.Task;
            DescriptionTask = "Description";
            TaskIcon = null;
            TimeLeft = 0f;
            BaseDuration = 0f;
            MandatorySlots = 1;
            OptionalSlots = 0;
            TaskHelpFactor = 0.75f;
            Room = RoomType.Trajectory;
            Furniture = FurnitureType.Console;
            IsTaskTutorial = false;

            SSChoiceTaskSaveData firstChoiceData = new SSChoiceTaskSaveData()
            {
                Text = "FirstChoice",
                PreviewOutcome = "Preview..."
            };

            Choices.Add(firstChoiceData);
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
                    Text = "New Choice"
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

            Foldout textFoldout = SSElementUtility.CreateFoldout("Description :");

            TextField textTextField =
                SSElementUtility.CreateTextArea(DescriptionTask, null,
                    callback => { DescriptionTask = callback.newValue; });

            textTextField.AddClasses("ss-node__text-field", "ss-node__quote-text-field");

            textFoldout.Add(textTextField);

            customDataContainer.Add(textFoldout);

            EnumField statusTypeEnumField = SSElementUtility.CreateEnumField(TaskStatus, "Status Type :",
                callback => { TaskStatus = (SSTaskStatus)callback.newValue; });

            customDataContainer.Add(statusTypeEnumField);

            EnumField taskTypeEnumField = SSElementUtility.CreateEnumField(TaskType, "Task Type :",
                callback => { TaskType = (SSTaskType)callback.newValue; });

            customDataContainer.Add(taskTypeEnumField);

            ObjectField iconObjectField = SSElementUtility.CreateObjectField(TaskIcon, typeof(Sprite), "Task Icon :",
                callback => { TaskIcon = (Sprite)callback.newValue; });

            customDataContainer.Add(iconObjectField);

            FloatField timeLeftFloatField = SSElementUtility.CreateFloatField(TimeLeft, "Time Left :",
                callback => { TimeLeft = callback.newValue; });

            customDataContainer.Add(timeLeftFloatField);

            FloatField baseDurationFloatField = SSElementUtility.CreateFloatField(BaseDuration, "Duration :",
                callback => { BaseDuration = callback.newValue; });

            customDataContainer.Add(baseDurationFloatField);

            IntegerField mandatorySlotsIntegerField = SSElementUtility.CreateIntegerField(MandatorySlots,
                "Leader Slots :",
                callback => { MandatorySlots = callback.newValue; });

            customDataContainer.Add(mandatorySlotsIntegerField);

            IntegerField optionalSlotsIntegerField = SSElementUtility.CreateIntegerField(OptionalSlots,
                "Assistant Slots :",
                callback => { OptionalSlots = callback.newValue; });

            customDataContainer.Add(optionalSlotsIntegerField);

            FloatField taskHelpFactorFloatField = SSElementUtility.CreateFloatField(TaskHelpFactor,
                "Task Help Factor :",
                callback => { TaskHelpFactor = callback.newValue; });

            customDataContainer.Add(taskHelpFactorFloatField);

            EnumField roomEnumField = SSElementUtility.CreateEnumField(Room, "Room :",
                callback => { Room = (RoomType)callback.newValue; });

            customDataContainer.Add(roomEnumField);

            EnumField furnitureEnumField = SSElementUtility.CreateEnumField(Furniture, "Furniture :",
                callback => { Furniture = (FurnitureType)callback.newValue; });

            customDataContainer.Add(furnitureEnumField);

            Toggle isTaskTutorialToggle = SSElementUtility.CreateToggle(IsTaskTutorial, "Is Task Tutorial :",
                callback => { IsTaskTutorial = callback.newValue; });

            customDataContainer.Add(isTaskTutorialToggle);

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

            Foldout choiceFoldout = SSElementUtility.CreateFoldout($"{choiceData.Text} :");

            ObjectField conditionField = SSElementUtility.CreateObjectField(choiceData.Condition, typeof(ConditionSO),
                "Condition :", callback => { choiceData.Condition = (ConditionSO)callback.newValue; });

            choiceFoldout.Add(conditionField);

            Foldout previewOutcomeFoldout = SSElementUtility.CreateFoldout("Preview Outcome :");

            TextField previewOutcomeTextField = SSElementUtility.CreateTextField(choiceData.PreviewOutcome, null,
                callback => { choiceData.PreviewOutcome = callback.newValue; });

            previewOutcomeTextField.AddClasses("ss-node__text-field", "ss-node__quote-text-field");

            previewOutcomeFoldout.Add(previewOutcomeTextField);

            choiceFoldout.Add(previewOutcomeFoldout);

            customDataContainer.Insert(Choices.IndexOf(choiceData), choiceFoldout);

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
                customDataContainer.Remove(choiceFoldout);
            });

            deleteChoiceButton.AddToClassList("ss-node__button");

            TextField choiceTextField = SSElementUtility.CreateTextField(choiceData.Text, null, callback =>
            {
                choiceData.Text = callback.newValue;
                choiceFoldout.text = $"{callback.newValue} :";
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