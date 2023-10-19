using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace SS.Elements
{
    using Enumerations;
    using Utilities;

    public class SSMultipleChoiceNode : SSNode
    {
        public override void Initialize(Vector2 position)
        {
            base.Initialize(position);

            NodeType = SSNodeType.MultipleChoice;

            Choices.Add("New Choice");
        }

        public override void Draw()
        {
            base.Draw();

            /* MAIN CONTAINER */

            Button addChoiceButton = SSElementUtility.CreateButton("Add Choice", () =>
            {
                Port choicePort = CreateChoicePort("New Choice");

                Choices.Add("New Choice");

                outputContainer.Add(choicePort);
            });

            addChoiceButton.AddToClassList("ss-node__button");

            mainContainer.Insert(1, addChoiceButton);

            /* OUTPUT CONTAINER */

            foreach (string choice in Choices)
            {
                Port choicePort = CreateChoicePort(choice);

                outputContainer.Add(choicePort);
            }

            RefreshExpandedState();
        }

        #region Elements Creation

        private Port CreateChoicePort(string choice)
        {
            Port choicePort = this.CreatePort();

            Button deleteChoiceButton = SSElementUtility.CreateButton("X");

            deleteChoiceButton.AddToClassList("ss-node__button");

            TextField choiceTextField = SSElementUtility.CreateTextField(choice);

            choiceTextField.AddClasses("ss-node__text-field", "ss-node__choice-text-field", "ss-node__text-field__hidden");

            choicePort.Add(choiceTextField);
            choicePort.Add(deleteChoiceButton);
            return choicePort;
        }

        #endregion
    }
}