using System.Collections.Generic;
using SS;
using SS.Enumerations;
using SS.ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

public class Checker : MonoBehaviour
{
    [SerializeField] private SSCampaignSO ssCampaign;
    [SerializeField] private SSLauncher principalLauncher;
    [SerializeField] private SSLauncher secondaryLauncher;
    [SerializeField] private SSLauncher trivialLauncher;

    private float priorityFactor;
    private Storyline chosenStoryline;
    private SSNodeGroupSO chosenTimeline;

    private List<Storyline> principalStorylines;
    private List<Storyline> secondaryStorylines;
    private List<Storyline> trivialStorylines;

    private List<Storyline> activeStorylines = new();
    private List<Storyline> availableStoryLines = new();
    private List<SSNodeGroupSO> availableTimelines = new();

    [ContextMenu("Initialize")]
    public void Initialize()
    {
        principalStorylines = ssCampaign.PrincipalStorylines;
        secondaryStorylines = ssCampaign.SecondaryStorylines;
        trivialStorylines = ssCampaign.TrivialStorylines;
    }

    [ContextMenu("GenerateRandomEvent")]
    public void GenerateRandomEvent()
    {
        if (principalStorylines.Count == 0 && secondaryStorylines.Count == 0 && trivialStorylines.Count == 0)
        {
            Debug.Log("No storylines available");
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

            // TODO : DO PROBABILITY CALCULATIONS HERE

            ChooseNewStoryline(SSStoryType.Principal);
            return;
        }

        if (activeStorylines.Count < 3)
        {
            // pickPercent : float, % chance for an outcome
            // weighedActivePercent : float, probability for an active timeline to be selected
            // weighedInactivePercent : float, probability for an inactive timeline to be selected
            // randPicker : float, random value in [0,100] used to chose among available or unavailable storylines

            var missingTypes = new List<SSStoryType>()
            {
                SSStoryType.Principal, SSStoryType.Secondary, SSStoryType.Trivial
            };
            for (int index = 0; index < activeStorylines.Count; index++)
            {
                if (activeStorylines[index].StorylineContainer.StoryType == SSStoryType.Principal ||
                    principalStorylines.Count == 0)
                    missingTypes.Remove(SSStoryType.Principal);
                if (activeStorylines[index].StorylineContainer.StoryType == SSStoryType.Secondary ||
                    secondaryStorylines.Count == 0)
                    missingTypes.Remove(SSStoryType.Secondary);
                if (activeStorylines[index].StorylineContainer.StoryType == SSStoryType.Trivial ||
                    trivialStorylines.Count == 0)
                    missingTypes.Remove(SSStoryType.Trivial);
            }

            // TODO : DO PROBABILITY CALCULATIONS HERE

            var storyType = missingTypes[Random.Range(0, missingTypes.Count)];

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
                    if (storyline.StorylineContainer.Condition) if (RouteCondition(storyline.StorylineContainer.Condition)) continue;
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
                    if (storyline.StorylineContainer.Condition) if (RouteCondition(storyline.StorylineContainer.Condition)) continue;
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
                    if (storyline.StorylineContainer.Condition) if (RouteCondition(storyline.StorylineContainer.Condition)) continue;
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

        // numberOfASL = availableStoryLines.length
        // pickPercent = 100.0/numberOfASL
        // randPicker = random(0.0,100.0)

        var numberOfASL = availableStoryLines.Count;
        var pickPercent = 100.0f / numberOfASL;
        var randPicker = Random.Range(0, 100);

        for (int i = 0; i < numberOfASL; i++)
        {
            // if (randPicker <= pickPercent * i)
            if (true)
            {
                chosenStoryline = availableStoryLines[i];
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
                    if (storyline.Timelines[index].StoryStatus == SSStoryStatus.Enabled)
                    {
                        isAllCompleted = false;
                        break;
                    }
                }

                if (isAllCompleted)
                {
                    activeStorylines.Remove(storyline);
                    if (activeStorylines.Count == 0)
                    {
                        Debug.Log("No storylines available");
                        GenerateRandomEvent();
                        return;
                    }
                }
            }

            // TODO : DO PROBABILITY CALCULATIONS HERE

            chosenStoryline = activeStorylines[Random.Range(0, activeStorylines.Count)];
        }

        for (var index = 0; index < chosenStoryline.Timelines.Count; index++)
        {
            var timeline = chosenStoryline.Timelines[index];
            if (timeline.StoryStatus != SSStoryStatus.Enabled) continue;
            if (timeline.IsFirstToPlay)
            {
                StartTimeline(timeline);
                return;
            }

            if (timeline.Condition) if (RouteCondition(timeline.Condition)) continue;
            availableTimelines.Add(timeline);
        }
        
        if (availableTimelines.Count == 0)
        {
            Debug.Log("No timelines available");
            return;
        }

        var numberOfASL = availableTimelines.Count;
        var pickPercent = 100.0f / numberOfASL;
        var randPicker = Random.Range(0, 100);

        for (int i = 0; i < numberOfASL; i++)
        {
            // if (randPicker <= pickPercent * i)
            if (true)
            {
                StartTimeline(availableTimelines[i]);
                break;
            }
        }
    }
    
    private bool RouteCondition(ConditionSO condition)
    {
        bool validateCondition = false;
        switch (condition.target)
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
}