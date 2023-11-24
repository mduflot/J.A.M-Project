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
        public Sprite TaskIcon { get; set; }
        public float TimeLeft { get; set; }
        public float BaseDuration { get; set; }
        public int MandatorySlots { get; set; }
        public int OptionalSlots { get; set; }
        public float TaskHelpFactor { get; set; }
        public SpaceshipManager.ShipRooms Room { get; set; }
        public bool IsPermanent { get; set; }

        private VisualElement customDataContainer = new();

        public override void Initialize(string nodeName, SSGraphView ssGraphView, Vector2 position)
        {
            base.Initialize(nodeName, ssGraphView, position);

            NodeType = SSNodeType.Task;
            DescriptionTask = "Description Task";
            TaskIcon = null;
            TimeLeft = 0f;
            BaseDuration = 0f;
            MandatorySlots = 1;
            OptionalSlots = 0;
            TaskHelpFactor = 0.75f;
            Room = SpaceshipManager.ShipRooms.Bedrooms;
            IsPermanent = false;

            SSChoiceTaskSaveData firstChoiceData = new SSChoiceTaskSaveData()
            {
                Text = "FirstChoice",
                Jobs = TraitsData.Job.None,
                PositiveTraits = TraitsData.PositiveTraits.None,
                NegativeTraits = TraitsData.NegativeTraits.None
            };

            SSChoiceTaskSaveData lastChoiceData = new SSChoiceTaskSaveData()
            {
                Text = "LastChoice",
                Jobs = TraitsData.Job.None,
                PositiveTraits = TraitsData.PositiveTraits.None,
                NegativeTraits = TraitsData.NegativeTraits.None
            };

            Choices.Add(firstChoiceData);
            Choices.Add(lastChoiceData);
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
                    Jobs = TraitsData.Job.None,
                    PositiveTraits = TraitsData.PositiveTraits.None,
                    NegativeTraits = TraitsData.NegativeTraits.None
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

            Foldout textFoldout = ElementUtility.CreateFoldout("Description Task :");

            TextField textTextField =
                ElementUtility.CreateTextArea(DescriptionTask, null,
                    callback => { DescriptionTask = callback.newValue; });

            textTextField.AddClasses("ss-node__text-field", "ss-node__quote-text-field");

            textFoldout.Add(textTextField);

            customDataContainer.Add(textFoldout);

            ObjectField iconObjectField = ElementUtility.CreateObjectField(TaskIcon, typeof(Sprite), "Task Icon :",
                callback => { TaskIcon = (Sprite)callback.newValue; });

            customDataContainer.Add(iconObjectField);

            FloatField timeLeftFloatField = ElementUtility.CreateFloatField(TimeLeft, "Time Left :",
                callback => { TimeLeft = callback.newValue; });

            customDataContainer.Add(timeLeftFloatField);

            FloatField baseDurationFloatField = ElementUtility.CreateFloatField(BaseDuration, "Base Duration :",
                callback => { BaseDuration = callback.newValue; });

            customDataContainer.Add(baseDurationFloatField);

            IntegerField mandatorySlotsIntegerField = ElementUtility.CreateIntegerField(MandatorySlots,
                "Mandatory Slots :",
                callback => { MandatorySlots = callback.newValue; });

            customDataContainer.Add(mandatorySlotsIntegerField);

            IntegerField optionalSlotsIntegerField = ElementUtility.CreateIntegerField(OptionalSlots,
                "Optional Slots :",
                callback => { OptionalSlots = callback.newValue; });

            customDataContainer.Add(optionalSlotsIntegerField);

            FloatField taskHelpFactorFloatField = ElementUtility.CreateFloatField(TaskHelpFactor, "Task Help Factor :",
                callback => { TaskHelpFactor = callback.newValue; });

            customDataContainer.Add(taskHelpFactorFloatField);

            EnumField roomEnumField = ElementUtility.CreateEnumField(Room, "Room :",
                callback => { Room = (SpaceshipManager.ShipRooms)callback.newValue; });

            customDataContainer.Add(roomEnumField);

            Toggle isPermanentToggle = ElementUtility.CreateToggle(IsPermanent, "Is Permanent :",
                callback => { IsPermanent = callback.newValue; });

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

            Foldout choiceFoldout = ElementUtility.CreateFoldout($"{choiceData.Text} :");

            EnumFlagsField jobEnumFlagsField = ElementUtility.CreateEnumFlagsField(choiceData.Jobs, "Jobs :",
                callback => { choiceData.Jobs = (TraitsData.Job)callback.newValue; });

            choiceFoldout.Add(jobEnumFlagsField);

            EnumFlagsField positiveTraitsEnumFlagsField = ElementUtility.CreateEnumFlagsField(choiceData.PositiveTraits,
                "Positive Traits :",
                callback => { choiceData.PositiveTraits = (TraitsData.PositiveTraits)callback.newValue; });

            choiceFoldout.Add(positiveTraitsEnumFlagsField);

            EnumFlagsField negativeTraitsEnumFlagsField = ElementUtility.CreateEnumFlagsField(choiceData.NegativeTraits,
                "Negative Traits :",
                callback => { choiceData.NegativeTraits = (TraitsData.NegativeTraits)callback.newValue; });

            choiceFoldout.Add(negativeTraitsEnumFlagsField);

            // TODO : Add outcomes

            customDataContainer.Insert(Choices.IndexOf(choiceData), choiceFoldout);

            Button deleteChoiceButton = ElementUtility.CreateButton("X", () =>
            {
                if (Choices.Count == 2)
                {
                    return;
                }

                if (choicePort.connected)
                {
                    graphView.DeleteElements(choicePort.connections);
                }

                // TODO: Remove traits from choiceData
                // choiceData.Traits.Clear();
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