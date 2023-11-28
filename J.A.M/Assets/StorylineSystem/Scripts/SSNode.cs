using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SS
{
    using Enumerations;
    using ScriptableObjects;

    public class SSNode : MonoBehaviour
    {
        /* UI GameObjects */
        [SerializeField] private TextMeshProUGUI currentStoryline;
        [SerializeField] private SpaceshipManager spaceshipManager;

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

        private List<CharacterBehaviour> characters = new();
        private List<CharacterBehaviour> assignedCharacters = new();
        private List<Tuple<Sprite, string, string>> dialogues;
        private SSTimeNodeSO timeNode;
        private uint durationTimeNode;
        private SSTaskNodeSO taskNode;

        public void StartTimeline()
        {
            dialogues = new();
            if (currentStoryline) currentStoryline.text = nodeContainer.name;
            CheckNodeType(node);
        }

        private void ResetTimeline()
        {
            dialogues.Clear();
            if (currentStoryline) currentStoryline.text = "No timeline";
        }

        /// <summary>
        /// Checks the type of the node and runs the appropriate function
        /// </summary>
        /// <param name="nodeSO"> Node you need to run </param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
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
                case SSNodeType.Time:
                {
                    RunNode(nodeSO as SSTimeNodeSO);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // REWORK : Refactor the speakerType
        // FIX : Problem with empty list if GD not set correctly
        private void RunNode(SSDialogueNodeSO nodeSO)
        {
            CharacterBehaviour actualSpeaker;
            List<CharacterBehaviour> tempCharacters;
            switch (nodeSO.SpeakerType)
            {
                case SSSpeakerType.Random:
                {
                    actualSpeaker = spaceshipManager.characters[Random.Range(0, spaceshipManager.characters.Length)];
                    dialogues.Add(new Tuple<Sprite, string, string>(actualSpeaker.GetCharacterData().characterIcon,
                        actualSpeaker.GetCharacterData().firstName, nodeSO.Text));
                    StartCoroutine(DisplayDialogue(actualSpeaker, nodeSO));
                    characters.Add(actualSpeaker);
                    break;
                }
                case SSSpeakerType.RandomOther:
                {
                    tempCharacters = spaceshipManager.characters.Except(characters).ToList();
                    actualSpeaker = tempCharacters[Random.Range(0, tempCharacters.Count)];
                    dialogues.Add(new Tuple<Sprite, string, string>(actualSpeaker.GetCharacterData().characterIcon,
                        actualSpeaker.GetCharacterData().firstName, nodeSO.Text));
                    StartCoroutine(DisplayDialogue(actualSpeaker, nodeSO));
                    characters.Add(actualSpeaker);
                    break;
                }
                case SSSpeakerType.Sensor:
                {
                    // TODO : Place on furniture / List of Furniture in spaceship
                    // dialogues.Add(new Tuple<Sprite, string, string>(null, "Sensor", nodeSO.Text));
                    break;
                }
                case SSSpeakerType.Expert:
                {
                    // TODO : Place on furniture / List of Furniture in spaceship
                    // dialogues.Add(new Tuple<Sprite, string, string>(null, "Expert", nodeSO.Text));
                    break;
                }
                case SSSpeakerType.Character1:
                {
                    if (characters[0])
                    {
                        dialogues.Add(new Tuple<Sprite, string, string>(characters[0].GetCharacterData().characterIcon,
                            characters[0].GetCharacterData().firstName, nodeSO.Text));
                        StartCoroutine(DisplayDialogue(characters[0], nodeSO));
                    }

                    break;
                }
                case SSSpeakerType.Character2:
                {
                    if (characters[1])
                    {
                        dialogues.Add(new Tuple<Sprite, string, string>(characters[1].GetCharacterData().characterIcon,
                            characters[1].GetCharacterData().firstName, nodeSO.Text));
                        StartCoroutine(DisplayDialogue(characters[1], nodeSO));
                    }

                    break;
                }
                case SSSpeakerType.Character3:
                {
                    if (characters[2])
                    {
                        dialogues.Add(new Tuple<Sprite, string, string>(characters[2].GetCharacterData().characterIcon,
                            characters[2].GetCharacterData().firstName, nodeSO.Text));
                        StartCoroutine(DisplayDialogue(characters[2], nodeSO));
                    }

                    break;
                }
                case SSSpeakerType.Character4:
                {
                    if (characters[3])
                    {
                        dialogues.Add(new Tuple<Sprite, string, string>(characters[3].GetCharacterData().characterIcon,
                            characters[3].GetCharacterData().firstName, nodeSO.Text));
                        StartCoroutine(DisplayDialogue(characters[3], nodeSO));
                    }

                    break;
                }
                case SSSpeakerType.Assigned1:
                {
                    if (assignedCharacters[0])
                    {
                        dialogues.Add(new Tuple<Sprite, string, string>(
                            assignedCharacters[0].GetCharacterData().characterIcon,
                            assignedCharacters[0].GetCharacterData().firstName, nodeSO.Text));
                        StartCoroutine(DisplayDialogue(assignedCharacters[0], nodeSO));
                    }

                    break;
                }
                case SSSpeakerType.Assigned2:
                {
                    if (assignedCharacters[1])
                    {
                        dialogues.Add(new Tuple<Sprite, string, string>(
                            assignedCharacters[1].GetCharacterData().characterIcon,
                            assignedCharacters[1].GetCharacterData().firstName, nodeSO.Text));
                        StartCoroutine(DisplayDialogue(assignedCharacters[1], nodeSO));
                    }

                    break;
                }
                case SSSpeakerType.Assigned3:
                {
                    if (assignedCharacters[2])
                    {
                        dialogues.Add(new Tuple<Sprite, string, string>(
                            assignedCharacters[2].GetCharacterData().characterIcon,
                            assignedCharacters[2].GetCharacterData().firstName, nodeSO.Text));
                        StartCoroutine(DisplayDialogue(assignedCharacters[2], nodeSO));
                    }

                    break;
                }
                case SSSpeakerType.Assigned4:
                {
                    if (assignedCharacters[3])
                    {
                        dialogues.Add(new Tuple<Sprite, string, string>(
                            assignedCharacters[3].GetCharacterData().characterIcon,
                            assignedCharacters[3].GetCharacterData().firstName, nodeSO.Text));
                        StartCoroutine(DisplayDialogue(assignedCharacters[3], nodeSO));
                    }

                    break;
                }
                case SSSpeakerType.NotAssigned:
                {
                    tempCharacters = spaceshipManager.characters.Except(assignedCharacters).ToList();
                    actualSpeaker = tempCharacters[Random.Range(0, tempCharacters.Count)];
                    dialogues.Add(new Tuple<Sprite, string, string>(actualSpeaker.GetCharacterData().characterIcon,
                        actualSpeaker.GetCharacterData().firstName, nodeSO.Text));
                    StartCoroutine(DisplayDialogue(actualSpeaker, nodeSO));

                    break;
                }
            }
        }

        private void RunNode(SSTaskNodeSO nodeSO)
        {
            var position = spaceshipManager.GetTaskPosition(nodeSO.Room).position;
            var notificationGO = spaceshipManager.taskNotificationPool.GetFromPool();
            if (notificationGO.TryGetComponent(out TaskNotification notification))
            {
                notification.transform.position = position;
                Task task = new Task(nodeSO.name, nodeSO.Description, nodeSO.Icon, nodeSO.TimeLeft, nodeSO.Duration,
                    nodeSO.MandatorySlots, nodeSO.OptionalSlots, nodeSO.TaskHelpFactor, nodeSO.Room, nodeSO.IsPermanent,
                    nodeSO.PreviewOutcome);
                notification.Initialize(task, spaceshipManager, dialogues);
                spaceshipManager.AddTask(notification);
                StartCoroutine(WaiterTask(nodeSO, task));
            }
        }

        private void RunNode(SSTimeNodeSO nodeSO)
        {
            timeNode = nodeSO;
            durationTimeNode = nodeSO.TimeToWait * TimeTickSystem.ticksPerHour;
            TimeTickSystem.OnTick += WaitingTime;
        }

        private void WaitingTime(object sender, TimeTickSystem.OnTickEventArgs e)
        {
            durationTimeNode -= TimeTickSystem.timePerTick;
            if (durationTimeNode <= 0)
            {
                if (timeNode.Choices.First().NextNode == null)
                {
                    ResetTimeline();
                    TimeTickSystem.OnTick -= WaitingTime;
                    return;
                }

                TimeTickSystem.OnTick -= WaitingTime;
                CheckNodeType(timeNode.Choices.First().NextNode);
            }
        }

        IEnumerator DisplayDialogue(CharacterBehaviour characterBehaviour, SSDialogueNodeSO nodeSO)
        {
            yield return new WaitUntil(() => characterBehaviour.speaker.isSpeaking == false);
            characterBehaviour.speaker.StartDialogue(nodeSO);

            yield return new WaitForSeconds(nodeSO.Duration);
            characterBehaviour.speaker.EndDialogue();
            if (nodeSO.Choices.First().NextNode == null)
            {
                ResetTimeline();
                yield break;
            }

            CheckNodeType(nodeSO.Choices.First().NextNode);
        }

        IEnumerator WaiterTask(SSTaskNodeSO nodeSO, Task task)
        {
            yield return new WaitUntil(() => spaceshipManager.GetTaskNotification(task).IsCompleted);
            assignedCharacters.AddRange(spaceshipManager.GetTaskNotification(task).LeaderCharacters);
            assignedCharacters.AddRange(spaceshipManager.GetTaskNotification(task).AssistantCharacters);
            if (nodeSO.Choices.First().NextNode == null)
            {
                ResetTimeline();
                yield break;
            }

            CheckNodeType(nodeSO.Choices.First().NextNode);
        }
    }
}