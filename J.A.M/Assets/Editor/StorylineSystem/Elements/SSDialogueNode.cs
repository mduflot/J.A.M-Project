using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace SS.Elements
{
    using Data.Save;
    using Enumerations;
    using Windows;

    public class SSDialogueNode : SSNode
    {
        public string Text { get; set; }
        public SSSpeakerType SpeakerType { get; set; }
        public uint Duration { get; set; }
        public bool IsDialogueTask { get; set; }
        public int PercentageTask { get; set; }

        public override void Initialize(string nodeName, SSGraphView ssGraphView, Vector2 position)
        {
            base.Initialize(nodeName, ssGraphView, position);

            NodeType = SSNodeType.DIALOGUE;
            Text = "Node text.";
            Duration = 1;
            IsDialogueTask = false;
            PercentageTask = 50;

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

            customDataContainer.AddToClassList("ss-node__custom-data-container");

            Foldout textFoldout = ElementUtility.CreateFoldout("Dialogue");

            TextField textTextField =
                ElementUtility.CreateTextArea(Text, null, callback => { Text = callback.newValue; });

            textTextField.AddClasses("ss-node__text-field", "ss-node__quote-text-field");

            textFoldout.Add(textTextField);

            customDataContainer.Add(textFoldout);

            EnumField enumField = ElementUtility.CreateEnumField(SpeakerType, "Speaker",
                callback => { SpeakerType = (SSSpeakerType)callback.newValue; });

            customDataContainer.Add(enumField);

            UnsignedIntegerField unsignedIntegerField = ElementUtility.CreateUnsignedIntegerField(Duration,
                "Duration", callback => { Duration = callback.newValue; });

            customDataContainer.Add(unsignedIntegerField);

            SliderInt sliderInt = null;

            Toggle toggle = ElementUtility.CreateToggle(IsDialogueTask, "DialogueTask", callback =>
            {
                IsDialogueTask = callback.newValue;
                if (callback.newValue)
                {
                    sliderInt = ElementUtility.CreateSliderIntField(PercentageTask, "PercentageTask : ", 0, 100,
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
                sliderInt = ElementUtility.CreateSliderIntField(PercentageTask, "PercentageTask : ", 0, 100,
                    callback =>
                    {
                        PercentageTask = callback.newValue;
                        sliderInt.label = "PercentageTask : " + callback.newValue;
                    });

                customDataContainer.Add(sliderInt);
            }

            extensionContainer.Add(customDataContainer);

            RefreshExpandedState();
        }
    }
}