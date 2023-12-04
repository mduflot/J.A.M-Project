using System.Collections.Generic;
using SS;
using SS.Enumerations;
using SS.ScriptableObjects;
using UnityEngine;

public class Checker : MonoBehaviour
{
    [SerializeField] private SSNode node;
    [SerializeField] private SSCampaignSO campaign;
    
    private List<Storyline> activeStorylines = new();
    private float priorityFactor;
    private Storyline chosenStoryline;
    private SSNodeGroupSO chosenTimeline;

    public void Initialize(SSCampaignSO ssCampaign)
    {
        campaign = ssCampaign;
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
                ChooseNewStoryline(campaign.ActivatableStorylines);
                return;
            }

            if (randPicker <= weighedInactivePercent)
            {
                ChooseNewStoryline(campaign.ActivatableStorylines);
            }
            else
            {
                PickTimelineFromStoryline();
            }

            return;
        }

        PickTimelineFromStoryline();
    }

    private void ChooseNewStoryline(List<Storyline> activatableStorylines)
    {
        //numberOfASL = availableStoryLines.length
        //pickPercent = 100.0/numberOfASL
        //randPicker = random(0.0,100.0)
        var numberOfASL = activatableStorylines.Count;
        var pickPercent = 100.0f / numberOfASL;
        var randPicker = Random.Range(0, 100);

        List<Storyline> availableStorylines = new List<Storyline>();

        foreach (var availableStoryline in activatableStorylines)
        {
            // TODO : Add conditions check
            availableStorylines.Add(availableStoryline);
        }

        // TODO : Revoir la gestion des probabilités
        for (int i = 0; i < numberOfASL; i++)
        {
            // FIX : IT'S POSSIBLE TO NEVER PICK A STORYLINE
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

    private void PickTimelineFromStoryline(bool isNewStoryline = false)
    {
        // TODO : Ajouter des probabilités entre les trois à sélectionner
        if (!isNewStoryline)
        {
            chosenStoryline = activeStorylines[Random.Range(0, activeStorylines.Count)];
        }

        var availableTimelines = new List<SSNodeGroupSO>();

        foreach (var nodeGroup in chosenStoryline.ActivatableTimelines)
        {
            // TODO : Add conditions check
            availableTimelines.Add(nodeGroup.Item2);
        }

        var numberOfASL = availableTimelines.Count;
        var pickPercent = 100.0f / numberOfASL;
        var randPicker = Random.Range(0, 100);

        for (int i = 0; i < numberOfASL; i++)
        {
            // FIX : IT'S POSSIBLE TO NEVER PICK A STORYLINE 
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
        PickTimelineFromStoryline(true);
    }

    private void PlayTimeline()
    {
        node.nodeContainer = chosenStoryline.StorylineContainer;
        node.nodeGroup = chosenTimeline;
        node.StartTimeline();
    }
}