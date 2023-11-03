using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checker
{ 
    public static void GenerateRandomEvent()
    {
        /*
         * var :
         */
        //activeStorylines : List<StoryLines>, list of currently active Storylines (max 3)
        //inactiveStoryLines : List<StoryLines>, list of currently inactive Storylines
        //priorityFactor : float, factor for active StoryLine selection
        //pickPercent : float, % chance for an outcome
        //weighedActivePercent : float, probability for an active timeline to be selected
        //weighedInactivePercent : float, probability for an inactive timeline to be selected
        //randPicker : float, random value used to chose among available or unavailable storylines
        
        /*
         * Algorithm
         */
        
        //if activeStorylines.length < 1 then
        //   ChoseNewStoryline()
        //else
        //   pickPercent = 100.0/3.0;
        //   weighedInactivePercent = pickPercent * ( 1 - 1.0/priorityFactor)
        //   weighedActivePercent = 1 - weighedInactivePercent 
        //   randPicker = random(0,100)
        //   if randPicker <= weighedInactivePercent then :
        //      ChoseNewStoryLine()
        //   else
        //      if randPicker <= (weighedActivePercent / 2.0) + weighedInactivePercent then : 
        //         PickEventFromActiveStoryline(activeStoryline[0])
        //      else
        //         PickEventFromActiveStoryline(activeStoryline[1])
        
    }
 
    public static void ChoseNewStoryline()
    {
        /*
         * var : 
         */
        //choseTimer : float, time elapsed since beginning | last storyline check
        //choseThreshold : float, time that has to elapse between two SL checks
        //StoryLines : List<StoryLines>, list of all storylines
        //availableStorylines : List<Storylines>, stores available storylines
        //numberOfASL : int, number of available storylines
        //pickPercent : float, % chance for a single Storyline to be chosen (supposing equiprobability)
        //randPicker : float, random value used to chose among available storylines
        //i : int, value used in for loop
        
        /*
         * Algorithm
         */
        
        //if choseTimer > choseThreshold then :
        //  foreach(var sl in Storylines) do :
        //      if(sl is available) then :
        //          availableStoryLines.add(sl)
        
        //numberOfASL = availableStoryLines.length
        //pickPercent = 100.0/numberOfASL
        //randPicker = random(0.0,100.0)
        
        //for i = 1, i <= numberOfASL, i++
        //  if randPicker <= pickPercent * i
        //      StartStoryLine(availableStoryLines[i-1]);
    }

    public static void PickEventFromActiveStoryline()
    {
        /*
         * var :
         */
        
        /*
         * Algorithm
         */
    }
    
    public static void CheckOutcome()
    {
        
        /*
         * var :
         */
        /*FLAGS*/
        //outJF : Job, Job flags that affect outcome
        //outPF : PositiveTraits, Positive Trait flags that affect outcome
        //outNF : NegativeTraits, Negative Trait flags that affect outcome
        //CJF : Job, character job flag(s)
        //CPF : PositiveTraits, character Positive trait flag(s)
        //CNF : NegativeTraits, character Negative trait flag(s)
        //matchedJF : Job, result of TraitSystem.MatchJobFlags(CJF, outJF)
        //matchedPF : PositiveTraits, result of TraitSystem.MatchPositiveFlags(CPF, outPF)
        //matchedNF : NegativeTraits, result of TraitSystem.MatchNegativeFlags(CNF, outNF)
        
        //isOutcomePositive : bool, store value for "end condition matched" check
        //finalOutcome : TaskOutcome, final outcome chosen based on parameters & conditions
        //outComeMap : dictionary<string, 
        
       /*
        * Algorithm
        */
        
        //isOutcomePositive = some result based on state of task
        
        //matchedJF = TraitSystem.MatchJobFlags(CJF, outJF)
        //matchedPF = TraitSystem.MatchPositiveFlags(CPF, outPF) 
        //matchedNF = TraitSystem.MatchNegativeFlags(CNF, outNF)
        
 
        
        
        /*
         * Operation on level outcome
         */
        
    }
}
