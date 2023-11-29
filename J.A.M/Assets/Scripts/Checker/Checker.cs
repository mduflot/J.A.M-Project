using System.Collections.Generic;
using SS;
using SS.ScriptableObjects;
using UnityEngine;

public class Checker : MonoBehaviour
{
    //activeStorylines : List<StoryLines>, list of currently active Storylines (max 3)
    //inactiveStoryLines : List<StoryLines>, list of currently inactive Storylines
    //priorityFactor : float, factor for active StoryLine selection
    private List<Storyline> allStorylines;
    private List<Storyline> activeStorylines;
    private List<Storyline> inactiveStorylines;
    private float priorityFactor;
    
    private Storyline chosenStoryline;
    
    //pickPercent : float, % chance for an outcome
    //weighedActivePercent : float, probability for an active timeline to be selected
    //weighedInactivePercent : float, probability for an inactive timeline to be selected
    //randPicker : float, random value in [0,100] used to chose among available or unavailable storylines
    float pickPercent, weighedActivePercent, weighedInactivePercent, randPicker;

    private void Initialize(SSCampaignSO campaign)
    {
        allStorylines = new List<Storyline>();
        foreach (var storyline in campaign.Storylines)
        {
            
        }
    }

    public void GenerateRandomEvent()
    {
        if (activeStorylines.Count < 3)
        {
            pickPercent = 1.0f/3.0f;
            weighedInactivePercent = pickPercent * (1 - 1.0f / priorityFactor);
            weighedActivePercent = 1 - weighedInactivePercent;
            randPicker = Random.Range(0, 1);
            if (randPicker <= weighedInactivePercent)
            {
                ChooseNewStoryline(inactiveStorylines);
            }
            else
            {
                if (randPicker <= (weighedActivePercent / 2.0) + weighedInactivePercent)
                {
                    PickTimelineFromStoryline();
                }
                else
                {
                    PickTimelineFromStoryline();
                }
            }
        }
        else
        {
            PickTimelineFromStoryline();
        }
    }

    public void PickTimelineFromStoryline()
    {
        /*
         * parameters :
         */
        //conditions : Traits, Traits Tuple giving current state of ship / available characters / etc..

        /*
         * variables :
         */
        //availableTimelines : List<Timeline>, list of all available timelines in the current Storyline based on conditions
        //numberOfTL : int, number of available Timelines
        //pickPercent : float, % chance of a single timeline being chosen (based on equiprobability)
        //randPicker : float, random value in [0,100] used to chose among available Timelines 
        //i : int, index used in for loop


        /*
         * Algorithm :
         */

        //for each Timeline in chosenStoryline do :
        //   if conditions match Timeline then :
        //      availableTimelines.add(Timeline)
        //
        //numberOfASL = availableTimelines.length
        //pickPercent = 100.0/numberOfAsl
        //randPicker = random(0,100)
        //
        //for i = 1, i <= numberOfASL, i++ do :
        //   if randPicker <= pickPercent * i then :
        //      PlayTimeline(availableTimelines[i-1])
        //      break;
    }

    private void ChooseNewStoryline(List<Storyline> inactiveStorylines = null)
    {
        /*
         * parameters :
         */
        //conditions : Traits, current set of conditions to match to Storylines conditions

        /*
         * variables :
         */
        //availableStorylines : List<Storylines>, stores available storylines based on their conditions
        //numberOfASL : int, number of available storylines
        //pickPercent : float, % chance for a single Storyline to be chosen (supposing equiprobability)
        //randPicker : float, random value in [0,100] used to chose among available storylines
        //i : int, index used in for loop

        /*
         * Algorithm :
         */

        //foreach(var sl in Storylines) do :
        //   if conditions match sl then :
        //       availableStoryLines.add(sl)

        //numberOfASL = availableStoryLines.length
        //pickPercent = 100.0/numberOfASL
        //randPicker = random(0.0,100.0)

        //for i = 1, i <= numberOfASL, i++
        //  if randPicker <= pickPercent * i
        //      StartStoryLine(availableStoryLines[i-1]);
        //      break
    }

    public void StartStoryline()
    {
        /*
         * parameters :
         */

        //chosenStoryline : StoryLine, Storyline to start

        /*
         * Algorithm :
         */

        //choseStoryline.setActive(true)
        //PickTimelineFromActiveStoryline(chosenStoryline)
    }

    public void PlayTimeline()
    {
        /*
         * parameters :
         */

        //chosenTimeline : TimeLine, Timeline to play

        /*
         * Algorithm :
         */

        //chosenTimeline.setActive(true)
        //choseTimeline.StartFirstTask() // Supposes that task start is handled outside of checker script 
    }
}