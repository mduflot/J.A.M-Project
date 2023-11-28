using System.Collections.Generic;
using SS;
using SS.ScriptableObjects;
using UnityEngine;

/*
 * NEEDED INFORMATION FOR CHECKER :
 *
 * STORYLINES :
 *    - What is the maximum number of concurrent storylines ? (assumed to be 3 for now)
 *    - Where are they stored ?
 *    - How do you check if they're active or not ?
 *    - How do you store their conditions ?
 *    - How do we calculate probability for a Storyline to be picked ? (Currently uses a priority factor shared by all active Storylines)
 *      FOR NOW : PROB FACTOR
 *
 * TIMELINES :
 *    - How are they stored ?
 *    - How do you store their conditions ?
 *      SAME AS STORYLINES
 *    - How are storylines conditions affected by timelines stored and affected ? (see Miro graph Storyline exemple -> "Affect conditions")
 *      SOME TIMELINES ARE LOCKED UNTIL OTHERS HAVE PLAYED (bool hasPlayed on some timelines)
 *    - How is the first task of a timeline stored ?
 *
 * CONDITIONS :
 *    - What are Storylines conditions ? -> Add "Traits" as "StorylinesConditions" ? Use preexisting traits ?
 *      GAUGE CONDITIONS / BOOLEANS ON SHIP TRAITS OR TRAITS (SHIPTRAITS NOT IMPLEMENTED FOR NOW -> TRUE)
 *    - Are the timelines conditions picked from the same set ?
 *    - Should they compare to var data from characters (mood, stress) ? -> Add TraitSystem function to generate flags based on charVarData
 *
 */

public class Checker : MonoBehaviour
{
    private List<Storyline> allStorylines;
    private List<Storyline> activeStorylines;

    private void Initialize(SSCampaignSO campaign)
    {
        allStorylines = new List<Storyline>();
        foreach (var storyline in campaign.Storylines)
        {
            allStorylines.Add(new Storyline(storyline.NodeContainer, storyline.Status, storyline.NodeGroups));
        }
    }

    private void ChooseOutcome()
    {
        if (activeStorylines.Count == 0) ChooseNewStoryline();
    }

    // currently assumes that the max number of storylines is 3
    public void GenerateRandomEvent()
    {
        /*
         * parameters :
         */

        //activeStorylines : List<StoryLines>, list of currently active Storylines (max 3)
        //inactiveStoryLines : List<StoryLines>, list of currently inactive Storylines
        //priorityFactor : float, factor for active StoryLine selection

        /*
         * variables :
         */

        //pickPercent : float, % chance for an outcome
        //weighedActivePercent : float, probability for an active timeline to be selected
        //weighedInactivePercent : float, probability for an inactive timeline to be selected
        //randPicker : float, random value in [0,100] used to chose among available or unavailable storylines

        /*
         * Algorithm :
         */

        //if activeStorylines.length < 1 then
        //   ChoseNewStoryline()
        //else
        //   pickPercent = 1.0/3.0;
        //   weighedInactivePercent = pickPercent * ( 1 - 1.0/priorityFactor)
        //   weighedActivePercent = 1 - weighedInactivePercent 
        //   randPicker = random(0,1)
        //   if randPicker <= weighedInactivePercent then :
        //      ChoseNewStoryLine(inactiveStorylines)
        //   else
        //      if randPicker <= (weighedActivePercent / 2.0) + weighedInactivePercent then : 
        //         PickEventFromActiveStoryline(activeStoryline[0])
        //      else
        //         PickEventFromActiveStoryline(activeStoryline[1])
    }

    public void PickTimelineFromActiveStoryline()
    {
        /*
         * parameters :
         */

        //chosenStoryline : Storyline, Storyline to pick Timelines from
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

    private void ChooseNewStoryline()
    {
        /*
         * parameters :
         */
        //inactiveStorylines : List<StoryLines>, list of all inactive storylines
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