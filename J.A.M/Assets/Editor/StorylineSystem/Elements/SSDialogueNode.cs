using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace SS.Elements
{
    using Data.Save;
    using Enumerations;
    using Utilities;
    using Windows;

    public class SSDialogueNode : SSNode
    {
        public SSDialogueType DialogueType { get; set; }
        public string Text { get; set; }
        public SSSpeakerType SpeakerType { get; set; }
        public float Duration { get; set; }
        public SSBarkType BarkType { get; set; }
        public bool IsDialogueTask { get; set; }
        public int PercentageTask { get; set; }
        public TraitsData.Job Job { get; set; }
        public TraitsData.PositiveTraits PositiveTraits { get; set; }
        public TraitsData.NegativeTraits NegativeTraits { get; set; }

        public override void Initialize(string nodeName, SSGraphView ssGraphView, Vector2 position)
        {
            base.Initialize(nodeName, ssGraphView, position);

            NodeType = SSNodeType.Dialogue;
            DialogueType = SSDialogueType.Dialogue;
            Text = "Node text.";
            Duration = 1.0f;
            BarkType = SSBarkType.Awaiting;
            IsDialogueTask = false;
            PercentageTask = 50;
            Job = TraitsData.Job.None;
            PositiveTraits = TraitsData.PositiveTraits.None;
            NegativeTraits = TraitsData.NegativeTraits.None;

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
            VisualElement dialogueDataContainer = new VisualElement();
            VisualElement barkDataContainer = new VisualElement();

            customDataContainer.AddToClassList("ss-node__custom-data-container");

            EnumField enumFieldDialogueType = SSElementUtility.CreateEnumField(DialogueType, "Dialogue Type",
                callback =>
                {
                    DialogueType = (SSDialogueType)callback.newValue;
                    if ((SSDialogueType)callback.newValue == SSDialogueType.Dialogue)
                    {
                        dialogueDataContainer = DrawDialogueType();
                        customDataContainer.Add(dialogueDataContainer);
                        if (customDataContainer.Contains(barkDataContainer))
                            customDataContainer.Remove(barkDataContainer);
                    }
                    else
                    {
                        barkDataContainer = DrawBarkType();
                        customDataContainer.Add(barkDataContainer);
                        if (customDataContainer.Contains(dialogueDataContainer))
                            customDataContainer.Remove(dialogueDataContainer);
                    }
                });

            if (DialogueType == SSDialogueType.Dialogue)
            {
                dialogueDataContainer = DrawDialogueType();
                customDataContainer.Add(dialogueDataContainer);
            }
            else
            {
                barkDataContainer = DrawBarkType();
                customDataContainer.Add(barkDataContainer);
            }

            customDataContainer.Insert(0, enumFieldDialogueType);

            extensionContainer.Add(customDataContainer);

            RefreshExpandedState();
        }

        private VisualElement DrawDialogueType()
        {
            VisualElement customDataContainer = new VisualElement();

            Foldout textFoldout = SSElementUtility.CreateFoldout("Dialogue");

            TextField textTextField =
                SSElementUtility.CreateTextArea(Text, null, callback => { Text = callback.newValue; });

            textTextField.AddClasses("ss-node__text-field", "ss-node__quote-text-field");

            textFoldout.Add(textTextField);

            customDataContainer.Add(textFoldout);

            FloatField unsignedIntegerField = SSElementUtility.CreateFloatField(Duration,
                "Duration", callback => { Duration = callback.newValue; });

            customDataContainer.Add(unsignedIntegerField);

            EnumField enumFieldJob = null;
            EnumField enumFieldPositiveTraits = null;
            EnumField enumFieldNegativeTraits = null;

            EnumField enumField = SSElementUtility.CreateEnumField(SpeakerType, "Speaker",
                callback =>
                {
                    SpeakerType = (SSSpeakerType)callback.newValue;
                    if (SpeakerType == SSSpeakerType.Traits)
                    {
                        enumFieldJob = SSElementUtility.CreateEnumField(Job, "Job",
                            callbackJob => { Job = (TraitsData.Job)callbackJob.newValue; });
                        enumFieldPositiveTraits = SSElementUtility.CreateEnumField(PositiveTraits,
                            "Positive Traits",
                            callbackPositiveTraits =>
                            {
                                PositiveTraits = (TraitsData.PositiveTraits)callbackPositiveTraits.newValue;
                            });
                        enumFieldNegativeTraits = SSElementUtility.CreateEnumField(NegativeTraits, "Negative Traits",
                            callbackNegativeTraits =>
                            {
                                NegativeTraits = (TraitsData.NegativeTraits)callbackNegativeTraits.newValue;
                            });

                        customDataContainer.Add(enumFieldJob);
                        customDataContainer.Add(enumFieldPositiveTraits);
                        customDataContainer.Add(enumFieldNegativeTraits);
                    }
                    else
                    {
                        if (customDataContainer.Contains(enumFieldJob)) customDataContainer.Remove(enumFieldJob);
                        if (customDataContainer.Contains(enumFieldPositiveTraits))
                            customDataContainer.Remove(enumFieldPositiveTraits);
                        if (customDataContainer.Contains(enumFieldNegativeTraits))
                            customDataContainer.Remove(enumFieldNegativeTraits);
                    }
                });

            customDataContainer.Add(enumField);

            if (SpeakerType == SSSpeakerType.Traits)
            {
                enumFieldJob = SSElementUtility.CreateEnumField(Job, "Job",
                    callbackJob => { Job = (TraitsData.Job)callbackJob.newValue; });
                enumFieldPositiveTraits = SSElementUtility.CreateEnumField(PositiveTraits,
                    "Positive Traits",
                    callbackPositiveTraits =>
                    {
                        PositiveTraits = (TraitsData.PositiveTraits)callbackPositiveTraits.newValue;
                    });
                enumFieldNegativeTraits = SSElementUtility.CreateEnumField(NegativeTraits, "Negative Traits",
                    callbackNegativeTraits =>
                    {
                        NegativeTraits = (TraitsData.NegativeTraits)callbackNegativeTraits.newValue;
                    });

                customDataContainer.Add(enumFieldJob);
                customDataContainer.Add(enumFieldPositiveTraits);
                customDataContainer.Add(enumFieldNegativeTraits);
            }

            return customDataContainer;
        }

        private VisualElement DrawBarkType()
        {
            VisualElement customDataContainer = new VisualElement();

            EnumField enumFieldBarkType = SSElementUtility.CreateEnumField(BarkType, "Bark Type",
                callback => { BarkType = (SSBarkType)callback.newValue; });
            customDataContainer.Add(enumFieldBarkType);

            FloatField unsignedIntegerField = SSElementUtility.CreateFloatField(Duration,
                "Duration", callback => { Duration = callback.newValue; });
            customDataContainer.Add(unsignedIntegerField);

            SliderInt sliderInt = null;

            Toggle toggle = SSElementUtility.CreateToggle(IsDialogueTask, "DialogueTask", callback =>
            {
                IsDialogueTask = callback.newValue;
                if (callback.newValue)
                {
                    sliderInt = SSElementUtility.CreateSliderIntField(PercentageTask, "PercentageTask : ", 0, 100,
                        callback =>
                        {
                            PercentageTask = callback.newValue;
                            sliderInt.label = "PercentageTask : " + callback.newValue;
                        });

                    customDataContainer.Add(sliderInt);
                }
                else
                {
                    customDataContainer.Remove(sliderInt);
                }
            });

            customDataContainer.Add(toggle);

            if (IsDialogueTask)
            {
                sliderInt = SSElementUtility.CreateSliderIntField(PercentageTask, "PercentageTask : ", 0, 100,
                    callback =>
                    {
                        PercentageTask = callback.newValue;
                        sliderInt.label = "PercentageTask : " + callback.newValue;
                    });

                customDataContainer.Add(sliderInt);
            }

            return customDataContainer;
        }
    }
}