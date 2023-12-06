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
            Room = RoomType.Flight;

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

            Button addChoiceButton = ElementUtility.CreateButton("Add Choice", () =>
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

            Foldout textFoldout = ElementUtility.CreateFoldout("Description :");

            TextField textTextField =
                ElementUtility.CreateTextArea(DescriptionTask, null,
                    callback => { DescriptionTask = callback.newValue; });

            textTextField.AddClasses("ss-node__text-field", "ss-node__quote-text-field");

            textFoldout.Add(textTextField);

            customDataContainer.Add(textFoldout);

            EnumField statusTypeEnumField = ElementUtility.CreateEnumField(TaskStatus, "Status Type :",
                callback => { TaskStatus = (SSTaskStatus)callback.newValue; });

            customDataContainer.Add(statusTypeEnumField);

            EnumField taskTypeEnumField = ElementUtility.CreateEnumField(TaskType, "Task Type :",
                callback => { TaskType = (SSTaskType)callback.newValue; });

            customDataContainer.Add(taskTypeEnumField);

            ObjectField iconObjectField = ElementUtility.CreateObjectField(TaskIcon, typeof(Sprite), "Task Icon :",
                callback => { TaskIcon = (Sprite)callback.newValue; });

            customDataContainer.Add(iconObjectField);

            FloatField timeLeftFloatField = ElementUtility.CreateFloatField(TimeLeft, "Time Left :",
                callback => { TimeLeft = callback.newValue; });

            customDataContainer.Add(timeLeftFloatField);

            FloatField baseDurationFloatField = ElementUtility.CreateFloatField(BaseDuration, "Duration :",
                callback => { BaseDuration = callback.newValue; });

            customDataContainer.Add(baseDurationFloatField);

            IntegerField mandatorySlotsIntegerField = ElementUtility.CreateIntegerField(MandatorySlots,
                "Leader Slots :",
                callback => { MandatorySlots = callback.newValue; });

            customDataContainer.Add(mandatorySlotsIntegerField);

            IntegerField optionalSlotsIntegerField = ElementUtility.CreateIntegerField(OptionalSlots,
                "Assistant Slots :",
                callback => { OptionalSlots = callback.newValue; });

            customDataContainer.Add(optionalSlotsIntegerField);

            FloatField taskHelpFactorFloatField = ElementUtility.CreateFloatField(TaskHelpFactor, "Task Help Factor :",
                callback => { TaskHelpFactor = callback.newValue; });

            customDataContainer.Add(taskHelpFactorFloatField);

            EnumField roomEnumField = ElementUtility.CreateEnumField(Room, "Room :",
                callback => { Room = (RoomType)callback.newValue; });

            customDataContainer.Add(roomEnumField);

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

            Foldout choiceFoldout = ElementUtility.CreateFoldout($"{choiceData.Text} :");

            ObjectField conditionField = ElementUtility.CreateObjectField(choiceData.Condition, typeof(ConditionSO),
                "Condition :", callback => { choiceData.Condition = (ConditionSO)callback.newValue; });

            choiceFoldout.Add(conditionField);

            Foldout previewOutcomeFoldout = ElementUtility.CreateFoldout("Preview Outcome :");

            TextField previewOutcomeTextField = ElementUtility.CreateTextField(choiceData.PreviewOutcome, null,
                callback => { choiceData.PreviewOutcome = callback.newValue; });

            previewOutcomeTextField.AddClasses("ss-node__text-field", "ss-node__quote-text-field");

            previewOutcomeFoldout.Add(previewOutcomeTextField);

            choiceFoldout.Add(previewOutcomeFoldout);

            customDataContainer.Insert(Choices.IndexOf(choiceData), choiceFoldout);

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

                Choices.Remove(choiceData);

                graphView.RemoveElement(choicePort);
                customDataContainer.Remove(choiceFoldout);
            });

            deleteChoiceButton.AddToClassList("ss-node__button");

            TextField choiceTextField = ElementUtility.CreateTextField(choiceData.Text, null, callback =>
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