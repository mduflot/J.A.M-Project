﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SS;
using SS.Enumerations;
using SS.ScriptableObjects;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Checker : MonoBehaviour, IDataPersistence
{
    [SerializeField] private List<SSCampaignSO> ssCampaigns;
    [SerializeField] private SSLauncher principalLauncher;
    [SerializeField] private SSLauncher secondaryLauncher;
    [SerializeField] private SSLauncher trivialLauncher;
    [SerializeField] private GameObject presentationContainer;
    [SerializeField] private TextMeshProUGUI presentationText;

    private SSCampaignSO ssCampaign;

    private float priorityFactor;
    private Storyline chosenStoryline;
    private SSNodeGroupSO chosenTimeline;

    private List<Storyline> principalStorylines;
    private List<Storyline> secondaryStorylines;
    private List<Storyline> trivialStorylines;

    private List<Storyline> activeStorylines = new();
    private List<Storyline> availableStoryLines = new();
    private List<SSNodeGroupSO> availableTimelines = new();

    public void Initialize(SSCampaignSO campaign = null)
    {
        ssCampaign = campaign != null ? campaign : ssCampaigns[0];
        principalStorylines = ssCampaign.PrincipalStorylines;
        secondaryStorylines = ssCampaign.SecondaryStorylines;
        trivialStorylines = ssCampaign.TrivialStorylines;
        // GenerateRandomEvent();
    }

    public void GenerateRandomEvent()
    {
        var enabledPStorylines =
            principalStorylines.Where(
                ((storyline => storyline.StorylineContainer.StoryStatus == SSStoryStatus.Enabled)));
        var enabledSStorylines =
            secondaryStorylines.Where(
                ((storyline => storyline.StorylineContainer.StoryStatus == SSStoryStatus.Enabled)));
        var enabledTStorylines =
            trivialStorylines.Where(((storyline => storyline.StorylineContainer.StoryStatus == SSStoryStatus.Enabled)));

        if ((!enabledPStorylines.Any() && !enabledSStorylines.Any() && !enabledTStorylines.Any()) ||
            (principalLauncher.IsRunning && secondaryLauncher.IsRunning && trivialLauncher.IsRunning))
        {
            Debug.LogError($"No storylines available or all launchers are running");
            return;
        }

        if (activeStorylines.Count == 0)
        {
            for (int index = 0; index < principalStorylines.Count; index++)
            {
                var storyline = principalStorylines[index];
                if (storyline.StorylineContainer.IsFirstToPlay)
                {
                    chosenStoryline = storyline;
                    activeStorylines.Add(chosenStoryline);
                    PickTimelineFromStoryline(true);
                    return;
                }
            }

            SSStoryType storyType = (SSStoryType)Random.Range(0, 3);

            ChooseNewStoryline(storyType);
            return;
        }

        if (activeStorylines.Count < 3)
        {
            var missingTypes = new List<SSStoryType>()
            {
                SSStoryType.Principal, SSStoryType.Secondary, SSStoryType.Trivial
            };
            var activeTypes = new List<SSStoryType>();
            for (int index = 0; index < activeStorylines.Count; index++)
            {
                if (activeStorylines[index].StorylineContainer.StoryType == SSStoryType.Principal ||
                    principalStorylines.Count == 0)
                {
                    if (!principalLauncher.IsRunning)
                        activeTypes.Add(SSStoryType.Principal);

                    missingTypes.Remove(SSStoryType.Principal);
                }

                if (activeStorylines[index].StorylineContainer.StoryType == SSStoryType.Secondary ||
                    secondaryStorylines.Count == 0)
                {
                    if (!secondaryLauncher.IsRunning)
                        activeTypes.Add(SSStoryType.Secondary);

                    missingTypes.Remove(SSStoryType.Secondary);
                }

                if (activeStorylines[index].StorylineContainer.StoryType == SSStoryType.Trivial ||
                    trivialStorylines.Count == 0)
                {
                    if (!trivialLauncher.IsRunning)
                        activeTypes.Add(SSStoryType.Trivial);

                    missingTypes.Remove(SSStoryType.Trivial);
                }
            }

            float pickPercent = .33f;
            float inactivePickPercent = pickPercent * missingTypes.Count;
            float randPicker = Random.Range(0.0f, 1.0f);

            SSStoryType storyType;
            if (randPicker < inactivePickPercent && missingTypes.Count > 0)
                storyType = missingTypes[Random.Range(0, missingTypes.Count)];
            else if (activeTypes.Count > 0)
                storyType = activeTypes[Random.Range(0, activeTypes.Count)];
            else
            {
                Debug.Log("No storylines available");
                return;
            }

            ChooseNewStoryline(storyType);
        }
        else
        {
            PickTimelineFromStoryline();
        }
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
            case SSStoryType.Trivial:
            {
                for (var index = 0; index < trivialStorylines.Count; index++)
                {
                    var storyline = trivialStorylines[index];
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
            Debug.Log("No storylines available");
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
                activeStorylines.Add(chosenStoryline);
                PickTimelineFromStoryline(true);
                break;
            }
        }
    }

    private void PickTimelineFromStoryline(bool isNewStoryline = false)
    {
        if (!isNewStoryline)
        {
            for (var index = 0; index < activeStorylines.Count; index++)
            {
                bool isAllCompleted = true;
                var storyline = activeStorylines[index];
                for (int j = 0; j < storyline.Timelines.Count; j++)
                {
                    if (storyline.Timelines[j].Status == SSStoryStatus.Enabled)
                    {
                        isAllCompleted = false;
                        break;
                    }
                }

                if (isAllCompleted)
                {
                    storyline.StorylineContainer.StoryStatus = SSStoryStatus.Completed;
                    activeStorylines.Remove(storyline);
                    if (activeStorylines.Count == 0)
                    {
                        Debug.Log($"All timelines from {storyline.StorylineContainer.FileName} are completed");
                        GenerateRandomEvent();
                        return;
                    }
                }
            }

            chosenStoryline = activeStorylines[Random.Range(0, activeStorylines.Count)];
        }

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
            Debug.Log("No timelines available");
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

    private void StartTimeline(SSNodeGroupSO timeline)
    {
        chosenTimeline = timeline;
        SSNodeSO node = null;
        presentationContainer.SetActive(true);
        presentationText.text = "New Storyline : " + chosenStoryline.StorylineContainer.FileName;
        StartCoroutine(DisablePresentation());
        Debug.Log($"Key : {chosenTimeline}");
        var count = chosenStoryline.StorylineContainer.NodeGroups[chosenTimeline].Count;
        switch (chosenStoryline.StorylineContainer.StoryType)
        {
            case SSStoryType.Principal:
                principalLauncher.nodeContainer = chosenStoryline.StorylineContainer;
                principalLauncher.nodeGroup = chosenTimeline;
                for (int index = 0; index < count; index++)
                {
                    if (chosenStoryline.StorylineContainer.NodeGroups[chosenTimeline][index].IsStartingNode)
                    {
                        node = chosenStoryline.StorylineContainer.NodeGroups[chosenTimeline][index];
                        break;
                    }
                }

                principalLauncher.node = node;
                principalLauncher.StartTimeline();
                break;
            case SSStoryType.Secondary:
                secondaryLauncher.nodeContainer = chosenStoryline.StorylineContainer;
                secondaryLauncher.nodeGroup = chosenTimeline;
                for (int index = 0; index < count; index++)
                {
                    if (chosenStoryline.StorylineContainer.NodeGroups[chosenTimeline][index].IsStartingNode)
                    {
                        node = chosenStoryline.StorylineContainer.NodeGroups[chosenTimeline][index];
                        break;
                    }
                }

                secondaryLauncher.node = node;
                secondaryLauncher.StartTimeline();
                break;
            case SSStoryType.Trivial:
                trivialLauncher.nodeContainer = chosenStoryline.StorylineContainer;
                trivialLauncher.nodeGroup = chosenTimeline;
                for (int index = 0; index < count; index++)
                {
                    if (chosenStoryline.StorylineContainer.NodeGroups[chosenTimeline][index].IsStartingNode)
                    {
                        node = chosenStoryline.StorylineContainer.NodeGroups[chosenTimeline][index];
                        break;
                    }
                }

                trivialLauncher.node = node;
                trivialLauncher.StartTimeline();
                break;
        }
    }

    private IEnumerator DisablePresentation()
    {
        yield return new WaitForSeconds(5.0f);
        presentationContainer.SetActive(false);
    }

    public void LoadData(GameData gameData)
    {
        // Give the campaign ID to the checker and load the campaign
        for (int index = 0; index < ssCampaigns.Count; index++)
        {
            if (ssCampaigns[index].ID == gameData.currentCampaignID)
            {
                Initialize(ssCampaigns[index]);
                break;
            }

            if (index == ssCampaigns.Count - 1)
            {
                Debug.LogError("Campaign not found");
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

        for (int indexStoryline = 0; indexStoryline < trivialStorylines.Count; indexStoryline++)
        {
            var storyline = trivialStorylines[indexStoryline];
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

        activeStorylines.Clear();

        for (int index = 0; index < gameData.activeStorylines.Count; index++)
        {
            var storyline = gameData.activeStorylines[index];
            for (int j = 0; j < ssCampaign.Storylines.Count; j++)
            {
                if (ssCampaign.Storylines[j].ID == storyline)
                {
                    activeStorylines.Add(ssCampaign.Storylines[j]);
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

        for (int indexStoryline = 0; indexStoryline < trivialStorylines.Count; indexStoryline++)
        {
            var storyline = trivialStorylines[indexStoryline];
            gameData.storylineStatus.Add(storyline.ID, storyline.Status);
            for (int indexTimeline = 0; indexTimeline < storyline.Timelines.Count; indexTimeline++)
            {
                var timeline = storyline.Timelines[indexTimeline];
                gameData.timelineStatus.Add(timeline.ID, timeline.Status);
            }
        }

        gameData.activeStorylines = new List<string>();

        for (int index = 0; index < activeStorylines.Count; index++)
        {
            var storyline = activeStorylines[index];
            gameData.activeStorylines.Add(storyline.ID);
        }
    }
}