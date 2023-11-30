using System.Collections.Generic;
using SS;
using SS.Enumerations;
using SS.ScriptableObjects;
using UnityEngine;

public class Checker : MonoBehaviour
{
    private SSCampaignSO campaign;
    private List<Storyline> activeStorylines;
    private float priorityFactor;

    private Storyline chosenStoryline;
    private SSNodeGroupSO chosenTimeline;

    // TODO : Revoir les probabilités de l'ensemble du script
    // TODO : Revoir la gestion du choix de la storyline
    
    public void Initialize(SSCampaignSO ssCampaign)
    {
        campaign = ssCampaign;
    }

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
            var weighedActivePercent = 1 - weighedInactivePercent;
            var randPicker = Random.Range(0, 1);
            if (activeStorylines.Count == 0)
            {
                ChooseNewStoryline(campaign.ActivatableStorylines);
                PickTimelineFromStoryline();
                return;
            }

            if (randPicker <= weighedInactivePercent)
            {
                ChooseNewStoryline(campaign.ActivatableStorylines);
                PickTimelineFromStoryline();
            }
            else
            {
                // TODO : ???
                if (randPicker <= (weighedActivePercent / 2.0) + weighedInactivePercent)
                {
                    PickTimelineFromStoryline(activeStorylines);
                }
                else
                {
                    PickTimelineFromStoryline(activeStorylines);
                }
            }
        }
        else
        {
            PickTimelineFromStoryline(activeStorylines);
        }
    }

    private void ChooseNewStoryline(List<Storyline> activatableStorylines = null)
    {
        /*
         * parameters :
         */
        //conditions : Traits, current set of conditions to match to Storylines conditions
        
        List<Storyline> availableStorylines = new List<Storyline>();

        //numberOfASL = availableStoryLines.length
        //pickPercent = 100.0/numberOfASL
        //randPicker = random(0.0,100.0)
        var numberOfASL = availableStorylines.Count;
        var pickPercent = 100.0f / numberOfASL;
        var randPicker = Random.Range(0, 100);

        foreach (var availableStoryline in activatableStorylines)
        {
            // TODO : Add conditions check
        }

        for (int i = 0; i < numberOfASL; i++)
        {
            if (randPicker <= pickPercent * i)
            {
                chosenStoryline = availableStorylines[i];
                chosenStoryline.StorylineContainer.StoryStatus = SSStoryStatus.Activated;
                activeStorylines.Add(chosenStoryline);
                StartStoryline();
                break;
            }
        }
    }

    private void PickTimelineFromStoryline(List<Storyline> activeStoryline = null)
    {
        /*
         * parameters :
         */
        //conditions : Traits, Traits Tuple giving current state of ship / available characters / etc..

        if (activeStorylines != null)
        {
            chosenStoryline = activeStorylines[Random.Range(0, activeStorylines.Count)];
        }

        var availableTimelines = new List<SSNodeGroupSO>();

        foreach (var nodeGroup in chosenStoryline.ActivatableTimelines)
        {
            // TODO : Add conditions check
        }

        var numberOfASL = availableTimelines.Count;
        var pickPercent = 100.0f / numberOfASL;
        var randPicker = Random.Range(0, 100);

        for (int i = 0; i < numberOfASL; i++)
        {
            if (randPicker <= pickPercent * i)
            {
                chosenTimeline = availableTimelines[i];
                PlayTimeline();
                break;
            }
        }
    }

    private void StartStoryline()
    {
        PickTimelineFromStoryline();
    }

    private void PlayTimeline()
    {
        // chosenTimeline // Supposes that task start is handled outside of checker script 
    }
}