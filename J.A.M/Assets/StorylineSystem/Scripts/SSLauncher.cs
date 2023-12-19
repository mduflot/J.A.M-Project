using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CharacterSystem;
using Managers;
using SS.Data;
using Tasks;
using TMPro;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SS
{
    using Enumerations;
    using ScriptableObjects;

    public class SSLauncher : MonoBehaviour
    {
        public bool IsCancelled;
        public bool CanIgnoreDialogueTask;

        /* UI GameObjects */
        [SerializeField] private TextMeshProUGUI currentStoryline;
        [SerializeField] private SpaceshipManager spaceshipManager;

        /* Node Scriptable Objects */
        [SerializeField] public SSNodeContainerSO nodeContainer;
        [SerializeField] public SSNodeGroupSO nodeGroup;
        [SerializeField] public SSNodeSO node;

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
        private Task task;
        private bool isRunning;

        public bool IsRunning => isRunning;

        private void Start()
        {
            spaceshipManager = GameManager.Instance.SpaceshipManager;
        }

        public void StartTimeline()
        {
            dialogues = new();
            if (currentStoryline) currentStoryline.text = nodeContainer.name;
            isRunning = true;
            IsCancelled = false;
            CanIgnoreDialogueTask = false;
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
        private void CheckNodeType(SSNodeSO nodeSO)
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
            }
        }

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
                    StartCoroutine(DisplayDialogue(actualSpeaker.speaker, actualSpeaker.GetCharacterData().firstName,
                        nodeSO));
                    characters.Add(actualSpeaker);
                    break;
                }
                case SSSpeakerType.RandomOther:
                {
                    tempCharacters = spaceshipManager.characters.Except(characters).ToList();
                    actualSpeaker = tempCharacters[Random.Range(0, tempCharacters.Count)];
                    dialogues.Add(new Tuple<Sprite, string, string>(actualSpeaker.GetCharacterData().characterIcon,
                        actualSpeaker.GetCharacterData().firstName, nodeSO.Text));
                    StartCoroutine(DisplayDialogue(actualSpeaker.speaker, actualSpeaker.GetCharacterData().firstName,
                        nodeSO));
                    characters.Add(actualSpeaker);
                    break;
                }
                case SSSpeakerType.Sensor:
                {
                    dialogues.Add(new Tuple<Sprite, string, string>(null, "Sensor", nodeSO.Text));
                    for (int index = 0; index < spaceshipManager.GetRoom(RoomType.Flight).roomObjects.Length; index++)
                    {
                        var furniture = spaceshipManager.GetRoom(RoomType.Flight).roomObjects[index];
                        if (furniture.furnitureType == FurnitureType.Console)
                        {
                            var sensor = furniture.transform;
                            if (sensor.TryGetComponent(out Speaker speaker))
                            {
                                StartCoroutine(DisplayDialogue(speaker, "Sensor", nodeSO));
                            }
                            else
                            {
                                Debug.LogWarning("No speaker on the sensor");
                            }

                            break;
                        }
                    }

                    break;
                }
                case SSSpeakerType.Expert:
                {
                    dialogues.Add(new Tuple<Sprite, string, string>(null, "Expert", nodeSO.Text));
                    for (int index = 0; index < spaceshipManager.GetRoom(RoomType.Docking).roomObjects.Length; index++)
                    {
                        var furniture = spaceshipManager.GetRoom(RoomType.Docking).roomObjects[index];
                        if (furniture.furnitureType == FurnitureType.PortHole)
                        {
                            var sensor = furniture.transform;
                            if (sensor.TryGetComponent(out Speaker speaker))
                            {
                                StartCoroutine(DisplayDialogue(speaker, "Expert", nodeSO));
                            }
                            else
                            {
                                Debug.LogWarning("No speaker on the expert");
                            }

                            break;
                        }
                    }

                    break;
                }
                case SSSpeakerType.Character1:
                {
                    if (characters[0])
                    {
                        dialogues.Add(new Tuple<Sprite, string, string>(characters[0].GetCharacterData().characterIcon,
                            characters[0].GetCharacterData().firstName, nodeSO.Text));
                        StartCoroutine(DisplayDialogue(characters[0].speaker,
                            characters[0].GetCharacterData().firstName, nodeSO));
                    }

                    break;
                }
                case SSSpeakerType.Character2:
                {
                    if (characters[1])
                    {
                        dialogues.Add(new Tuple<Sprite, string, string>(characters[1].GetCharacterData().characterIcon,
                            characters[1].GetCharacterData().firstName, nodeSO.Text));
                        StartCoroutine(DisplayDialogue(characters[1].speaker,
                            characters[1].GetCharacterData().firstName, nodeSO));
                    }

                    break;
                }
                case SSSpeakerType.Character3:
                {
                    if (characters[2])
                    {
                        dialogues.Add(new Tuple<Sprite, string, string>(characters[2].GetCharacterData().characterIcon,
                            characters[2].GetCharacterData().firstName, nodeSO.Text));
                        StartCoroutine(DisplayDialogue(characters[2].speaker,
                            characters[2].GetCharacterData().firstName, nodeSO));
                    }

                    break;
                }
                case SSSpeakerType.Character4:
                {
                    if (characters[3])
                    {
                        dialogues.Add(new Tuple<Sprite, string, string>(characters[3].GetCharacterData().characterIcon,
                            characters[3].GetCharacterData().firstName, nodeSO.Text));
                        StartCoroutine(DisplayDialogue(characters[3].speaker,
                            characters[3].GetCharacterData().firstName, nodeSO));
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
                        StartCoroutine(DisplayDialogue(assignedCharacters[0].speaker,
                            assignedCharacters[0].GetCharacterData().firstName, nodeSO));
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
                        StartCoroutine(DisplayDialogue(assignedCharacters[1].speaker,
                            assignedCharacters[1].GetCharacterData().firstName, nodeSO));
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
                        StartCoroutine(DisplayDialogue(assignedCharacters[2].speaker,
                            assignedCharacters[2].GetCharacterData().firstName, nodeSO));
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
                        StartCoroutine(DisplayDialogue(assignedCharacters[3].speaker,
                            assignedCharacters[3].GetCharacterData().firstName, nodeSO));
                    }

                    break;
                }
                case SSSpeakerType.NotAssigned:
                {
                    tempCharacters = spaceshipManager.characters.Except(assignedCharacters).ToList();
                    actualSpeaker = tempCharacters[Random.Range(0, tempCharacters.Count)];
                    dialogues.Add(new Tuple<Sprite, string, string>(actualSpeaker.GetCharacterData().characterIcon,
                        actualSpeaker.GetCharacterData().firstName, nodeSO.Text));
                    StartCoroutine(DisplayDialogue(actualSpeaker.speaker, actualSpeaker.GetCharacterData().firstName,
                        nodeSO));

                    break;
                }
            }
        }

        private void RunNode(SSTaskNodeSO nodeSO)
        {
            if (nodeSO.TaskType.Equals(SSTaskType.Permanent))
                if (spaceshipManager.IsTaskActive(nodeSO.name))
                    return;
            var room = spaceshipManager.GetRoom(nodeSO.Room);
            var notificationGO = spaceshipManager.notificationPool.GetFromPool();
            if (notificationGO.TryGetComponent(out Notification notification))
            {
                Transform roomTransform;
                if (room.roomObjects.Any(furniture => furniture.furnitureType == nodeSO.Furniture))
                {
                    roomTransform = room.roomObjects.First(furniture => furniture.furnitureType == nodeSO.Furniture)
                        .transform;
                    roomTransform.GetComponent<NotificationContainer>().DisplayNotification();
                }
                else
                {
                    roomTransform = room.roomObjects[0].transform;
                }

                notification.transform.position = roomTransform.position;
                notificationGO.transform.parent = roomTransform;
                roomTransform.GetComponent<NotificationContainer>().DisplayNotification();
                var conditions = new List<Tuple<ConditionSO, string>>();
                foreach (var choiceData in nodeSO.Choices)
                {
                    conditions.Add(new Tuple<ConditionSO, string>(((SSNodeChoiceTaskData)choiceData).Condition,
                        ((SSNodeChoiceTaskData)choiceData).PreviewOutcome));
                }

                task = new Task(nodeSO.name, nodeSO.Description, nodeSO.TaskStatus, nodeSO.TaskType, nodeSO.Icon,
                    nodeSO.TimeLeft, nodeSO.Duration,
                    nodeSO.MandatorySlots, nodeSO.OptionalSlots, nodeSO.TaskHelpFactor, nodeSO.Room,
                    conditions);
                notification.Initialize(task, nodeSO, spaceshipManager, this, dialogues);
                spaceshipManager.AddTask(notification);
                if (nodeSO.TaskType.Equals(SSTaskType.Permanent)) notification.Display();
                StartCoroutine(WaiterTask(nodeSO, task));
            }
        }

        public void RunNodeCancel(Notification notification, Task actualTask, float duration, SSTaskNodeSO taskNode)
        {
            StartCoroutine(WaitRunCancelNode(notification, actualTask, taskNode));
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
                    isRunning = false;
                    nodeGroup.StoryStatus = SSStoryStatus.Completed;
                    ResetTimeline();
                    TimeTickSystem.OnTick -= WaitingTime;
                    return;
                }

                if (IsCancelled)
                {
                    CanIgnoreDialogueTask = true;
                    IsCancelled = false;
                    TimeTickSystem.OnTick -= WaitingTime;
                    return;
                }

                TimeTickSystem.OnTick -= WaitingTime;
                CheckNodeType(timeNode.Choices.First().NextNode);
            }
        }

        private IEnumerator WaitRunCancelNode(Notification notification, Task actualTask, SSTaskNodeSO taskNode)
        {
            yield return new WaitUntil(() => IsCancelled == false);
            notification.InitializeCancelTask();
            StartCoroutine(WaiterTask(taskNode, actualTask));
        }

        private IEnumerator DisplayDialogue(Speaker speaker, string characterName, SSDialogueNodeSO nodeSO)
        {
            nodeSO.isCompleted = false;
            if (CanIgnoreDialogueTask && nodeSO.IsDialogueTask)
            {
                if (nodeSO.Choices.First().NextNode == null)
                {
                    isRunning = false;
                    nodeGroup.StoryStatus = SSStoryStatus.Completed;
                    ResetTimeline();
                    yield break;
                }
            }

            CanIgnoreDialogueTask = false;
            if (!nodeSO.IsDialogueTask && task != null)
                yield return new WaitUntil(() => task.Duration <= 0 || IsCancelled);
            if (IsCancelled)
            {
                CanIgnoreDialogueTask = true;
                IsCancelled = false;
                yield break;
            }

            speaker.AddDialogue(task, nodeSO, characterName);

            yield return new WaitUntil(() => nodeSO.isCompleted);

            if (nodeSO.Choices.First().NextNode == null)
            {
                isRunning = false;
                nodeGroup.StoryStatus = SSStoryStatus.Completed;
                ResetTimeline();
                yield break;
            }

            CheckNodeType(nodeSO.Choices.First().NextNode);
        }

        private IEnumerator WaiterTask(SSTaskNodeSO nodeSO, Task task)
        {
            var notification = spaceshipManager.GetTaskNotification(task);
            yield return new WaitUntil(() => notification.IsStarted || notification.IsCancelled);
            if (notification.IsCancelled)
            {
                if (task.TaskType.Equals(SSTaskType.Permanent)) spaceshipManager.RemoveTask(notification);
                yield break;
            }

            assignedCharacters.AddRange(spaceshipManager.GetTaskNotification(task).LeaderCharacters);
            assignedCharacters.AddRange(spaceshipManager.GetTaskNotification(task).AssistantCharacters);
            if (nodeSO.Choices[task.conditionIndex].NextNode == null)
            {
                isRunning = false;
                nodeGroup.StoryStatus = SSStoryStatus.Completed;
                ResetTimeline();
                yield break;
            }

            if (IsCancelled)
            {
                CanIgnoreDialogueTask = true;
                IsCancelled = false;
                yield break;
            }

            CheckNodeType(nodeSO.Choices[task.conditionIndex].NextNode);
        }
    }
}