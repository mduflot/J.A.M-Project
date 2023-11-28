using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SS.Elements
{
    using Data.Save;
    using Enumerations;
    using ScriptableObjects;
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
        public string PreviewOutcome { get; set; }

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
            Room = SpaceshipManager.ShipRooms.Bedrooms;
            IsPermanent = false;

            SSChoiceTaskSaveData firstChoiceData = new SSChoiceTaskSaveData()
            {
                Text = "FirstChoice",
                Jobs = TraitsData.Job.None,
                PositiveTraits = TraitsData.PositiveTraits.None,
                NegativeTraits = TraitsData.NegativeTraits.None,
                IsUnlockStoryline = false,
                IsUnlockTimeline = false,
                StatusNodeContainers = new List<SerializableTuple<SSStoryStatus, SSNodeContainerSO>>(),
                StatusNodeGroups = new List<SerializableTuple<SSStoryStatus, SSNodeGroupSO>>()
            };

            SSChoiceTaskSaveData lastChoiceData = new SSChoiceTaskSaveData()
            {
                Text = "LastChoice",
                Jobs = TraitsData.Job.None,
                PositiveTraits = TraitsData.PositiveTraits.None,
                NegativeTraits = TraitsData.NegativeTraits.None,
                IsUnlockStoryline = false,
                IsUnlockTimeline = false,
                StatusNodeContainers = new List<SerializableTuple<SSStoryStatus, SSNodeContainerSO>>(),
                StatusNodeGroups = new List<SerializableTuple<SSStoryStatus, SSNodeGroupSO>>()
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

            Foldout textFoldout = ElementUtility.CreateFoldout("Description :");

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
                callback => { Room = (SpaceshipManager.ShipRooms)callback.newValue; });

            customDataContainer.Add(roomEnumField);

            Toggle isPermanentToggle = ElementUtility.CreateToggle(IsPermanent, "Is Permanent :",
                callback => { IsPermanent = callback.newValue; });

            customDataContainer.Add(isPermanentToggle);

            Foldout previewOutcomeFoldout = ElementUtility.CreateFoldout("Preview Outcome :");

            TextField previewOutcomeTextField = ElementUtility.CreateTextField(PreviewOutcome, null,
                callback => { PreviewOutcome = callback.newValue; });

            previewOutcomeTextField.AddClasses("ss-node__text-field", "ss-node__quote-text-field");

            previewOutcomeFoldout.Add(previewOutcomeTextField);

            customDataContainer.Add(previewOutcomeFoldout);

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

            // TODO : Insert at the right index (not at the end)
            
            ListView listViewStoryline = null;
            ListView listViewTimeline = null;

            Toggle isUnlockStorylineToggle = ElementUtility.CreateToggle(choiceData.IsUnlockStoryline,
                "Is Unlock Storyline :",
                callback =>
                {
                    choiceData.IsUnlockStoryline = callback.newValue;
                    if (callback.newValue)
                    {
                        listViewStoryline =
                            ElementUtility.CreateListViewEnumObjectField(choiceData.StatusNodeContainers,
                                "Node Containers :");
                        choiceFoldout.Insert(4, listViewStoryline);
                    }
                    else
                    {
                        choiceData.StatusNodeContainers.Clear();
                        choiceFoldout.Remove(listViewStoryline);
                    }
                });

            choiceFoldout.Add(isUnlockStorylineToggle);

            Toggle isUnlockTimelineToggle = ElementUtility.CreateToggle(choiceData.IsUnlockTimeline,
                "Is Unlock Timeline :",
                callback =>
                {
                    choiceData.IsUnlockTimeline = callback.newValue;
                    if (callback.newValue)
                    {
                        listViewTimeline =
                            ElementUtility.CreateListViewEnumObjectField(choiceData.StatusNodeGroups, "Node Groups :");
                        choiceFoldout.Add(listViewTimeline);
                    }
                    else
                    {
                        choiceData.StatusNodeGroups.Clear();
                        choiceFoldout.Remove(listViewTimeline);
                    }
                });

            choiceFoldout.Add(isUnlockTimelineToggle);

            if (choiceData.IsUnlockStoryline)
            {
                listViewStoryline =
                    ElementUtility.CreateListViewEnumObjectField(choiceData.StatusNodeContainers, "Node Containers :");
                choiceFoldout.Insert(4, listViewStoryline);
            }

            if (choiceData.IsUnlockTimeline)
            {
                listViewTimeline =
                    ElementUtility.CreateListViewEnumObjectField(choiceData.StatusNodeGroups, "Node Groups :");
                choiceFoldout.Add(listViewTimeline);
            }
            
            // TODO : Add conditions foldout
            // TODO : Add addButton and deleteButton to conditions foldout
            Button addConditionButton = ElementUtility.CreateButton("Add Condition", () =>
            {
                // TODO : Add condition
                // TODO : Create method for adding conditions & outcomes
            });

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

                choiceData.Jobs = TraitsData.Job.None;
                choiceData.PositiveTraits = TraitsData.PositiveTraits.None;
                choiceData.NegativeTraits = TraitsData.NegativeTraits.None;
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