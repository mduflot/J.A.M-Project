using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CharacterSystem;
using Managers;
using Tasks;
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
        public bool IsRunning { get; private set; }
        public List<SerializableTuple<string, string>> dialogues { get; set; }
        public SSNodeSO CurrentNode { get; private set; }

        /* STORYLINE & TIMELINE */
        public Storyline storyline { get; set; }
        public Timeline timeline { get; set; }
        public uint waitingTime { get; set; }
        public Task task { get; set; }

        /* CHARACTERS SPEAKER */
        public List<CharacterBehaviour> characters { get; set; }
        public List<CharacterBehaviour> assignedCharacters { get; set; }
        public List<CharacterBehaviour> notAssignedCharacters { get; set; }
        public List<CharacterBehaviour> traitsCharacters { get; set; }
        public SpaceshipManager spaceshipManager { get; set; }

        /* Node Scriptable Objects */
        public SSNodeContainerSO nodeContainer;
        public SSNodeGroupSO nodeGroup;
        public SSNodeSO node;

        /* Filters */
        [SerializeField] private bool groupedNodes;
        [SerializeField] private bool startingNodesOnly;

        /* Indexes */
        [SerializeField] private int selectedNodeGroupIndex;
        [SerializeField] private int selectedNodeIndex;

        /* Nodes */
        private SSTimeNodeSO timeNode;
        private uint durationTimeNode;
        private SSCheckConditionNodeSO checkNode;

        /* Logs */
        public StorylineLog storylineLog;

        private GameObject notificationGO;

        public void StartTimeline()
        {
            if (nodeContainer.StoryType != SSStoryType.Tasks)
            {
                if (Checker.Instance.allStorylineLogs.Any(storylineLog => storylineLog.storylineID == storyline.ID))
                {
                    storylineLog = Checker.Instance.allStorylineLogs.First(storylineLog =>
                        storylineLog.storylineID == storyline.ID);
                }
                else
                {
                    storylineLog = new StorylineLog(storyline.ID, storyline.StorylineContainer.FileName,
                        GameManager.Instance.UIManager.date.text, "");
                    Checker.Instance.allStorylineLogs.Add(storylineLog);
                }

                if (timeline.Status == SSStoryStatus.Completed)
                {
                    TimeTickSystem.OnTick += WaitTimeline;
                    return;
                }
            }

            if (nodeContainer.StoryType == SSStoryType.Principal)
                GameManager.Instance.UIManager.mainStorylineText.text = nodeContainer.name;
            spaceshipManager = GameManager.Instance.SpaceshipManager;
            dialogues ??= new();
            characters ??= new();
            assignedCharacters ??= new();
            notAssignedCharacters ??= new();
            traitsCharacters ??= new();
            IsRunning = true;
            task = null;
            SoundManager.Instance.PlaySound(SoundManager.Instance.notificationChime);
            CheckNodeType(node);
        }

        public void StartTimelineOnDrop(CharacterIcon icon)
        {
            if (nodeContainer.StoryType == SSStoryType.Principal)
                GameManager.Instance.UIManager.mainStorylineText.text = nodeContainer.name;
            spaceshipManager = GameManager.Instance.SpaceshipManager;
            dialogues = new();
            characters = new();
            assignedCharacters = new();
            notAssignedCharacters = new();
            traitsCharacters = new();
            IsRunning = true;
            task = null;
            StartCoroutine(RunNode(node as SSTaskNodeSO, icon));
        }

        public void StartTimelineOnTask(TaskLog taskLog)
        {
            if (nodeContainer.StoryType != SSStoryType.Tasks)
            {
                if (Checker.Instance.allStorylineLogs.Any(storylineLog => storylineLog.storylineID == storyline.ID))
                {
                    storylineLog = Checker.Instance.allStorylineLogs.First(storylineLog =>
                        storylineLog.storylineID == storyline.ID);
                }
                else
                {
                    storylineLog = new StorylineLog(storyline.ID, storyline.StorylineContainer.FileName,
                        GameManager.Instance.UIManager.date.text, "");
                    Checker.Instance.allStorylineLogs.Add(storylineLog);
                }

                if (timeline.Status == SSStoryStatus.Completed)
                {
                    TimeTickSystem.OnTick += WaitTimeline;
                    return;
                }
            }

            if (nodeContainer.StoryType == SSStoryType.Principal)
                GameManager.Instance.UIManager.mainStorylineText.text = nodeContainer.name;
            spaceshipManager = GameManager.Instance.SpaceshipManager;
            dialogues = new();
            characters = new();
            assignedCharacters = new();
            notAssignedCharacters = new();
            traitsCharacters = new();
            IsRunning = true;
            CurrentNode = node;
            task = null;
            StartCoroutine(RunNode(node as SSTaskNodeSO, null, taskLog));
        }

        private void ResetTimeline()
        {
            if (nodeContainer.StoryType == SSStoryType.Principal)
                GameManager.Instance.UIManager.mainStorylineText.text = "No Storyline";
        }

        private bool IsFinish()
        {
            return !storyline.Timelines.Any(timeline => timeline.Status == SSStoryStatus.Enabled);
        }

        private void WaitTimeline(object sender, TimeTickSystem.OnTickEventArgs e)
        {
            if (waitingTime > 0)
            {
                waitingTime -= TimeTickSystem.timePerTick;
                return;
            }

            TimeTickSystem.OnTick -= CheckConditionNode;
            TimeTickSystem.OnTick -= WaitTimeline;
            Checker.Instance.allStorylineLogs.First(storylineLog => storylineLog.storylineID == storyline.ID)
                .timelineLogs.Add(new TimelineLog(timeline.ID, timeline.TimelineContainer.GroupName,
                    GameManager.Instance.UIManager.date.text));

            List<Timeline> availableTimelines = new();
            for (var i = 0; i < storyline.Timelines.Count; i++)
            {
                var timeline = storyline.Timelines[i];
                if (timeline.Status == SSStoryStatus.Completed) continue;
                if (timeline.TimelineContainer.Condition)
                    if (RouteCondition(timeline.TimelineContainer.Condition))
                        continue;
                availableTimelines.Add(timeline);
            }

            if (availableTimelines.Count == 0)
            {
                if (!nodeContainer.IsReplayable) storyline.Status = SSStoryStatus.Completed;
                else
                {
                    for (int i = 0; i < storyline.Timelines.Count; i++)
                    {
                        var timeline = storyline.Timelines[i];
                        timeline.Status = SSStoryStatus.Enabled;
                    }
                }

                if (nodeContainer.StoryType == SSStoryType.Principal)
                    Checker.Instance.GenerateNewPrincipalEvent(nodeContainer.IsTutorialToPlay);
                Checker.Instance.activeLaunchers.Remove(this);
                Checker.Instance.launcherPool.AddToPool(gameObject);
                Debug.Log("No more timeline available / Storyline completed");
                return;
            }

            timeline = availableTimelines[Random.Range(0, availableTimelines.Count)];
            nodeGroup = timeline.TimelineContainer;
            for (int index = 0;
                 index < storyline.StorylineContainer.NodeGroups[timeline.TimelineContainer].Count;
                 index++)
            {
                if (storyline.StorylineContainer.NodeGroups[timeline.TimelineContainer][index].IsStartingNode)
                {
                    node = storyline.StorylineContainer.NodeGroups[timeline.TimelineContainer][index];
                    break;
                }
            }

            StartTimeline();
        }

        private bool RouteCondition(ConditionSO condition)
        {
            bool validateCondition = false;
            switch (condition.BaseCondition.target)
            {
                case OutcomeData.OutcomeTarget.Leader:
                    // validateCondition = ConditionSystem.CheckCharacterCondition(LeaderCharacters[0].GetTraits(), condition);
                    break;
                case OutcomeData.OutcomeTarget.Assistant:
                    // validateCondition = ConditionSystem.CheckCharacterCondition(AssistantCharacters[0].GetTraits(), condition);
                    break;
                case OutcomeData.OutcomeTarget.Gauge:
                    validateCondition = ConditionSystem.CheckGaugeCondition(condition);
                    break;
                case OutcomeData.OutcomeTarget.GaugeValue:
                    validateCondition = ConditionSystem.CheckGaugeValueCondition(condition);
                    break;
                case OutcomeData.OutcomeTarget.Crew:
                    validateCondition = ConditionSystem.CheckCrewCondition(condition);
                    break;
                case OutcomeData.OutcomeTarget.Ship:
                    validateCondition = ConditionSystem.CheckSpaceshipCondition(condition);
                    break;
            }

            return validateCondition;
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
                    StartCoroutine(RunNode(nodeSO as SSTaskNodeSO));
                    break;
                }
                case SSNodeType.Time:
                {
                    RunNode(nodeSO as SSTimeNodeSO);
                    break;
                }
                case SSNodeType.Popup:
                {
                    RunNode(nodeSO as SSPopupNodeSO);
                    break;
                }
                case SSNodeType.CheckCondition:
                {
                    RunNode(nodeSO as SSCheckConditionNodeSO);
                    break;
                }
            }
        }

        #region Dialogue

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
                    dialogues.Add(new SerializableTuple<string, string>("Sensor", nodeSO.Text));
                    StartCoroutine(DisplayDialogue("Sensor", nodeSO));

                    break;
                }
                case SSSpeakerType.Expert:
                {
                    dialogues.Add(new SerializableTuple<string, string>("Expert", nodeSO.Text));
                    StartCoroutine(DisplayDialogue("Expert", nodeSO));

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
                        if (nodeSO.Choices[0].NextNode == null)
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
                        if (nodeSO.Choices[0].NextNode == null)
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
                        if (nodeSO.Choices[0].NextNode == null)
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
                        if (nodeSO.Choices[0].NextNode == null)
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
                        if (nodeSO.Choices[0].NextNode == null)
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
                    StartCoroutine(DisplayDialogue(actualSpeaker.GetCharacterData().firstName, nodeSO));
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
                case SSSpeakerType.IncomingSignal:
                {
                    dialogues.Add(new SerializableTuple<string, string>("Incoming Signal", nodeSO.Text));
                    StartCoroutine(DisplayDialogue("Incoming Signal", nodeSO));

                    break;
                }
            }
        }

        private void SetDialogue(CharacterBehaviour character, SSDialogueNodeSO nodeSO)
        {
            dialogues.Add(new SerializableTuple<string, string>(character.GetCharacterData().ID, nodeSO.Text));
            StartCoroutine(DisplayDialogue(character.GetCharacterData().firstName, nodeSO));
        }

        private IEnumerator DisplayDialogue(string characterName, SSDialogueNodeSO nodeSO)
        {
            nodeSO.IsCompleted = false;

            if (!nodeSO.IsDialogueTask && task != null) yield return new WaitUntil(() => task.Duration <= 0);

            if (nodeContainer.StoryType is SSStoryType.Principal)
            {
                if (nodeSO.IsDialogueTask && nodeSO.DialogueType == SSDialogueType.Bark)
                {
                    StartCoroutine(AddDialogueNotification(nodeSO));
                }
                else
                {
                    GameManager.Instance.UIManager.dialogueManager.InitializeMenu(storyline.StorylineContainer.name);
                    GameManager.Instance.UIManager.dialogueManager.AddDialogue(nodeSO, characterName);
                }
            }
            else
            {
                StartCoroutine(AddDialogueNotification(nodeSO));
            }

            yield return new WaitUntil(() => nodeSO.IsCompleted);

            if (nodeSO.Choices[0].NextNode == null)
            {
                GameManager.Instance.UIManager.dialogueManager.ActivateButton();
                IsRunning = false;
                ResetTimeline();
                if (nodeContainer.StoryType != SSStoryType.Tasks)
                {
                    timeline.Status = SSStoryStatus.Completed;
                    if (nodeGroup.TimeIsOverride)
                        waitingTime = nodeGroup.OverrideWaitTime * TimeTickSystem.ticksPerHour;
                    else
                        waitingTime = (uint)(Random.Range(nodeGroup.MinWaitTime, nodeGroup.MaxWaitTime) *
                                             TimeTickSystem.ticksPerHour);
                    if (IsFinish()) waitingTime = 0;
                    TimeTickSystem.OnTick += WaitTimeline;
                }

                yield break;
            }

            if (nodeSO.Choices[0].NextNode is not SSDialogueNodeSO)
                GameManager.Instance.UIManager.dialogueManager.ActivateButton();
            CheckNodeType(nodeSO.Choices[0].NextNode);
        }

        private IEnumerator AddDialogueNotification(SSDialogueNodeSO nodeSO)
        {
            if (notificationGO.TryGetComponent(out Notification notification))
            {
                if (nodeSO.IsDialogueTask)
                    yield return new WaitUntil(() =>
                        1 - nodeSO.PercentageTask / 100.0f >= task.Duration / task.BaseDuration);
                Sprite spriteIcon;
                string firstName;
                switch (nodeSO.BarkType)
                {
                    case SSBarkType.Awaiting:
                        Debug.LogError("Don't use Awaiting bark type for dialogue, is already handled by the system");
                        break;
                    case SSBarkType.Completing:
                        spriteIcon = task.leaderCharacters[0].GetCharacterData().characterIcon;
                        firstName = task.leaderCharacters[0].GetCharacterData().firstName;
                        notification.DisplayDialogue(spriteIcon, firstName,
                            task.leaderCharacters[0].GetCharacterData().completingBarks[
                                Random.Range(0,
                                    task.leaderCharacters[0].GetCharacterData().completingBarks.Length)], nodeSO);
                        break;
                    case SSBarkType.Completed:
                        spriteIcon = task.leaderCharacters[0].GetCharacterData().characterIcon;
                        firstName = task.leaderCharacters[0].GetCharacterData().firstName;
                        notification.DisplayDialogue(spriteIcon, firstName,
                            task.leaderCharacters[0].GetCharacterData().completedBarks[
                                Random.Range(0,
                                    task.leaderCharacters[0].GetCharacterData().completedBarks.Length)], nodeSO);
                        break;
                    case SSBarkType.Ignored:
                        var randomCharacter =
                            spaceshipManager.characters[Random.Range(0, spaceshipManager.characters.Length)];
                        spriteIcon = randomCharacter.GetCharacterData().characterIcon;
                        firstName = randomCharacter.GetCharacterData().firstName;
                        notification.DisplayDialogue(spriteIcon, firstName,
                            randomCharacter.GetCharacterData().ignoredBarks[
                                Random.Range(0, randomCharacter.GetCharacterData().ignoredBarks.Length)],
                            nodeSO);
                        break;
                }
            }
        }

        #endregion

        #region Cancel

        public void RunTimedNodeCancel(Notification notification, Task actualTask, SSTaskNodeSO taskNode)
        {
            StopAllCoroutines();
            notification.InitializeCancelTask();
            StartCoroutine(WaiterTask(taskNode, actualTask));
        }

        public void RunUntimedNodeCancel(Notification notification, Task actualTask, SSTaskNodeSO taskNode)
        {
            StopAllCoroutines();
            notification.Initialize(actualTask, taskNode, spaceshipManager, this, dialogues);
            StartCoroutine(WaiterTask(taskNode, actualTask));
        }

        #endregion

        #region Time

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
                if (timeNode.Choices[0].NextNode == null)
                {
                    IsRunning = false;
                    ResetTimeline();
                    if (nodeContainer.StoryType != SSStoryType.Tasks)
                    {
                        timeline.Status = SSStoryStatus.Completed;
                        if (nodeGroup.TimeIsOverride)
                            waitingTime = nodeGroup.OverrideWaitTime * TimeTickSystem.ticksPerHour;
                        else
                            waitingTime = (uint)(Random.Range(nodeGroup.MinWaitTime, nodeGroup.MaxWaitTime) *
                                                 TimeTickSystem.ticksPerHour);
                        if (IsFinish()) waitingTime = 0;
                        TimeTickSystem.OnTick += WaitTimeline;
                    }

                    return;
                }

                TimeTickSystem.OnTick -= WaitingTime;
                CheckNodeType(timeNode.Choices[0].NextNode);
            }
        }

        #endregion

        #region Task

        private IEnumerator RunNode(SSTaskNodeSO node, CharacterIcon icon = null, TaskLog taskToPlay = null)
        {
            // Verify if the task is already active
            if (node.TaskType.Equals(SSTaskType.Permanent))
                if (spaceshipManager.IsTaskActive(node.name))
                    yield break;

            if (task != null && taskToPlay == null) yield return new WaitUntil(() => task.Duration <= 0);
            assignedCharacters.Clear();
            notAssignedCharacters.Clear();
            var room = spaceshipManager.GetRoom(node.Room);
            notificationGO = spaceshipManager.notificationPool.GetFromPool();
            if (notificationGO.TryGetComponent(out Notification notification))
            {
                Transform roomTransform;
                if (room.roomObjects.Any(furniture => furniture.furnitureType == node.Furniture))
                {
                    roomTransform = room.roomObjects.First(furniture => furniture.furnitureType == node.Furniture)
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
                foreach (var choiceData in node.Choices)
                {
                    conditions.Add(new Tuple<ConditionSO, string>(((SSNodeChoiceTaskData)choiceData).Condition,
                        ((SSNodeChoiceTaskData)choiceData).PreviewOutcome));
                }

                if (taskToPlay != null)
                {
                    task = new Task(node, storyline != null ? storyline.StorylineContainer.FileName : "", conditions,
                        taskToPlay.IsStarted);
                    notification.Initialize(task, node, spaceshipManager, this, dialogues, taskToPlay);
                }
                else
                {
                    task = new Task(node, storyline != null ? storyline.StorylineContainer.FileName : "", conditions);
                    notification.Initialize(task, node, spaceshipManager, this, dialogues);
                }

                if (nodeContainer.StoryType is SSStoryType.Spontaneous)
                {
                    var randomCharacter =
                        spaceshipManager.characters[Random.Range(0, spaceshipManager.characters.Length)];
                    notification.DisplayDialogue(randomCharacter.GetCharacterData().characterIcon,
                        randomCharacter.GetCharacterData().firstName,
                        randomCharacter.GetCharacterData()
                            .awaitingBarks[Random.Range(0, randomCharacter.GetCharacterData().awaitingBarks.Length)]);
                }

                if (node.TaskType.Equals(SSTaskType.Permanent) && icon != null) notification.Display(icon);
                else if (node.TaskType.Equals(SSTaskType.Permanent) || node.TaskType.Equals(SSTaskType.Compute))
                    notification.Display();
                StartCoroutine(WaiterTask(node, task));
            }
        }

        private IEnumerator WaiterTask(SSTaskNodeSO nodeSO, Task task)
        {
            var notification = spaceshipManager.GetTaskNotification(task);
            yield return new WaitUntil(() => notification.IsStarted);

            if (nodeSO.Choices[task.conditionIndex].NextNode == null)
            {
                yield return new WaitUntil(() => task.Duration <= 0);
                IsRunning = false;
                ResetTimeline();
                if (nodeContainer.StoryType != SSStoryType.Tasks)
                {
                    timeline.Status = SSStoryStatus.Completed;
                    if (nodeGroup.TimeIsOverride)
                        waitingTime = nodeGroup.OverrideWaitTime * TimeTickSystem.ticksPerHour;
                    else
                        waitingTime = (uint)(Random.Range(nodeGroup.MinWaitTime, nodeGroup.MaxWaitTime) *
                                             TimeTickSystem.ticksPerHour);
                    if (IsFinish()) waitingTime = 0;
                    TimeTickSystem.OnTick += WaitTimeline;
                }

                yield break;
            }

            assignedCharacters.AddRange(spaceshipManager.GetTaskNotification(task).LeaderCharacters);
            assignedCharacters.AddRange(spaceshipManager.GetTaskNotification(task).AssistantCharacters);

            CheckNodeType(nodeSO.Choices[task.conditionIndex].NextNode);
        }

        #endregion

        #region Popup

        private void RunNode(SSPopupNodeSO nodeSO)
        {
            StartCoroutine(WaitingPopup(nodeSO));
        }

        private IEnumerator WaitingPopup(SSPopupNodeSO nodeSO)
        {
            if (task != null) yield return new WaitUntil(() => task.Duration <= 0);

            if (nodeSO.IsTutorialPopup) GameManager.Instance.UIManager.PopupTutorial.Initialize(nodeSO.Text);
            else
            {
                storylineLog.storylineEndLog = nodeSO.Text;
                GameManager.Instance.UIManager.PopupStoryline.Initialize(nodeSO.Text, nodeContainer.FileName);
            }

            switch (nodeSO.PopupUIType)
            {
                case SSPopupUIType.None:
                    break;
                case SSPopupUIType.Gauges:
                    for (int index = 0; index < GameManager.Instance.UIManager.GaugesMenu.Count; index++)
                    {
                        var gauge = GameManager.Instance.UIManager.GaugesMenu[index];
                        gauge.SetActive(true);
                    }

                    break;
                case SSPopupUIType.Tasks:
                    GameManager.Instance.UIManager.TasksMenu.SetActive(true);
                    break;
                case SSPopupUIType.Spaceship:
                    GameManager.Instance.UIManager.SpaceshipMenu.SetActive(true);
                    break;
            }

            yield return new WaitUntil(() => GameManager.Instance.UIManager.PopupTutorial.continueButtonPressed ||
                                             GameManager.Instance.UIManager.PopupTutorial.passTutorialPressed);

            if (nodeSO.Choices[0].NextNode == null ||
                GameManager.Instance.UIManager.PopupTutorial.passTutorialPressed)
            {
                IsRunning = false;
                ResetTimeline();
                GameManager.Instance.SpaceshipManager.IsInTutorial = false;
                for (int index = 0; index < GameManager.Instance.UIManager.GaugesMenu.Count; index++)
                {
                    var gauge = GameManager.Instance.UIManager.GaugesMenu[index];
                    gauge.SetActive(true);
                }

                GameManager.Instance.UIManager.TasksMenu.SetActive(true);
                GameManager.Instance.UIManager.SpaceshipMenu.SetActive(true);
                if (nodeContainer.StoryType != SSStoryType.Tasks)
                {
                    timeline.Status = SSStoryStatus.Completed;
                    if (nodeGroup.TimeIsOverride)
                        waitingTime = nodeGroup.OverrideWaitTime * TimeTickSystem.ticksPerHour;
                    else
                        waitingTime = (uint)(Random.Range(nodeGroup.MinWaitTime, nodeGroup.MaxWaitTime) *
                                             TimeTickSystem.ticksPerHour);
                    if (IsFinish()) waitingTime = 0;
                    TimeTickSystem.OnTick += WaitTimeline;
                }

                if (nodeSO.IsTutorialPopup) Debug.Log("End of tutorial");
                yield break;
            }

            CheckNodeType(nodeSO.Choices[0].NextNode);
        }

        #endregion

        #region CheckCondition

        private void RunNode(SSCheckConditionNodeSO nodeSO)
        {
            checkNode = nodeSO;
            TimeTickSystem.OnTick += CheckConditionNode;

            if (timeNode.Choices[0].NextNode == null)
            {
                IsRunning = false;
                ResetTimeline();
                if (nodeContainer.StoryType != SSStoryType.Tasks)
                {
                    timeline.Status = SSStoryStatus.Completed;
                    if (nodeGroup.TimeIsOverride)
                        waitingTime = nodeGroup.OverrideWaitTime * TimeTickSystem.ticksPerHour;
                    else
                        waitingTime = (uint)(Random.Range(nodeGroup.MinWaitTime, nodeGroup.MaxWaitTime) *
                                             TimeTickSystem.ticksPerHour);
                    if (IsFinish()) waitingTime = 0;
                    TimeTickSystem.OnTick += WaitTimeline;
                }

                return;
            }

            CheckNodeType(nodeSO.Choices[0].NextNode);
        }

        private void CheckConditionNode(object sender, TimeTickSystem.OnTickEventArgs e)
        {
            if (!RouteCondition(checkNode.Condition)) return;

            var outcomeEventArgs = new OutcomeSystem.OutcomeEventArgs[checkNode.Condition.outcomes.Outcomes.Length];
            for (int index = 0; index < checkNode.Condition.outcomes.Outcomes.Length; index++)
            {
                var outcome = checkNode.Condition.outcomes.Outcomes[index];
                switch (outcome.OutcomeTarget)
                {
                    case OutcomeData.OutcomeTarget.Leader:
                        outcomeEventArgs[index] = OutcomeSystem.GenerateEventArgs(outcome, task.leaderCharacters[0]);
                        break;

                    case OutcomeData.OutcomeTarget.Assistant:
                        outcomeEventArgs[index] = OutcomeSystem.GenerateEventArgs(outcome, task.assistantCharacters[0]);
                        break;

                    case OutcomeData.OutcomeTarget.Crew:
                        outcomeEventArgs[index] = OutcomeSystem.GenerateEventArgs(outcome,
                            GameManager.Instance.SpaceshipManager.characters);
                        break;

                    case OutcomeData.OutcomeTarget.Ship:
                        outcomeEventArgs[index] = OutcomeSystem.GenerateEventArgs(outcome);
                        break;
                    case OutcomeData.OutcomeTarget.Gauge:
                        if (outcome.OutcomeType.Equals(OutcomeData.OutcomeType.Gauge))
                            outcomeEventArgs[index] =
                                OutcomeSystem.GenerateEventArgs(outcome, outcome.OutcomeTargetGauge);
                        else
                        {
                            if (task.leaderCharacters[0].GetMood() < task.leaderCharacters[0].GetBaseVolition())
                            {
                                outcomeEventArgs[index] = OutcomeSystem.GenerateEventArgs(outcome,
                                    outcome.OutcomeTargetGauge, task.leaderCharacters[0].GetBaseVolition() / 2);
                            }
                            else
                            {
                                outcomeEventArgs[index] = OutcomeSystem.GenerateEventArgs(outcome,
                                    outcome.OutcomeTargetGauge, task.leaderCharacters[0].GetBaseVolition());
                            }
                        }

                        if (task.TaskType == SSTaskType.Permanent) outcomeEventArgs[index].value /= task.Duration;

                        break;
                }
            }

            var outcomeEvents = new OutcomeSystem.OutcomeEvent[outcomeEventArgs.Length];
            for (int i = 0; i < outcomeEventArgs.Length; i++)
            {
                outcomeEvents[i] = OutcomeSystem.GenerateOutcomeEvent(outcomeEventArgs[i]);
            }

            for (uint i = 0; i < outcomeEvents.Length; i++)
            {
                outcomeEvents[i].Invoke(outcomeEventArgs[i]);
            }

            TimeTickSystem.OnTick -= CheckConditionNode;
        }

        #endregion
    }
}