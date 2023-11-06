using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace SS
{
    using Enumerations;
    using ScriptableObjects;

    public class SSNode : MonoBehaviour
    {
        /* UI GameObjects */
        [SerializeField] private GameObject dialogueLayout;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private TextMeshProUGUI currentStoryline;
        [SerializeField] private SpaceshipManager spaceshipManager;

        private List<CharacterBehaviour> characters = new();
        private List<CharacterBehaviour> assignedCharacters = new();

        private List<Tuple<CharacterBehaviour, string>> dialogues;
        
        private SSTimeNodeSO timeNodeSO;
        private uint durationTimeNode;

        private SSDialogueNodeSO dialogueNodeSO;
        private uint durationDialogueNode;
        
        /* Node Scriptable Objects */
        [SerializeField] private SSNodeContainerSO nodeContainer;
        [SerializeField] private SSNodeGroupSO nodeGroup;
        [SerializeField] private SSNodeSO node;

        /* Filters */
        [SerializeField] private bool groupedNodes;
        [SerializeField] private bool startingNodesOnly;

        /* Indexes */
        [SerializeField] private int selectedNodeGroupIndex;
        [SerializeField] private int selectedNodeIndex;

        public void StartTimeline()
        {
            characters.AddRange(spaceshipManager.characters);
            dialogues = new();
            currentStoryline.text = nodeContainer.name;
            CheckNodeType(node);
        }

        public void CheckNodeType(SSNodeSO nodeSO)
        {
            switch (nodeSO.NodeType)
            {
                case SSNodeType.Dialogue:
                {
                    RunNode(nodeSO as SSDialogueNodeSO);
                    break;
                }
                case SSNodeType.Task:
                {
                    RunNode(nodeSO as SSTaskNodeSO);
                    break;
                }
                case SSNodeType.Reward:
                {
                    RunNode(nodeSO as SSRewardNodeSO);
                    break;
                }
                case SSNodeType.Time:
                {
                    RunNode(nodeSO as SSTimeNodeSO);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void RunNode(SSRewardNodeSO nodeSO)
        {
            if (nodeSO.Choices.First().NextNode == null)
            {
                return;
            }
            CheckNodeType(nodeSO.Choices.First().NextNode);
        }

        private void RunNode(SSDialogueNodeSO nodeSO)
        {
            dialogueLayout.SetActive(true);
            dialogueText.gameObject.SetActive(true);
            dialogueText.text = nodeSO.Text;
            // dialogues.Add(new Tuple<CharacterBehaviour, string>(characters[0], nodeSO.Text));
            dialogueNodeSO = nodeSO;
            durationDialogueNode = nodeSO.TimeToWait * TimeTickSystem.ticksPerHour;
            TimeTickSystem.OnTick += WaitingDialogue;
        }

        private void RunNode(SSTaskNodeSO nodeSO)
        {
            spaceshipManager.SpawnTask(nodeSO.TaskData);
            StartCoroutine(WaiterTask(nodeSO));
        }

        private void RunNode(SSTimeNodeSO nodeSO)
        {
            timeNodeSO = nodeSO;
            durationTimeNode = nodeSO.TimeToWait * TimeTickSystem.ticksPerHour;
            TimeTickSystem.OnTick += WaitingTime;
        }

        private void WaitingTime(object sender, TimeTickSystem.OnTickEventArgs e)
        {
            durationTimeNode -= TimeTickSystem.timePerTick;
            if (durationTimeNode <= 0)
            {
                if (timeNodeSO.Choices.First().NextNode == null)
                {
                    TimeTickSystem.OnTick -= WaitingTime;
                    return;
                }
                CheckNodeType(timeNodeSO.Choices.First().NextNode);
                TimeTickSystem.OnTick -= WaitingTime;
            }
        }

        private void WaitingDialogue(object sender, TimeTickSystem.OnTickEventArgs e)
        {
            durationDialogueNode -= TimeTickSystem.timePerTick;
            if (durationDialogueNode <= 0)
            {
                dialogueLayout.SetActive(false);
                dialogueText.gameObject.SetActive(false);
                if (dialogueNodeSO.Choices.First().NextNode == null)
                {
                    TimeTickSystem.OnTick -= WaitingDialogue;
                    return;
                }
                CheckNodeType(dialogueNodeSO.Choices.First().NextNode);
                TimeTickSystem.OnTick -= WaitingDialogue;
            }
        }

        IEnumerator WaiterTask(SSTaskNodeSO nodeSO)
        {
            yield return new WaitUntil(() => spaceshipManager.GetTaskNotification(nodeSO.TaskData).isCompleted);
            assignedCharacters.AddRange(spaceshipManager.GetTaskNotification(nodeSO.TaskData).LeaderCharacters);
            assignedCharacters.AddRange(spaceshipManager.GetTaskNotification(nodeSO.TaskData).AssistantCharacters);
            if (nodeSO.Choices.First().NextNode == null)
            {
                yield break;
            }
            CheckNodeType(nodeSO.Choices.First().NextNode);
        }
    }
}