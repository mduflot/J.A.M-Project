using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CharacterSystem;
using Managers;
using SS;
using SS.Enumerations;
using SS.ScriptableObjects;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Checker : MonoBehaviour, IDataPersistence
{
    public static Checker Instance { get; private set; }
    public Pool<GameObject> launcherPool;
    [HideInInspector] public List<SSLauncher> activeLaunchers;
    [HideInInspector] public List<StorylineLog> allStorylineLogs;

    [SerializeField] private List<SSCampaignSO> ssCampaigns;
    [SerializeField] private GameObject presentationContainer;
    [SerializeField] private TextMeshProUGUI presentationText;

    [SerializeField] private GameObject launcherPrefab;
    [SerializeField] private uint waitTimeAfterTutorial = 3;
    [SerializeField] private uint waitTimeTrajectory = 12;
    [SerializeField] private uint minWaitTimePrincipal = 20;
    [SerializeField] private uint maxWaitTimePrincipal = 30;
    [SerializeField] private uint minWaitTimeSecondary = 20;
    [SerializeField] private uint maxWaitTimeSecondary = 30;

    private SSCampaignSO ssCampaign;

    public List<Storyline> allStorylines;
    public List<Storyline> principalStorylines;
    private List<Storyline> secondaryStorylines;
    private List<Storyline> leakStorylines;
    private List<Storyline> trajectoryStorylines;

    private List<Storyline> availableStoryLines = new();
    private List<SSNodeGroupSO> availableTimelines = new();

    private uint waitingTimePrincipal;
    private uint waitingTimeSecondary;
    private uint waitingTimeTrajectory;
    private bool isAlreadyWaiting;

    public bool IsStopChecker;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            Debug.Log("Found more than one Checker in the scene.");
            return;
        }

        Instance = this;
    }

    private void Initialize(SSCampaignSO campaign = null)
    {
        launcherPool = new Pool<GameObject>(launcherPrefab, 3);
        activeLaunchers = new List<SSLauncher>();
        ssCampaign = campaign != null ? campaign : ssCampaigns[0];
        allStorylines = new List<Storyline>();
        principalStorylines = new();
        secondaryStorylines = new();
        leakStorylines = new();
        trajectoryStorylines = new();
        for (int i = 0; i < ssCampaign.Storylines.Count; i++)
        {
            var storyline = ssCampaign.Storylines[i];
            switch (storyline.StoryType)
            {
                case SSStoryType.Principal:
                    principalStorylines.Add(new Storyline(storyline, storyline.NodeGroups.Keys.ToList()));
                    break;
                case SSStoryType.Spontaneous:
                    secondaryStorylines.Add(new Storyline(storyline, storyline.NodeGroups.Keys.ToList()));
                    break;
                case SSStoryType.Leak:
                    leakStorylines.Add(new Storyline(storyline, storyline.NodeGroups.Keys.ToList()));
                    break;
                case SSStoryType.Trajectory:
                    trajectoryStorylines.Add(new Storyline(storyline, storyline.NodeGroups.Keys.ToList()));
                    break;
                default:
                    Debug.Log("Storyline type not found.");
                    break;
            }
        }

        allStorylines.AddRange(principalStorylines);
        allStorylines.AddRange(secondaryStorylines);
        allStorylines.AddRange(leakStorylines);
        allStorylines.AddRange(trajectoryStorylines);
        isAlreadyWaiting = false;
        if (!DataPersistenceManager.Instance.IsNewGame) return;
        GameManager.Instance.UIManager.TasksMenu.SetActive(false);
        GameManager.Instance.UIManager.SpaceshipMenu.SetActive(false);
        for (int index = 0; index < GameManager.Instance.UIManager.GaugesMenu.Count; index++)
        {
            var gauge = GameManager.Instance.UIManager.GaugesMenu[index];
            gauge.SetActive(false);
        }
        ChooseNewStoryline(SSStoryType.Principal);
        DataPersistenceManager.Instance.IsNewGame = false;
    }

    public void GenerateNewEvent()
    {
        Debug.Log("Generating new secondary event. Maybe nothing will happen.");
        ChooseNewStoryline(SSStoryType.Spontaneous);
    }

    public void GenerateNewPrincipalEvent(bool isTutorial)
    {
        if (isTutorial)
        {
            waitingTimeTrajectory = waitTimeTrajectory * TimeTickSystem.ticksPerHour;
            TimeTickSystem.OnTick += GenerateTrajectoryEvent;
            waitingTimePrincipal = waitTimeAfterTutorial * TimeTickSystem.ticksPerHour;
        }
        else waitingTimePrincipal = (uint)Random.Range(minWaitTimePrincipal, maxWaitTimePrincipal) * TimeTickSystem.ticksPerHour;
        Debug.Log("Generating new principal event. Maybe nothing will happen.");
        TimeTickSystem.OnTick += WaitStorylinePrincipal;
    }

    private void WaitStorylinePrincipal(object sender, TimeTickSystem.OnTickEventArgs e)
    {
        if (waitingTimePrincipal > 0)
        {
            waitingTimePrincipal -= TimeTickSystem.timePerTick;
            return;
        }
        
        TimeTickSystem.OnTick -= WaitStorylinePrincipal;
        ChooseNewStoryline(SSStoryType.Principal);
    }

    private void WaitStorylineSecondary(object sender, TimeTickSystem.OnTickEventArgs e)
    {
        if (waitingTimeSecondary > 0)
        {
            waitingTimeSecondary -= TimeTickSystem.timePerTick;
            return;
        }

        this.isAlreadyWaiting = false;
        TimeTickSystem.OnTick -= WaitStorylineSecondary;
        ChooseNewStoryline(SSStoryType.Spontaneous);
    }
    
    private void GenerateTrajectoryEvent(object sender, TimeTickSystem.OnTickEventArgs e)
    {
        if (waitingTimeTrajectory > 0)
        {
            waitingTimeTrajectory -= TimeTickSystem.timePerTick;
            return;
        }

        TimeTickSystem.OnTick -= GenerateTrajectoryEvent;
        waitingTimeTrajectory = waitTimeTrajectory * TimeTickSystem.ticksPerHour;
        ChooseNewStoryline(SSStoryType.Trajectory);
    }

    public void ChooseNewStoryline(SSStoryType storyType) {
        if (IsStopChecker) {
            Debug.Log("Checker is stopped.");
            switch (storyType) {
                case SSStoryType.Principal:
                    waitingTimePrincipal = (uint)Random.Range(minWaitTimePrincipal, maxWaitTimePrincipal) *
                                           TimeTickSystem.ticksPerHour;
                    TimeTickSystem.OnTick += WaitStorylinePrincipal;
                    break;
                case SSStoryType.Spontaneous:
                    isAlreadyWaiting = true;
                    waitingTimeSecondary = (uint)Random.Range(minWaitTimeSecondary, maxWaitTimeSecondary) *
                                                TimeTickSystem.ticksPerHour;
                    TimeTickSystem.OnTick += WaitStorylineSecondary;
                    break;
                case SSStoryType.Trajectory:
                    TimeTickSystem.OnTick += GenerateTrajectoryEvent;
                    break;
            }
            return;
        } 
        availableStoryLines.Clear();
        switch (storyType)
        {
            case SSStoryType.Principal:
            {
                if (principalStorylines.Any(storyline => storyline.StorylineContainer.IsTutorialToPlay && storyline.Status == SSStoryStatus.Enabled))
                {
                    PickTimelineFromStoryline(principalStorylines.First(storyline =>
                        storyline.StorylineContainer.IsTutorialToPlay));
                    return;
                }
                for (var index = 0; index < principalStorylines.Count; index++)
                {
                    var storyline = principalStorylines[index];
                    if (storyline.Status == SSStoryStatus.Completed) continue;
                    if (storyline.StorylineContainer.Condition)
                        if (!RouteCondition(storyline.StorylineContainer.Condition))
                            continue;
                    if (activeLaunchers.Any(launcher => launcher.storyline == storyline)) continue;
                    if (storyline.StorylineContainer.IsFirstToPlay)
                    {
                        PickTimelineFromStoryline(storyline);
                        return;
                    }
                    availableStoryLines.Add(storyline);
                }

                break;
            }
            case SSStoryType.Spontaneous:
            {
                for (var index = 0; index < secondaryStorylines.Count; index++)
                {
                    var storyline = secondaryStorylines[index];
                    if (storyline.Status == SSStoryStatus.Completed) continue;
                    if (storyline.StorylineContainer.Condition)
                        if (!RouteCondition(storyline.StorylineContainer.Condition))
                            continue;
                    if (activeLaunchers.Any(launcher => launcher.storyline == storyline)) continue;
                    availableStoryLines.Add(storyline);
                }

                break;
            }
            case SSStoryType.Leak:
                for (int index = 0; index < leakStorylines.Count; index++)
                {
                    var storyline = leakStorylines[index];
                    if (storyline.Status == SSStoryStatus.Completed) continue;
                    if (storyline.StorylineContainer.Condition)
                        if (!RouteCondition(storyline.StorylineContainer.Condition))
                            continue;
                    if (activeLaunchers.Any(launcher => launcher.storyline == storyline)) continue;
                    availableStoryLines.Add(storyline);
                }
                break;
            case SSStoryType.Trajectory:
                for (int index = 0; index < trajectoryStorylines.Count; index++)
                {
                    var storyline = trajectoryStorylines[index];
                    if (storyline.Status == SSStoryStatus.Completed) continue;
                    if (storyline.StorylineContainer.Condition)
                        if (!RouteCondition(storyline.StorylineContainer.Condition))
                            continue;
                    if (activeLaunchers.Any(launcher => launcher.storyline == storyline)) continue;
                    availableStoryLines.Add(storyline);
                }
                break;
        }

        if (availableStoryLines.Count == 0)
        {
            Debug.Log($"All {storyType} storylines are completed or don't have a valid condition.");
            if (storyType == SSStoryType.Principal)
            {
                TimeTickSystem.ModifyTimeScale(0);
                GameManager.Instance.UIManager.PopupEndGame.InitializeEndGame("Earth has been reached. You won the game!", "Victory!");
                TimeTickSystem.OnTick -= WaitStorylinePrincipal;
                TimeTickSystem.OnTick -= WaitStorylineSecondary;
            } 
            return;
        }

        float numberOfASL = availableStoryLines.Count;
        float pickPercent = 1.0f / numberOfASL;
        float randPicker = Random.Range(0.0f, 1.0f);

        for (int i = 1; i <= numberOfASL; i++)
        {
            if (randPicker <= pickPercent * i)
            {
                PickTimelineFromStoryline(availableStoryLines[i - 1]);
                break;
            }
        }
    }

    private void PickTimelineFromStoryline(Storyline storyline)
    {
        availableTimelines.Clear();
        for (var index = 0; index < storyline.Timelines.Count; index++)
        {
            var timeline = storyline.Timelines[index];
            if (timeline.Status != SSStoryStatus.Enabled) continue;
            if (timeline.TimelineContainer.IsFirstToPlay)
            {
                StartTimeline(storyline, timeline.TimelineContainer);
                return;
            }

            if (timeline.TimelineContainer.Condition)
                if (!RouteCondition(timeline.TimelineContainer.Condition))
                    continue;
            availableTimelines.Add(timeline.TimelineContainer);
        }

        if (availableTimelines.Count == 0)
        {
            Debug.Log("All timelines are completed or don't have a valid condition.");
            return;
        }

        var numberOfASL = availableTimelines.Count;
        var pickPercent = 1.0f / numberOfASL;
        var randPicker = Random.Range(0.0f, 1.0f);

        for (int i = 1; i <= numberOfASL; i++)
        {
            if (randPicker <= pickPercent * i)
            {
                StartTimeline(storyline, availableTimelines[i - 1]);
                break;
            }
        }
    }

    public void StartTimeline(Storyline storyline, SSNodeGroupSO timeline, SSNodeSO node = null,
        List<SerializableTuple<string, string>> dialogues = null, List<string> characters = null,
        List<string> assignedCharacters = null, List<string> notAssignedCharacters = null,
        List<string> traitsCharacters = null, uint waitingTime = 0, TaskLog taskLog = null)
    {
        if (storyline.StorylineContainer.StoryType == SSStoryType.Trajectory) TimeTickSystem.OnTick += GenerateTrajectoryEvent;
        presentationContainer.SetActive(true);
        presentationText.text = "New Storyline : " + storyline.StorylineContainer.FileName;
        StartCoroutine(DisablePresentation());
        var count = storyline.StorylineContainer.NodeGroups[timeline].Count;
        var launcherObject = launcherPool.GetFromPool();
        var launcher = launcherObject.GetComponent<SSLauncher>();
        launcher.storyline = storyline;
        launcher.waitingTime = waitingTime;
        launcher.timeline =
            storyline.Timelines.First(timelineToChoose => timelineToChoose.TimelineContainer == timeline);
        activeLaunchers.Add(launcher);
        launcher.nodeContainer = storyline.StorylineContainer;
        launcher.nodeGroup = timeline;
        if (node == null)
        {
            for (int index = 0; index < count; index++)
            {
                if (storyline.StorylineContainer.NodeGroups[timeline][index].IsStartingNode)
                {
                    node = storyline.StorylineContainer.NodeGroups[timeline][index];
                    break;
                }
            }
        }

        launcher.node = node;
        if (dialogues != null) launcher.dialogues = dialogues;
        if (characters != null)
        {
            List<CharacterBehaviour> charactersList = new List<CharacterBehaviour>();
            for (int index = 0; index < characters.Count; index++)
            {
                for (int indexCharacter = 0;
                     indexCharacter < GameManager.Instance.SpaceshipManager.characters.Length;
                     indexCharacter++)
                {
                    var character = GameManager.Instance.SpaceshipManager.characters[indexCharacter];
                    if (character.GetCharacterData().ID == characters[index])
                    {
                        charactersList.Add(character);
                    }
                }
            }

            launcher.characters = charactersList;
        }

        if (notAssignedCharacters != null)
        {
            List<CharacterBehaviour> charactersList = new List<CharacterBehaviour>();
            for (int index = 0; index < notAssignedCharacters.Count; index++)
            {
                for (int indexCharacter = 0;
                     indexCharacter < GameManager.Instance.SpaceshipManager.characters.Length;
                     indexCharacter++)
                {
                    var character = GameManager.Instance.SpaceshipManager.characters[indexCharacter];
                    if (character.GetCharacterData().ID == notAssignedCharacters[index])
                    {
                        charactersList.Add(character);
                    }
                }
            }

            launcher.notAssignedCharacters = charactersList;
        }

        if (assignedCharacters != null)
        {
            List<CharacterBehaviour> charactersList = new List<CharacterBehaviour>();
            for (int index = 0; index < assignedCharacters.Count; index++)
            {
                for (int indexCharacter = 0;
                     indexCharacter < GameManager.Instance.SpaceshipManager.characters.Length;
                     indexCharacter++)
                {
                    var character = GameManager.Instance.SpaceshipManager.characters[indexCharacter];
                    if (character.GetCharacterData().ID == assignedCharacters[index])
                    {
                        charactersList.Add(character);
                    }
                }
            }

            launcher.assignedCharacters = charactersList;
        }

        if (traitsCharacters != null)
        {
            List<CharacterBehaviour> charactersList = new List<CharacterBehaviour>();
            for (int index = 0; index < traitsCharacters.Count; index++)
            {
                for (int indexCharacter = 0;
                     indexCharacter < GameManager.Instance.SpaceshipManager.characters.Length;
                     indexCharacter++)
                {
                    var character = GameManager.Instance.SpaceshipManager.characters[indexCharacter];
                    if (character.GetCharacterData().ID == traitsCharacters[index])
                    {
                        charactersList.Add(character);
                    }
                }
            }

            launcher.traitsCharacters = charactersList;
        }

        if (storyline.StorylineContainer.StoryType == SSStoryType.Spontaneous && !isAlreadyWaiting)
        {
            this.isAlreadyWaiting = true;
            this.waitingTimeSecondary = (uint)Random.Range(minWaitTimeSecondary, maxWaitTimeSecondary) *
                               TimeTickSystem.ticksPerHour;
            TimeTickSystem.OnTick += WaitStorylineSecondary;
        }

        if (taskLog != null)
        {
            launcher.StartTimelineOnTask(taskLog);
        }
        else
        {
            launcher.StartTimeline();
        }
    }

    #region Utilities

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

    private IEnumerator DisablePresentation()
    {
        yield return new WaitForSeconds(5.0f);
        presentationContainer.SetActive(false);
    }

    #endregion

    #region Save&Load

    public void LoadData(GameData gameData)
    {
        for (int index = 0; index < ssCampaigns.Count; index++)
        {
            if (ssCampaigns[index].ID == gameData.currentCampaignID)
            {
                Initialize(ssCampaigns[index]);
                break;
            }

            if (index == ssCampaigns.Count - 1)
            {
                Debug.Log("Campaign was not found in the list of campaigns. Initializing to default.");
                Initialize();
                return;
            }
        }

        for (int indexStoryline = 0; indexStoryline < allStorylines.Count; indexStoryline++)
        {
            var storyline = allStorylines[indexStoryline];
            if (gameData.storylineStatus.ContainsKey(storyline.ID))
            {
                storyline.Status = gameData.storylineStatus[storyline.ID];
                for (int indexTimeline = 0; indexTimeline < storyline.Timelines.Count; indexTimeline++)
                {
                    var timeline = storyline.Timelines[indexTimeline];
                    if (gameData.timelineStatus.ContainsKey(timeline.ID))
                    {
                        timeline.Status = gameData.timelineStatus[timeline.ID];
                    }
                }
            }
        }

        for (int indexActiveStoryline = 0;
             indexActiveStoryline < gameData.activeStorylines.Count;
             indexActiveStoryline++)
        {
            var storyline = gameData.activeStorylines[indexActiveStoryline];
            for (int indexStoryline = 0; indexStoryline < allStorylines.Count; indexStoryline++)
            {
                if (allStorylines[indexStoryline].ID == storyline)
                {
                    var chosenStoryline = allStorylines[indexStoryline];
                    TaskLog taskLog = null;
                    if (gameData.currentTasks.ContainsKey(chosenStoryline.ID))
                    {
                        taskLog = gameData.currentTasks[chosenStoryline.ID];
                    }
                    var waitingTimeTimeline = gameData.waitingTimesTimeline[chosenStoryline.ID];
                    if (gameData.activeTimelines.Count > 0)
                    {
                        for (int index = 0; index < gameData.activeTimelines.Count; index++)
                        {
                            var timeline = gameData.activeTimelines[index];
                            for (int k = 0; k < chosenStoryline.Timelines.Count; k++)
                            {
                                if (chosenStoryline.Timelines[k].ID == timeline)
                                {
                                    var chosenTimeline = chosenStoryline.Timelines[k].TimelineContainer;
                                    if (gameData.currentNodes.TryGetValue(chosenStoryline.ID, out string nodeName))
                                    {
                                        for (int indexNode = 0;
                                             indexNode < chosenStoryline.StorylineContainer.NodeGroups[chosenTimeline]
                                                 .Count;
                                             indexNode++)
                                        {
                                            var node =
                                                chosenStoryline.StorylineContainer
                                                    .NodeGroups[chosenTimeline][indexNode];
                                            if (taskLog != null)
                                            {
                                                if (node.NodeName == taskLog.NodeTaskName)
                                                {
                                                    var dialogues = gameData.dialogueTimelines[chosenStoryline.ID];
                                                    var characters =
                                                        gameData.charactersActiveTimelines[chosenStoryline.ID];
                                                    var assignedCharacters =
                                                        gameData.assignedActiveTimelines[chosenStoryline.ID];
                                                    var notAssignedCharacters =
                                                        gameData.notAssignedActiveTimelines[chosenStoryline.ID];
                                                    var traitsCharacters =
                                                        gameData.traitsCharactersActiveStorylines[chosenStoryline.ID];
                                                    StartTimeline(chosenStoryline, chosenTimeline, node, dialogues,
                                                        characters,
                                                        assignedCharacters, notAssignedCharacters, traitsCharacters,
                                                        waitingTimeTimeline, taskLog);
                                                    break;
                                                }
                                            }
                                            else 
                                            {
                                                if (node.NodeName == nodeName)
                                                {
                                                    var dialogues = gameData.dialogueTimelines[chosenStoryline.ID];
                                                    var characters =
                                                        gameData.charactersActiveTimelines[chosenStoryline.ID];
                                                    var assignedCharacters =
                                                        gameData.assignedActiveTimelines[chosenStoryline.ID];
                                                    var notAssignedCharacters =
                                                        gameData.notAssignedActiveTimelines[chosenStoryline.ID];
                                                    var traitsCharacters =
                                                        gameData.traitsCharactersActiveStorylines[chosenStoryline.ID];
                                                    StartTimeline(chosenStoryline, chosenTimeline, node, dialogues,
                                                        characters,
                                                        assignedCharacters, notAssignedCharacters, traitsCharacters,
                                                        waitingTimeTimeline);
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    break;
                                }
                            }
                        }
                    }

                    break;
                }
            }
        }
        
        TimeTickSystem.OnTick += GenerateTrajectoryEvent;
    }

    public void SaveData(ref GameData gameData)
    {
        gameData.currentCampaignID = ssCampaign.ID;

        gameData.storylineStatus.Clear();
        gameData.timelineStatus.Clear();

        for (int indexStoryline = 0; indexStoryline < allStorylines.Count; indexStoryline++)
        {
            var storyline = allStorylines[indexStoryline];
            gameData.storylineStatus.Add(storyline.ID, storyline.Status);
            for (int indexTimeline = 0; indexTimeline < storyline.Timelines.Count; indexTimeline++)
            {
                var timeline = storyline.Timelines[indexTimeline];
                gameData.timelineStatus.Add(timeline.ID, timeline.Status);
            }
        }

        gameData.activeStorylines.Clear();

        for (int index = 0; index < activeLaunchers.Count; index++)
        {
            var storyline = activeLaunchers[index].storyline;
            gameData.activeStorylines.Add(storyline.ID);
        }

        gameData.activeTimelines.Clear();
        gameData.waitingTimesTimeline.Clear();

        for (int indexLauncher = 0; indexLauncher < activeLaunchers.Count; indexLauncher++)
        {
            var launcher = activeLaunchers[indexLauncher];
            if (!launcher.IsRunning) continue;
            gameData.waitingTimesTimeline.Add(launcher.storyline.ID, launcher.waitingTime);
            for (int index = 0; index < allStorylines.Count; index++)
            {
                var storyline = allStorylines[index];
                var result = storyline.Timelines.Where(timeline => timeline.TimelineContainer == launcher.nodeGroup);
                if (result.Any())
                {
                    gameData.activeTimelines.Add(result.First().ID);
                    break;
                }
            }
        }

        gameData.currentNodes.Clear();
        gameData.currentTasks.Clear();
        gameData.dialogueTimelines.Clear();
        gameData.charactersActiveTimelines.Clear();
        gameData.assignedActiveTimelines.Clear();
        gameData.notAssignedActiveTimelines.Clear();
        gameData.traitsCharactersActiveStorylines.Clear();

        for (int indexLauncher = 0; indexLauncher < activeLaunchers.Count; indexLauncher++)
        {
            var launcher = activeLaunchers[indexLauncher];
            if (launcher.IsRunning)
            {
                for (int index = 0; index < allStorylines.Count; index++)
                {
                    var storyline = allStorylines[index];
                    if (storyline.StorylineContainer == launcher.nodeContainer)
                    {
                        if (launcher.task is not null)
                        {
                            List<string> leaderCharacters = new List<string>();
                            for (int indexLeader = 0; indexLeader < launcher.task.leaderCharacters.Count; indexLeader++)
                            {
                                leaderCharacters.Add(launcher.task.leaderCharacters[indexLeader].GetCharacterData().ID);
                            }
                            
                            List<string> assistantCharacters = new List<string>();
                            for (int indexAssistant = 0;
                                 indexAssistant < launcher.task.assistantCharacters.Count;
                                 indexAssistant++)
                            {
                                assistantCharacters.Add(launcher.task.assistantCharacters[indexAssistant]
                                    .GetCharacterData().ID);
                            }

                            var taskLog = new TaskLog(launcher.task.Name, launcher.task.Duration, launcher.task.TimeLeft, launcher.task.IsStarted,
                                leaderCharacters, assistantCharacters);
                            gameData.currentTasks.Add(storyline.ID, taskLog);
                        }

                        gameData.currentNodes.Add(storyline.ID, launcher.CurrentNode.NodeName);
                        gameData.dialogueTimelines.Add(storyline.ID, launcher.dialogues);
                        List<string> charactersID = new List<string>();
                        for (int indexCharacter = 0; indexCharacter < launcher.characters.Count; indexCharacter++)
                        {
                            charactersID.Add(launcher.characters[indexCharacter].GetCharacterData().ID);
                        }

                        gameData.charactersActiveTimelines.Add(storyline.ID, charactersID);
                        List<string> assignedCharactersID = new List<string>();
                        for (int indexAssigned = 0;
                             indexAssigned < launcher.assignedCharacters.Count;
                             indexAssigned++)
                        {
                            assignedCharactersID.Add(launcher.assignedCharacters[indexAssigned].GetCharacterData().ID);
                        }

                        gameData.assignedActiveTimelines.Add(storyline.ID, assignedCharactersID);
                        List<string> notAssignedCharactersID = new List<string>();
                        for (int indexNotAssigned = 0;
                             indexNotAssigned < launcher.notAssignedCharacters.Count;
                             indexNotAssigned++)
                        {
                            notAssignedCharactersID.Add(launcher.notAssignedCharacters[indexNotAssigned].GetCharacterData().ID);
                        }

                        gameData.notAssignedActiveTimelines.Add(storyline.ID, notAssignedCharactersID);
                        List<string> traitsCharactersID = new List<string>();
                        for (int indexTraits = 0; indexTraits < launcher.traitsCharacters.Count; indexTraits++)
                        {
                            traitsCharactersID.Add(launcher.traitsCharacters[indexTraits].GetCharacterData().ID);
                        }

                        gameData.traitsCharactersActiveStorylines.Add(storyline.ID, traitsCharactersID);
                        break;
                    }
                }
            }
        }

        gameData.allStorylineLogs.Clear();

        gameData.allStorylineLogs = allStorylineLogs;
    }

    #endregion
}