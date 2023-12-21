using SS.Enumerations;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SS.Elements
{
    using Data.Save;
    using Utilities;
    using Windows;

    public class SSSoundNode : SSNode
    {
        public AudioClip AudioClip { get; set; }
        
        public override void Initialize(string nodeName, SSGraphView ssGraphView, Vector2 position)
        {
            base.Initialize(nodeName, ssGraphView, position);
            
            NodeType = SSNodeType.Sound;
            
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

            VisualElement customDataContainer = new();

            customDataContainer.AddToClassList("ss-node__custom-data-container");
            
            ObjectField objectField = SSElementUtility.CreateObjectField(AudioClip, typeof(AudioClip), "Audio Clip:", callback =>
            {
                AudioClip = (AudioClip) callback.newValue;
            });
            
            customDataContainer.Add(objectField);
            
            extensionContainer.Add(customDataContainer);
            
            RefreshExpandedState();
        }
    }
}