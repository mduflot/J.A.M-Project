using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CharacterSystem;
using Managers;
using Tasks;
using TMPro;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SS
{
    using Data;
    using Enumerations;
    using ScriptableObjects;

    public class SSLauncher : MonoBehaviour
    {
        [HideInInspector] public bool IsCancelled;
        [HideInInspector] public bool CanIgnoreDialogueTask;
        public bool IsRunning { get; private set; }
        public List<SerializableTuple<string, string>> dialogues { get; set; }
        public SSNodeSO CurrentNode { get; private set; }
        public List<CharacterBehaviour> characters { get; set; }
        public List<CharacterBehaviour> assignedCharacters { get; set; }
        public List<CharacterBehaviour> notAssignedCharacters { get; set; }
        public List<CharacterBehaviour> traitsCharacters { get; set; }
        public List<StorylineLog> storylineLogs { get; set; }

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

        private SSTimeNodeSO timeNode;
        private uint durationTimeNode;
        private Task task;

        private void Start()
        {
            spaceshipManager = GameManager.Instance.SpaceshipManager;
            storylineLogs = new List<StorylineLog>();
            dialogues = new();
            characters = new();
            assignedCharacters = new();
            notAssignedCharacters = new();
            traitsCharacters = new();
        }

        public void StartTimeline()
        {
            if (currentStoryline) currentStoryline.text = nodeContainer.name;
            IsRunning = true;
            IsCancelled = false;
            CanIgnoreDialogueTask = false;
            CheckNodeType(node);
        }

        public void StartTimeline(CharacterIcon icon)
        {
            if (currentStoryline) currentStoryline.text = nodeContainer.name;
            IsRunning = true;
            IsCancelled = false;
            CanIgnoreDialogueTask = false;
            RunNode(node as SSTaskNodeSO, icon);
        }

        private void ResetTimeline()
        {
            dialogues.Clear();
            characters.Clear();
            assignedCharacters.Clear();
            notAssignedCharacters.Clear();
            traitsCharacters.Clear();
            if (currentStoryline) currentStoryline.text = "No timeline";
        }

        /// <summary>
        /// Checks the type of the node and runs the appropriate function
        /// </summary>
        /// <param name="nodeSO"> Node you need to run </param>
        private void CheckNodeType(SSNodeSO nodeSO)
        {
            CurrentNode = nodeSO;
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
            List<CharacterBehaviour> tempCharacters = new();
            switch (nodeSO.SpeakerType)
            {
                case SSSpeakerType.Random:
                {
                    tempCharacters = spaceshipManager.characters.Except(characters).ToList();
                    if (tempCharacters.Count == 0)
                    {
                        Debug.LogWarning($"Try to get a random character but there is no one left");
                        break;
                    }

                    actualSpeaker = tempCharacters[Random.Range(0, tempCharacters.Count)];
                    SetDialogue(actualSpeaker, nodeSO);
                    characters.Add(actualSpeaker);
                    break;
                }
                case SSSpeakerType.Sensor:
                {
                    for (int index = 0; index < spaceshipManager.GetRoom(RoomType.Flight).roomObjects.Length; index++)
                    {
                        var furniture = spaceshipManager.GetRoom(RoomType.Flight).roomObjects[index];
                        if (furniture.furnitureType == FurnitureType.Console)
                        {
                            var sensor = furniture.transform;
                            if (sensor.TryGetComponent(out Speaker speaker))
                            {
                                dialogues.Add(new SerializableTuple<string, string>("Sensor", nodeSO.Text));
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
                    for (int index = 0; index < spaceshipManager.GetRoom(RoomType.Docking).roomObjects.Length; index++)
                    {
                        var furniture = spaceshipManager.GetRoom(RoomType.Docking).roomObjects[index];
                        if (furniture.furnitureType == FurnitureType.PortHole)
                        {
                            var sensor = furniture.transform;
                            if (sensor.TryGetComponent(out Speaker speaker))
                            {
                                dialogues.Add(new SerializableTuple<string, string>("Expert", nodeSO.Text));
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
                        SetDialogue(characters[0], nodeSO);
                    }
                    else Debug.LogWarning($"Try to get a character but no one is assigned to this slot");

                    break;
                }
                case SSSpeakerType.Character2:
                {
                    if (characters[1])
                    {
                        SetDialogue(characters[1], nodeSO);
                    }
                    else Debug.LogWarning($"Try to get a character but no one is assigned to this slot");

                    break;
                }
                case SSSpeakerType.Character3:
                {
                    if (characters[2])
                    {
                        SetDialogue(characters[2], nodeSO);
                    }
                    else Debug.LogWarning($"Try to get a character but no one is assigned to this slot");

                    break;
                }
                case SSSpeakerType.Character4:
                {
                    if (characters[3])
                    {
                        SetDialogue(characters[3], nodeSO);
                    }
                    else Debug.LogWarning($"Try to get a character but no one is assigned to this slot");

                    break;
                }
                case SSSpeakerType.Character5:
                {
                    if (characters[4])
                    {
                        SetDialogue(characters[4], nodeSO);
                    }
                    else Debug.LogWarning($"Try to get a character but no one is assigned to this slot");

                    break;
                }
                case SSSpeakerType.Character6:
                {
                    if (characters[5])
                    {
                        SetDialogue(characters[5], nodeSO);
                    }
                    else Debug.LogWarning($"Try to get a character but no one is assigned to this slot");

                    break;
                }
                case SSSpeakerType.Assigned1:
                {
                    if (assignedCharacters[0])
                    {
                        SetDialogue(assignedCharacters[0], nodeSO);
                    }
                    else Debug.LogWarning($"Try to get a character but no one is assigned to this slot");

                    break;
                }
                case SSSpeakerType.Assigned2:
                {
                    if (assignedCharacters[1])
                    {
                        SetDialogue(assignedCharacters[1], nodeSO);
                    }
                    else
                    {
                        if (nodeSO.Choices.First().NextNode == null)
                        {
                            IsRunning = false;
                            nodeGroup.StoryStatus = SSStoryStatus.Completed;
                            ResetTimeline();
                            return;
                        }

                        CheckNodeType(nodeSO.Choices.First().NextNode);
                    }

                    break;
                }
                case SSSpeakerType.Assigned3:
                {
                    if (assignedCharacters[2])
                    {
                        SetDialogue(assignedCharacters[2], nodeSO);
                    }
                    else
                    {
                        if (nodeSO.Choices.First().NextNode == null)
                        {
                            IsRunning = false;
                            nodeGroup.StoryStatus = SSStoryStatus.Completed;
                            ResetTimeline();
                            return;
                        }

                        CheckNodeType(nodeSO.Choices.First().NextNode);
                    }

                    break;
                }
                case SSSpeakerType.Assigned4:
                {
                    if (assignedCharacters[3])
                    {
                        SetDialogue(assignedCharacters[3], nodeSO);
                    }
                    else
                    {
                        if (nodeSO.Choices.First().NextNode == null)
                        {
                            IsRunning = false;
                            nodeGroup.StoryStatus = SSStoryStatus.Completed;
                            ResetTimeline();
                            return;
                        }

                        CheckNodeType(nodeSO.Choices.First().NextNode);
                    }

                    break;
                }
                case SSSpeakerType.Assigned5:
                {
                    if (assignedCharacters[4])
                    {
                        SetDialogue(assignedCharacters[4], nodeSO);
                    }
                    else
                    {
                        if (nodeSO.Choices.First().NextNode == null)
                        {
                            IsRunning = false;
                            nodeGroup.StoryStatus = SSStoryStatus.Completed;
                            ResetTimeline();
                            return;
                        }

                        CheckNodeType(nodeSO.Choices.First().NextNode);
                    }

                    break;
                }
                case SSSpeakerType.Assigned6:
                {
                    if (assignedCharacters[5])
                    {
                        SetDialogue(assignedCharacters[5], nodeSO);
                    }
                    else
                    {
                        if (nodeSO.Choices.First().NextNode == null)
                        {
                            IsRunning = false;
                            nodeGroup.StoryStatus = SSStoryStatus.Completed;
                            ResetTimeline();
                            return;
                        }

                        CheckNodeType(nodeSO.Choices.First().NextNode);
                    }

                    break;
                }
                case SSSpeakerType.NotAssigned:
                {
                    tempCharacters = spaceshipManager.characters.Except(assignedCharacters)
                        .Except(notAssignedCharacters).ToList();
                    if (tempCharacters.Count == 0)
                    {
                        Debug.LogWarning($"Try to get a character but all of them are assigned");
                        break;
                    }

                    actualSpeaker = tempCharacters[Random.Range(0, tempCharacters.Count)];
                    SetDialogue(actualSpeaker, nodeSO);
                    notAssignedCharacters.Add(actualSpeaker);

                    break;
                }
                case SSSpeakerType.Other1:
                {
                    if (notAssignedCharacters[0])
                    {
                        SetDialogue(notAssignedCharacters[0], nodeSO);
                    }
                    else Debug.LogWarning($"Try to get a character but no one is assigned to this slot");

                    break;
                }
                case SSSpeakerType.Other2:
                {
                    if (notAssignedCharacters[1])
                    {
                        SetDialogue(notAssignedCharacters[1], nodeSO);
                    }
                    else Debug.LogWarning($"Try to get a character but no one is assigned to this slot");

                    break;
                }
                case SSSpeakerType.Other3:
                {
                    if (notAssignedCharacters[2])
                    {
                        SetDialogue(notAssignedCharacters[2], nodeSO);
                    }
                    else Debug.LogWarning($"Try to get a character but no one is assigned to this slot");

                    break;
                }
                case SSSpeakerType.Other4:
                {
                    if (notAssignedCharacters[3])
                    {
                        SetDialogue(notAssignedCharacters[3], nodeSO);
                    }
                    else Debug.LogWarning($"Try to get a character but no one is assigned to this slot");

                    break;
                }
                case SSSpeakerType.Other5:
                {
                    if (notAssignedCharacters[4])
                    {
                        SetDialogue(notAssignedCharacters[4], nodeSO);
                    }
                    else Debug.LogWarning($"Try to get a character but no one is assigned to this slot");

                    break;
                }
                case SSSpeakerType.Other6:
                {
                    if (notAssignedCharacters[5])
                    {
                        SetDialogue(notAssignedCharacters[5], nodeSO);
                    }
                    else Debug.LogWarning($"Try to get a character but no one is assigned to this slot");

                    break;
                }
                case SSSpeakerType.Traits:
                {
                    for (int i = 0; i < spaceshipManager.characters.Length; i++)
                    {
                        var character = spaceshipManager.characters[i];
                        if (character.GetJob().HasFlag(nodeSO.Job) &&
                            character.GetPositiveTraits().HasFlag(nodeSO.PositiveTraits) &&
                            character.GetNegativeTraits().HasFlag(nodeSO.NegativeTraits))
                        {
                            tempCharacters.Add(character);
                        }
                    }

                    actualSpeaker = tempCharacters[Random.Range(0, tempCharacters.Count)];
                    dialogues.Add(
                        new SerializableTuple<string, string>(actualSpeaker.GetCharacterData().ID, nodeSO.Text));
                    StartCoroutine(DisplayDialogue(actualSpeaker.speaker, actualSpeaker.GetCharacterData().firstName,
                        nodeSO));
                    traitsCharacters.Add(actualSpeaker);

                    break;
                }
                case SSSpeakerType.CharacterTraits1:
                {
                    if (traitsCharacters[0])
                    {
                        SetDialogue(traitsCharacters[0], nodeSO);
                    }
                    else Debug.LogWarning($"Try to get a character but no one is assigned to this slot");

                    break;
                }
                case SSSpeakerType.CharacterTraits2:
                {
                    if (traitsCharacters[1])
                    {
                        SetDialogue(traitsCharacters[1], nodeSO);
                    }
                    else Debug.LogWarning($"Try to get a character but no one is assigned to this slot");

                    break;
                }
                case SSSpeakerType.CharacterTraits3:
                {
                    if (traitsCharacters[2])
                    {
                        SetDialogue(traitsCharacters[2], nodeSO);
                    }
                    else Debug.LogWarning($"Try to get a character but no one is assigned to this slot");

                    break;
                }
                case SSSpeakerType.CharacterTraits4:
                {
                    if (traitsCharacters[3])
                    {
                        SetDialogue(traitsCharacters[3], nodeSO);
                    }
                    else Debug.LogWarning($"Try to get a character but no one is assigned to this slot");

                    break;
                }
                case SSSpeakerType.CharacterTraits5:
                {
                    if (traitsCharacters[4])
                    {
                        SetDialogue(traitsCharacters[4], nodeSO);
                    }
                    else Debug.LogWarning($"Try to get a character but no one is assigned to this slot");

                    break;
                }
                case SSSpeakerType.CharacterTraits6:
                {
                    if (traitsCharacters[5])
                    {
                        SetDialogue(traitsCharacters[5], nodeSO);
                    }
                    else Debug.LogWarning($"Try to get a character but no one is assigned to this slot");

                    break;
                }
            }
        }

        private void SetDialogue(CharacterBehaviour character, SSDialogueNodeSO nodeSO)
        {
            dialogues.Add(new SerializableTuple<string, string>(character.GetCharacterData().ID, nodeSO.Text));
            StartCoroutine(DisplayDialogue(character.speaker,
                character.GetCharacterData().firstName, nodeSO));
        }

        private void RunNode(SSTaskNodeSO nodeSO, CharacterIcon icon = null)
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
                if (nodeSO.TaskType.Equals(SSTaskType.Permanent) && icon != null) notification.Display(icon);
                else if (nodeSO.TaskType.Equals(SSTaskType.Permanent) || nodeSO.TaskType.Equals(SSTaskType.Compute))
                    notification.Display();
                StartCoroutine(WaiterTask(nodeSO, task));
            }
        }

        public void RunTimedNodeCancel(Notification notification, Task actualTask, SSTaskNodeSO taskNode)
        {
            StartCoroutine(WaitTimedRunCancelNode(notification, actualTask, taskNode));
        }

        public void RunUntimedNodeCancel(Notification notification, Task actualTask, SSTaskNodeSO taskNode)
        {
            StartCoroutine(WaitUntimedRunCancelNode(notification, actualTask, taskNode));
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
                    IsRunning = false;
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

        private IEnumerator WaitTimedRunCancelNode(Notification notification, Task actualTask, SSTaskNodeSO taskNode)
        {
            yield return new WaitUntil(() => IsCancelled == false);
            notification.InitializeCancelTask();
            StartCoroutine(WaiterTask(taskNode, actualTask));
        }

        private IEnumerator WaitUntimedRunCancelNode(Notification notification, Task actualTask, SSTaskNodeSO taskNode)
        {
            yield return new WaitUntil(() => IsCancelled == false);
            CanIgnoreDialogueTask = false;
            notification.Initialize(actualTask, taskNode, spaceshipManager, this, dialogues);
            StartCoroutine(WaiterTask(taskNode, actualTask));
        }

        private IEnumerator DisplayDialogue(Speaker speaker, string characterName, SSDialogueNodeSO nodeSO)
        {
            nodeSO.IsCompleted = false;
            if (CanIgnoreDialogueTask && nodeSO.IsDialogueTask)
            {
                if (nodeSO.Choices.First().NextNode == null)
                {
                    IsRunning = false;
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

            yield return new WaitUntil(() => nodeSO.IsCompleted);

            if (nodeSO.Choices.First().NextNode == null)
            {
                IsRunning = false;
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

            if (nodeSO.Choices[task.conditionIndex].NextNode == null)
            {
                IsRunning = false;
                nodeGroup.StoryStatus = SSStoryStatus.Completed;
                ResetTimeline();
                yield break;
            }

            assignedCharacters.AddRange(spaceshipManager.GetTaskNotification(task).LeaderCharacters);
            assignedCharacters.AddRange(spaceshipManager.GetTaskNotification(task).AssistantCharacters);

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