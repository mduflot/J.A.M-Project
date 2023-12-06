using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class TraitSystem
{
    [Serializable] public class TraitEvent : UnityEvent<string> {}

    [Serializable]
    public class TraitEventElement <T>
    {
        public T trait;
        public TraitEvent traitEvent;
    }
    
    public static TraitsData.Job MatchJobFlags(TraitsData.Job job, TraitsData.Job flagsToMatch)
    {
        return job & flagsToMatch;
    }
    
    public static TraitsData.PositiveTraits MatchPositiveFlags(TraitsData.PositiveTraits flags, TraitsData.PositiveTraits flagsToMatch)
    {
        return flags & flagsToMatch;
    }
    
    public static TraitsData.NegativeTraits MatchNegativeFlags(TraitsData.NegativeTraits flags, TraitsData.NegativeTraits flagsToMatch)
    {
        return flags & flagsToMatch;
    }

    public static TraitsData.StatTraits GetStatFlags(float mood, float stress)
    {
        TraitsData.StatTraits statTraits = TraitsData.StatTraits.None;
        if (mood >= .75f) statTraits |= TraitsData.StatTraits.GoodMood;
        else if (mood <= .25f) statTraits |= TraitsData.StatTraits.BadMood;
        if (stress >= .75f) statTraits |= TraitsData.StatTraits.Peaceful;
        else if (stress <= .25f) statTraits |= TraitsData.StatTraits.Stressed;

        return statTraits;
    }
    
    // Add task to param to get all flags in one param
    public static void ApplyBonuses(Character character, TraitsData.Job taskJob, TraitsData.PositiveTraits taskPosTraits, TraitsData.NegativeTraits taskNegTraits)
    {
        TraitsData.Job matchedJob = MatchJobFlags(character.GetJob(), taskJob);
        TraitsData.PositiveTraits pTraits = MatchPositiveFlags(character.GetPositiveTraits(), taskPosTraits);
        TraitsData.NegativeTraits nTraits = MatchNegativeFlags(character.GetNegativeTraits(), taskNegTraits);

        Debug.Log("Character Job :");
        ApplyJobBonus(matchedJob);

        //Apply positive bonus for all flags
        Debug.Log("Character Positives : ");
        foreach(TraitsData.PositiveTraits matchedValue in Enum.GetValues(typeof(TraitsData.PositiveTraits)))
        {
            if((matchedValue & pTraits) != 0)
            {
                ApplyPositiveTraitBonus(matchedValue);
            }
        }

        //Apply negative bonus for all flags
        Debug.Log("Character Negatives : ");
        foreach(TraitsData.NegativeTraits matchedValue in Enum.GetValues(typeof(TraitsData.NegativeTraits)))
        {
            if((matchedValue & nTraits) != 0)
            {
                ApplyNegativeTraitBonus(matchedValue);
            }
        }
    }

    
    // call like : ApplyJobBonus(MatchJobFlags(NPC.Job, Task.Job))
    // returns volition bonus
    public static float ApplyJobBonus(TraitsData.Job jobMatch)
    {
        switch(jobMatch)
        {
            /*
            case TraitsData.Job.None:
                //No bonus ?
                Debug.Log("no job");
                return 0.0f;
            case TraitsData.Job.Soldier:
                Debug.Log("soldier");
                return 0.0f;
            case TraitsData.Job.Electrician:
                Debug.Log("electrician");
                return 7.0f;
            case TraitsData.Job.Gardener:
                Debug.Log("gardener");
                return 5.0f;
            case TraitsData.Job.Mechanic:
                Debug.Log("mechanic");
                return 1.0f;
            default:
                return 0.0f;
                */
        }

        return 0.0f;
    }
    
    public static void ApplyPositiveTraitBonus(TraitsData.PositiveTraits pt)
    {
        switch(pt)
        {
            /*
            case TraitsData.PositiveTraits.None:
                //No bonus ?
                Debug.Log("no positives");
                break;
            case TraitsData.PositiveTraits.Crafty:
                Debug.Log("crafty guy");
                break;
            case TraitsData.PositiveTraits.Smart:
                //durée * 0.9, efficacité +
                Debug.Log("smart guy");
                break;
            case TraitsData.PositiveTraits.Quick:
                //durée * 0.5 ?
                Debug.Log("quick guy");
                break;
            case TraitsData.PositiveTraits.GreenHanded:
                // + 15 food généré ?
                Debug.Log("greenhanded guy");
                break;
            default:
                break;
                */
        }
    }
   
    public static void ApplyNegativeTraitBonus(TraitsData.NegativeTraits nt)
    {
        switch(nt)
        {
            /*
            case TraitsData.NegativeTraits.None:
                //No bonus ?
                Debug.Log("no negatives");
                break;
            case TraitsData.NegativeTraits.Slow:
                //durée tache * 1.5 ?
                Debug.Log("slow guy");
                break;
            case TraitsData.NegativeTraits.Dull:
                //durée +, efficacité - ?
                Debug.Log("dull guy");
                break;
            case TraitsData.NegativeTraits.Unfocused:
                //durée +, efficacité - ?
                Debug.Log("unfocused guy");
                break;
            case TraitsData.NegativeTraits.Depressed:
                //morale - 10 ?
                Debug.Log("depressed guy");
                break;
            default:
                break;
                */
        }
    }

   
    public static void ApplyStatBonus(TraitsData.StatTraits st)
    {
        switch (st)
        {
            case TraitsData.StatTraits.None:
                break;
            case TraitsData.StatTraits.BadMood:
                Debug.Log("Character is in a bad mood");
                break;
            case TraitsData.StatTraits.GoodMood:
                Debug.Log("Character is in a good mood");
                break;
            case TraitsData.StatTraits.Stressed:
                Debug.Log("Character is stressed");
                break;
            case TraitsData.StatTraits.Peaceful:
                Debug.Log("Character is peaceful");
                break;
        }
    }
}
