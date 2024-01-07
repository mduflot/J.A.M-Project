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

    [SerializeField] private List<SSCampaignSO> ssCampaigns;
    [SerializeField] private GameObject presentationContainer;
    [SerializeField] private TextMeshProUGUI presentationText;

    [SerializeField] private GameObject launcherPrefab;
    [SerializeField] private uint minWaitTimeMain = 5;
    [SerializeField] private uint maxWaitTimeMain = 10;
    [SerializeField] private uint minWaitTimeSecondary = 10;
    [SerializeField] private uint maxWaitTimeSecondary = 20;

    private List<SSLauncher> activeLaunchers;

    private SSCampaignSO ssCampaign;

    private Storyline chosenStoryline;
    private SSNodeGroupSO chosenTimeline;

    private List<Storyline> principalStorylines;
    private List<Storyline> secondaryStorylines;

    private List<Storyline> availableStoryLines = new();
    private List<SSNodeGroupSO> availableTimelines = new();

    private uint waitingTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            Debug.LogError("Found more than one Checker in the scene.");
            return;
        }

        Instance = this;
    }

    public void Initialize(SSCampaignSO campaign = null)
    {
        launcherPool = new Pool<GameObject>(launcherPrefab, 3);
        activeLaunchers = new();
        ssCampaign = campaign != null ? campaign : ssCampaigns[0];
        principalStorylines = ssCampaign.PrincipalStorylines;
        secondaryStorylines = ssCampaign.SecondaryStorylines;
    }

    public void GenerateNewEvent()
    {
        if (activeLaunchers.Count < 1)
        {
            ChooseNewStoryline(SSStoryType.Principal); 
            return;
        }
        else 
        {
            for (int index = 0; index < activeLaunchers.Count; index++)
            {
                var launcher = activeLaunchers[index];
                if (launcher.nodeContainer.StoryType == SSStoryType.Principal && launcher.storyline.Status == SSStoryStatus.Completed)
                {
                    ChooseNewStoryline(SSStoryType.Principal);
                    return;
                }
            }

            ChooseNewStoryline(SSStoryType.Secondary);
        }
    }

    private void WaitStoryline(object sender, TimeTickSystem.OnTickEventArgs e)
    {
        if (waitingTime > 0)
        {
            waitingTime -= TimeTickSystem.timePerTick;
            return;
        }

        TimeTickSystem.OnTick -= WaitStoryline;
        ChooseNewStoryline(SSStoryType.Secondary);
    }

    private void ChooseNewStoryline(SSStoryType storyType)
    {
        availableStoryLines.Clear();
        switch (storyType)
        {
            case SSStoryType.Principal:
            {
                for (var index = 0; index < principalStorylines.Count; index++)
                {
                    var storyline = principalStorylines[index];
                    if (storyline.StorylineContainer.StoryStatus != SSStoryStatus.Enabled) continue;
                    if (storyline.StorylineContainer.Condition)
                        if (RouteCondition(storyline.StorylineContainer.Condition))
                            continue;
                    availableStoryLines.Add(storyline);
                }

                break;
            }
            case SSStoryType.Secondary:
            {
                for (var index = 0; index < secondaryStorylines.Count; index++)
                {
                    var storyline = secondaryStorylines[index];
                    if (storyline.StorylineContainer.StoryStatus != SSStoryStatus.Enabled) continue;
                    if (storyline.StorylineContainer.Condition)
                        if (RouteCondition(storyline.StorylineContainer.Condition))
                            continue;
                    availableStoryLines.Add(storyline);
                }

                break;
            }
        }

        if (availableStoryLines.Count == 0)
        {
            Debug.LogError($"All {storyType} storylines are completed or don't have a valid condition.");
            return;
        }

        float numberOfASL = availableStoryLines.Count;
        float pickPercent = 1.0f / numberOfASL;
        float randPicker = Random.Range(0.0f, 1.0f);

        for (int i = 1; i <= numberOfASL; i++)
        {
            if (randPicker <= pickPercent * i)
            {
                chosenStoryline = availableStoryLines[i - 1];
                PickTimelineFromStoryline();
                break;
            }
        }
    }

    private void PickTimelineFromStoryline()
    {
        for (var index = 0; index < chosenStoryline.Timelines.Count; index++)
        {
            var timeline = chosenStoryline.Timelines[index];
            if (timeline.Status != SSStoryStatus.Enabled) continue;
            if (timeline.TimelineContainer.IsFirstToPlay)
            {
                StartTimeline(timeline.TimelineContainer);
                return;
            }

            if (timeline.TimelineContainer.Condition)
                if (RouteCondition(timeline.TimelineContainer.Condition))
                    continue;
            availableTimelines.Add(timeline.TimelineContainer);
        }

        if (availableTimelines.Count == 0)
        {
            Debug.LogError("All timelines are completed or don't have a valid condition.");
            return;
        }

        var numberOfASL = availableTimelines.Count;
        var pickPercent = 1.0f / numberOfASL;
        var randPicker = Random.Range(0.0f, 1.0f);

        for (int i = 1; i <= numberOfASL; i++)
        {
            if (randPicker <= pickPercent * i)
            {
                StartTimeline(availableTimelines[i - 1]);
                break;
            }
        }
    }

    private void StartTimeline(SSNodeGroupSO timeline, SSNodeSO node = null,
        List<SerializableTuple<string, string>> dialogues = null, List<string> characters = null,
        List<string> assignedCharacters = null, List<string> notAssignedCharacters = null,
        List<string> traitsCharacters = null)
    {
        chosenTimeline = timeline;
        presentationContainer.SetActive(true);
        presentationText.text = "New Storyline : " + chosenStoryline.StorylineContainer.FileName;
        StartCoroutine(DisablePresentation());
        var count = chosenStoryline.StorylineContainer.NodeGroups[chosenTimeline].Count;
        var launcherObject = launcherPool.GetFromPool();
        var launcher = launcherObject.GetComponent<SSLauncher>();
        launcher.nodeContainer = chosenStoryline.StorylineContainer;
        launcher.nodeGroup = chosenTimeline;
        if (node == null)
        {
            for (int index = 0; index < count; index++)
            {
                if (chosenStoryline.StorylineContainer.NodeGroups[chosenTimeline][index].IsStartingNode)
                {
                    node = chosenStoryline.StorylineContainer.NodeGroups[chosenTimeline][index];
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

        if (chosenStoryline.StorylineContainer.StoryType == SSStoryType.Secondary)
        {
            waitingTime = (uint) Random.Range(minWaitTimeSecondary, maxWaitTimeSecondary);
            TimeTickSystem.OnTick += WaitStoryline;
        }
        launcher.StartTimeline();
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
                Debug.LogError("Campaign was not found in the list of campaigns. Initializing to default.");
                Initialize();
                return;
            }
        }

        for (int indexStoryline = 0; indexStoryline < principalStorylines.Count; indexStoryline++)
        {
            var storyline = principalStorylines[indexStoryline];
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

        for (int indexStoryline = 0; indexStoryline < secondaryStorylines.Count; indexStoryline++)
        {
            var storyline = secondaryStorylines[indexStoryline];
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
            for (int indexStoryline = 0; indexStoryline < ssCampaign.Storylines.Count; indexStoryline++)
            {
                if (ssCampaign.Storylines[indexStoryline].ID == storyline)
                {
                    chosenStoryline = ssCampaign.Storylines[indexStoryline];
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
                                            if (node.NodeName == nodeName)
                                            {
                                                var dialogues = gameData.dialogueTimelines[chosenStoryline.ID];
                                                var characters = gameData.charactersActiveTimelines[chosenStoryline.ID];
                                                var assignedCharacters =
                                                    gameData.assignedActiveTimelines[chosenStoryline.ID];
                                                var notAssignedCharacters =
                                                    gameData.notAssignedActiveTimelines[chosenStoryline.ID];
                                                var traitsCharacters =
                                                    gameData.traitsCharactersActiveStorylines[chosenStoryline.ID];
                                                StartTimeline(chosenTimeline, node, dialogues, characters,
                                                    assignedCharacters, notAssignedCharacters, traitsCharacters);
                                                break;
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
    }

    public void SaveData(ref GameData gameData)
    {
        gameData.currentCampaignID = ssCampaign.ID;

        gameData.storylineStatus.Clear();
        gameData.timelineStatus.Clear();

        for (int indexStoryline = 0; indexStoryline < principalStorylines.Count; indexStoryline++)
        {
            var storyline = principalStorylines[indexStoryline];
            gameData.storylineStatus.Add(storyline.ID, storyline.Status);
            for (int indexTimeline = 0; indexTimeline < storyline.Timelines.Count; indexTimeline++)
            {
                var timeline = storyline.Timelines[indexTimeline];
                gameData.timelineStatus.Add(timeline.ID, timeline.Status);
            }
        }

        for (int indexStoryline = 0; indexStoryline < secondaryStorylines.Count; indexStoryline++)
        {
            var storyline = secondaryStorylines[indexStoryline];
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

        for (int indexLauncher = 0; indexLauncher < activeLaunchers.Count; indexLauncher++)
        {
            var launcher = activeLaunchers[indexLauncher];
            if (!launcher.IsRunning) continue;
            for (int index = 0; index < principalStorylines.Count; index++)
            {
                var storyline = principalStorylines[index];
                var result = storyline.Timelines.Where(timeline => timeline.TimelineContainer == launcher.nodeGroup);
                if (result.Any())
                {
                    gameData.activeTimelines.Add(result.First().ID);
                    break;
                }
            }
        }

        gameData.currentNodes.Clear();
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
                for (int index = 0; index < principalStorylines.Count; index++)
                {
                    var storyline = principalStorylines[index];
                    if (storyline.StorylineContainer == launcher.nodeContainer)
                    {
                        gameData.currentNodes.Add(storyline.ID, launcher.CurrentNode.NodeName);
                        gameData.dialogueTimelines.Add(storyline.ID, launcher.dialogues);
                        List<string> charactersID = new List<string>();
                        for (int indexCharacter = 0; indexCharacter < launcher.characters.Count; indexCharacter++)
                        {
                            charactersID.Add(launcher.characters[indexCharacter].GetCharacterData().ID);
                        }

                        gameData.charactersActiveTimelines.Add(storyline.ID, charactersID);
                        charactersID.Clear();
                        for (int indexAssigned = 0;
                            indexAssigned < launcher.assignedCharacters.Count;
                            indexAssigned++)
                        {
                            charactersID.Add(launcher.assignedCharacters[indexAssigned].GetCharacterData().ID);
                        }

                        gameData.assignedActiveTimelines.Add(storyline.ID, charactersID);
                        charactersID.Clear();
                        for (int indexNotAssigned = 0;
                            indexNotAssigned < launcher.notAssignedCharacters.Count;
                            indexNotAssigned++)
                        {
                            charactersID.Add(launcher.notAssignedCharacters[indexNotAssigned].GetCharacterData().ID);
                        }

                        gameData.notAssignedActiveTimelines.Add(storyline.ID, charactersID);
                        charactersID.Clear();
                        for (int indexTraits = 0; indexTraits < launcher.traitsCharacters.Count; indexTraits++)
                        {
                            charactersID.Add(launcher.traitsCharacters[indexTraits].GetCharacterData().ID);
                        }

                        gameData.traitsCharactersActiveStorylines.Add(storyline.ID, charactersID);
                        break;
                    }
                }
            }
        }

        gameData.principalStorylineLogs.Clear();
        gameData.secondaryStorylineLogs.Clear();
        gameData.trivialStorylineLogs.Clear();
    }

    #endregion
}