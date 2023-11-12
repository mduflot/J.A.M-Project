using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace SS.Elements
{
    using Windows;
    using Data.Save;
    using Enumerations;

    public class SSRewardNode : SSNode
    {
        public List<SSRewardType> RewardTypes { get; set; }

        public override void Initialize(string nodeName, SSGraphView ssGraphView, Vector2 position)
        {
            base.Initialize(nodeName, ssGraphView, position);

            NodeType = SSNodeType.Reward;
            RewardTypes = new List<SSRewardType>()
            {
                SSRewardType.Money
            };

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

            Button addRewardButton = ElementUtility.CreateButton("Add Reward", () =>
            {
                RewardTypes.Add(SSRewardType.Money);
                VisualElement rewardDataContainer = new();

                var index = RewardTypes.Count - 1;
                EnumField enumField = ElementUtility.CreateEnumField(RewardTypes[index], "Reward :", callback =>
                {
                    RewardTypes[index] = (SSRewardType)callback.newValue;
                });

                Button removeRewardButton = ElementUtility.CreateButton("X", () =>
                {
                    if (RewardTypes.Count == 1)
                    {
                        return;
                    }

                    RewardTypes.RemoveAt(RewardTypes.Count - 1);
                    customDataContainer.Remove(rewardDataContainer);
                });

                removeRewardButton.AddToClassList("ss-node__button");

                rewardDataContainer.Add(removeRewardButton);
                rewardDataContainer.Add(enumField);
                customDataContainer.Add(rewardDataContainer);
            });

            addRewardButton.AddToClassList("ss-node__button");

            customDataContainer.Add(addRewardButton);

            for (var index = 0; index < RewardTypes.Count; index++)
            {
                VisualElement rewardDataContainer = new VisualElement();

                var rewardIndex = index;
                
                EnumField enumField = ElementUtility.CreateEnumField(RewardTypes[rewardIndex], "Reward", callback =>
                {
                    RewardTypes[rewardIndex] = (SSRewardType)callback.newValue;
                });

                Button removeRewardButton = ElementUtility.CreateButton("X", () =>
                {
                    if (RewardTypes.Count == 1)
                    {
                        return;
                    }

                    RewardTypes.RemoveAt(rewardIndex);
                    customDataContainer.Remove(rewardDataContainer);
                });

                removeRewardButton.AddToClassList("ss-node__button");

                rewardDataContainer.Add(removeRewardButton);
                rewardDataContainer.Add(enumField);
                customDataContainer.Add(rewardDataContainer);
            }

            extensionContainer.Add(customDataContainer);

            RefreshExpandedState();
        }
    }
}