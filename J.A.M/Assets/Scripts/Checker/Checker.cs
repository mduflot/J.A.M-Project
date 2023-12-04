using System.Collections.Generic;
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
    private List<Storyline> availableStoryLines = new();

    public void Initialize(SSCampaignSO ssCampaign)
    {
        storylines = ssCampaign.Storylines;
        
        foreach (var storyline in storylines)
        {
            if (activeStorylines.Contains(storyline)) continue;
            if (storyline.StoryStatus != SSStoryStatus.Enabled) continue;
            // TODO : CHECK CONDITIONS
            availableStoryLines.Add(storyline);
        }
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
            foreach (var storyline in activeStorylines)
            {
                if (storyline.EnabledTimelines.Count == 0)
                {
                    storyline.StoryStatus = SSStoryStatus.Completed;
                    activeStorylines.Remove(storyline);
                }
            }
            if (activeStorylines.Count == 0)
            {
                ChooseNewStoryline();
                return;
            }
            chosenStoryline = activeStorylines[Random.Range(0, activeStorylines.Count)];
        }

        var numberOfASL = chosenStoryline.EnabledTimelines.Count;
        var pickPercent = 100.0f / numberOfASL;
        var randPicker = Random.Range(0, 100);

        for (int i = 0; i < numberOfASL; i++)
        {
            if (randPicker <= pickPercent * i)
            {
                chosenTimeline = chosenStoryline.EnabledTimelines[i];
                node.nodeContainer = chosenStoryline.StorylineContainer;
                node.nodeGroup = chosenTimeline;
                node.StartTimeline();
                break;
            }
        }
    }
}