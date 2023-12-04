using System.Collections.Generic;
using System.Linq;
using SS;
using SS.Enumerations;
using SS.ScriptableObjects;
using UnityEngine;

public class Checker : MonoBehaviour
{
    [SerializeField] private SSNode node;
    [SerializeField] private List<Storyline> storylines;

    private List<Storyline> activeStorylines = new();
    private float priorityFactor;
    private Storyline chosenStoryline;
    private SSNodeGroupSO chosenTimeline;

    public void Initialize(SSCampaignSO ssCampaign)
    {
        storylines = ssCampaign.Storylines;
    }

    [ContextMenu("GenerateRandomEvent")]
    public void GenerateRandomEvent()
    {
        if (activeStorylines.Count < 3)
        {
            //pickPercent : float, % chance for an outcome
            //weighedActivePercent : float, probability for an active timeline to be selected
            //weighedInactivePercent : float, probability for an inactive timeline to be selected
            //randPicker : float, random value in [0,100] used to chose among available or unavailable storylines
            var pickPercent = 1.0f / 3.0f;
            var weighedInactivePercent = pickPercent * (1 - 1.0f / priorityFactor);
            // var weighedActivePercent = 1 - weighedInactivePercent;
            var randPicker = Random.Range(0, 1);
            if (activeStorylines.Count == 0)
            {
                ChooseNewStoryline();
                return;
            }

            if (randPicker <= weighedInactivePercent)
            {
                ChooseNewStoryline();
            }
            else
            {
                PickTimelineFromStoryline();
            }

            return;
        }

        PickTimelineFromStoryline();
    }

    private void ChooseNewStoryline()
    {
        List<Storyline> availableStoryLines = new List<Storyline>();

        foreach (var storyline in storylines)
        {
            if (activeStorylines.Contains(storyline)) continue;
            if (storyline.StoryStatus != SSStoryStatus.Enabled) continue;
            // TODO : CHECK CONDITIONS
            availableStoryLines.Add(storyline);
        }

        //numberOfASL = availableStoryLines.length
        //pickPercent = 100.0/numberOfASL
        //randPicker = random(0.0,100.0)
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
            chosenStoryline = activeStorylines[Random.Range(0, activeStorylines.Count)];
        }

        List<SSNodeGroupSO> availableTimelines = new List<SSNodeGroupSO>();

        bool isAllDisabled = true;
        foreach (var timeline in chosenStoryline.Timelines.Where(timeline =>
                     timeline.StoryStatus == SSStoryStatus.Enabled))
        {
            isAllDisabled = false;
        }

        if (isAllDisabled)
        {
            chosenStoryline.StoryStatus = SSStoryStatus.Disabled;
            activeStorylines.Remove(chosenStoryline);
            ChooseNewStoryline();
            return;
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
                node.StartTimeline();
                break;
            }
        }
    }
}