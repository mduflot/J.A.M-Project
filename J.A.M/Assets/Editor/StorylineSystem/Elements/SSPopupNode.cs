using SS.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SS.Elements
{
    using Data.Save;
    using Enumerations;
    using Windows;

    public class SSPopupNode : SSNode
    {
        public string Text { get; set; }

        public SSPopupUIType PopupUIType { get; set; }

        public override void Initialize(string nodeName, SSGraphView ssGraphView, Vector2 position)
        {
            base.Initialize(nodeName, ssGraphView, position);

            NodeType = SSNodeType.Popup;
            Text = "Node text.";

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

            Foldout textFoldout = SSElementUtility.CreateFoldout("Popup");

            TextField textTextField =
                SSElementUtility.CreateTextArea(Text, null, callback => { Text = callback.newValue; });

            textTextField.AddClasses("ss-node__text-field", "ss-node__quote-text-field");

            textFoldout.Add(textTextField);

            customDataContainer.Add(textFoldout);

            EnumFlagsField popupEnumFlagsField = SSElementUtility.CreateEnumFlagsField(PopupUIType, "UI",
                callbackUI => { PopupUIType = (SSPopupUIType)callbackUI.newValue; });

            customDataContainer.Add(popupEnumFlagsField);
            
            extensionContainer.Add(customDataContainer);
            
            RefreshExpandedState();
        }
    }
}