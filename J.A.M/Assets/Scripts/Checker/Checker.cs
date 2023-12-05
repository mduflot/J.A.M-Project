using System.Collections.Generic;
using SS;
using SS.Enumerations;
using SS.ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

public class Checker : MonoBehaviour
{
    [SerializeField] private SSNode node;

    private float priorityFactor;
    private Storyline chosenStoryline;
    private SSNodeGroupSO chosenTimeline;

    private List<Storyline> principalStorylines;
    private List<Storyline> secondaryStorylines;
    private List<Storyline> trivialStorylines;

    private List<Storyline> activeStorylines = new();
    private List<Storyline> availableStoryLines = new();
    private List<SSNodeGroupSO> availableTimelines = new();

    public void Initialize(SSCampaignSO ssCampaign)
    {
        principalStorylines = ssCampaign.PrincipalStorylines;
        secondaryStorylines = ssCampaign.SecondaryStorylines;
        trivialStorylines = ssCampaign.TrivialStorylines;
    }

    [ContextMenu("GenerateRandomEvent")]
    public void GenerateRandomEvent()
    {
        if (activeStorylines.Count == 0) ChooseNewStoryline(SSStoryType.Principal);
        if (activeStorylines.Count < 3)
        {
            // pickPercent : float, % chance for an outcome
            // weighedActivePercent : float, probability for an active timeline to be selected
            // weighedInactivePercent : float, probability for an inactive timeline to be selected
            // randPicker : float, random value in [0,100] used to chose among available or unavailable storylines

            var missingStoryTypes = new List<SSStoryType>();
            for (int index = 0; index < activeStorylines.Count; index++)
            {
                if (activeStorylines[index].StorylineContainer.StoryType == SSStoryType.Principal)
                    missingStoryTypes.Add(SSStoryType.Principal);
                if (activeStorylines[index].StorylineContainer.StoryType == SSStoryType.Secondary)
                    missingStoryTypes.Add(SSStoryType.Secondary);
                if (activeStorylines[index].StorylineContainer.StoryType == SSStoryType.Trivial)
                    missingStoryTypes.Add(SSStoryType.Trivial);
            }

            // TODO : DO PROBABILITY CALCULATIONS HERE

            var storyType = missingStoryTypes[Random.Range(0, missingStoryTypes.Count)];
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
                    if (activeStorylines.Contains(storyline)) continue;
                    if (storyline.StorylineContainer.StoryStatus != SSStoryStatus.Enabled) continue;
                    // TODO : CHECK CONDITIONS
                    availableStoryLines.Add(storyline);
                }

                break;
            }
            case SSStoryType.Secondary:
            {
                for (var index = 0; index < secondaryStorylines.Count; index++)
                {
                    var storyline = secondaryStorylines[index];
                    if (activeStorylines.Contains(storyline)) continue;
                    if (storyline.StorylineContainer.StoryStatus != SSStoryStatus.Enabled) continue;
                    // TODO : CHECK CONDITIONS
                    availableStoryLines.Add(storyline);
                }

                break;
            }
            case SSStoryType.Trivial:
            {
                for (var index = 0; index < trivialStorylines.Count; index++)
                {
                    var storyline = trivialStorylines[index];
                    if (activeStorylines.Contains(storyline)) continue;
                    if (storyline.StorylineContainer.StoryStatus != SSStoryStatus.Enabled) continue;
                    // TODO : CHECK CONDITIONS
                    availableStoryLines.Add(storyline);
                }

                break;
            }
        }

        // numberOfASL = availableStoryLines.length
        // pickPercent = 100.0/numberOfASL
        // randPicker = random(0.0,100.0)

        var numberOfASL = availableStoryLines.Count;
        var pickPercent = 100.0f / numberOfASL;
        var randPicker = Random.Range(0, 100);

        for (int i = 0; i < numberOfASL; i++)
        {
            if (randPicker <= pickPercent * i)
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
                var storyline = activeStorylines[index];
                if (storyline.Timelines.Count == 0)
                {
                    storyline.StorylineContainer.StoryStatus = SSStoryStatus.Completed;
                    activeStorylines.Remove(storyline);
                }
            }

            if (activeStorylines.Count == 0)
            {
                GenerateRandomEvent();
                return;
            }

            // TODO : DO PROBABILITY CALCULATIONS HERE

            chosenStoryline = activeStorylines[Random.Range(0, activeStorylines.Count)];
        }

        foreach (var timeline in chosenStoryline.Timelines)
        {
            if (timeline.StoryStatus != SSStoryStatus.Enabled) continue;
            // TODO : CHECK CONDITIONS
            availableTimelines.Add(timeline);
        }

        var numberOfASL = availableTimelines.Count;
        var pickPercent = 100.0f / numberOfASL;
        var randPicker = Random.Range(0, 100);

        for (int i = 0; i < numberOfASL; i++)
        {
            if (randPicker <= pickPercent * i)
            {
                chosenTimeline = availableTimelines[i];
                node.nodeContainer = chosenStoryline.StorylineContainer;
                node.nodeGroup = chosenTimeline;
                node.SetStoryline(chosenStoryline);
                node.StartTimeline();
                break;
            }
        }
    }
}