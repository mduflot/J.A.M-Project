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

    public static void ApplyBonuses(CharacterBehaviour character, TaskNotification tn)
    {
        Debug.Log($"Applying bonuses for : {character.name}");
        
        Debug.Log($"Expected Job : {tn.taskTraits.GetJob()}");
        Debug.Log($"Character Job : {character.GetJob()}");
        TraitsData.Job matchedJob = MatchJobFlags(character.GetJob(), tn.taskTraits.GetJob());
        TraitsData.PositiveTraits pTraits = MatchPositiveFlags(character.GetPositiveTraits(), tn.taskTraits.GetPositiveTraits());
        TraitsData.NegativeTraits nTraits = MatchNegativeFlags(character.GetNegativeTraits(), tn.taskTraits.GetNegativeTraits());

        if((matchedJob & tn.taskTraits.GetJob()) != 0)
            ApplyJobBonus(tn, matchedJob);

        Debug.Log($"PT : {pTraits}");
        //Apply positive bonus for all flags
        foreach(TraitsData.PositiveTraits matchedValue in Enum.GetValues(typeof(TraitsData.PositiveTraits)))
        {
            if((matchedValue & pTraits) != 0 && (matchedValue & tn.taskTraits.GetPositiveTraits()) != 0)
            {
                ApplyPositiveTraitBonus(tn, matchedValue);
            }
        }

        Debug.Log($"PT : {nTraits}");
        //Apply negative bonus for all flags
        foreach(TraitsData.NegativeTraits matchedValue in Enum.GetValues(typeof(TraitsData.NegativeTraits)))
        {
            if((matchedValue & nTraits) != 0 && (matchedValue & tn.taskTraits.GetNegativeTraits()) != 0)
            {
                ApplyNegativeTraitBonus(tn, matchedValue);
            }
        }
    }
    
    // call like : ApplyJobBonus(MatchJobFlags(NPC.Job, Task.Job))
    // returns volition bonus
    public static float ApplyJobBonus(TraitsData.Job jobMatch)
    {
        switch(jobMatch)
        {
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
        }
    }
    public static float ApplyJobBonus(TaskNotification tn, TraitsData.Job jobMatch)
    {
        switch(jobMatch)
        {
            case TraitsData.Job.None:
                //No bonus ?
                if(tn.taskJobEvents.ContainsKey(TraitsData.Job.None))
                    tn.taskJobEvents[TraitsData.Job.None].Invoke("None :");
                return 0.0f;
            case TraitsData.Job.Soldier:
                if(tn.taskJobEvents.ContainsKey(TraitsData.Job.Soldier))
                    tn.taskJobEvents[TraitsData.Job.Soldier].Invoke("Soldier :");
                return 0.0f;
            case TraitsData.Job.Electrician:
                if(tn.taskJobEvents.ContainsKey(TraitsData.Job.Electrician))
                    tn.taskJobEvents[TraitsData.Job.Electrician].Invoke("Electrician : ");
                return 7.0f;
            case TraitsData.Job.Gardener:
                if(tn.taskJobEvents.ContainsKey(TraitsData.Job.Gardener))
                    tn.taskJobEvents[TraitsData.Job.Gardener].Invoke("Gardener : ");
                return 5.0f;
            case TraitsData.Job.Mechanic:
                if(tn.taskJobEvents.ContainsKey(TraitsData.Job.Mechanic))
                    tn.taskJobEvents[TraitsData.Job.Mechanic].Invoke("Electrician : ");
                return 1.0f;
            default:
                return 0.0f;
        }
    }
    
    public static void ApplyPositiveTraitBonus(TraitsData.PositiveTraits pt)
    {
        switch(pt)
        {
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
        }
    }
    public static void ApplyPositiveTraitBonus(TaskNotification tn, TraitsData.PositiveTraits ptMatch)
    {
        switch(ptMatch)
        {
            case TraitsData.PositiveTraits.None:
                //No bonus ?
                if(tn.taskPTEvents.ContainsKey(TraitsData.PositiveTraits.None))
                    tn.taskPTEvents[TraitsData.PositiveTraits.None].Invoke("None :");
                break;
            case TraitsData.PositiveTraits.Crafty:
                if(tn.taskPTEvents.ContainsKey(TraitsData.PositiveTraits.Crafty))
                    tn.taskPTEvents[TraitsData.PositiveTraits.Crafty].Invoke("Crafty :");
                break;
            case TraitsData.PositiveTraits.Smart:
                if(tn.taskPTEvents.ContainsKey(TraitsData.PositiveTraits.Smart))
                    tn.taskPTEvents[TraitsData.PositiveTraits.Smart].Invoke("Smart :");
                break;
            case TraitsData.PositiveTraits.Quick:
                if(tn.taskPTEvents.ContainsKey(TraitsData.PositiveTraits.Quick))
                    tn.taskPTEvents[TraitsData.PositiveTraits.Quick].Invoke("Quick :");
                break;
            case TraitsData.PositiveTraits.GreenHanded:
                if(tn.taskPTEvents.ContainsKey(TraitsData.PositiveTraits.GreenHanded))
                    tn.taskPTEvents[TraitsData.PositiveTraits.GreenHanded].Invoke("Greenhanded :");
                break;
            default:
                break;
        }
    }
    
    public static void ApplyNegativeTraitBonus(TraitsData.NegativeTraits nt)
    {
        switch(nt)
        {
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
        }
    }

    public static void ApplyNegativeTraitBonus(TaskNotification tn, TraitsData.NegativeTraits ntMatch)
    {
        switch(ntMatch)
        {
            case TraitsData.NegativeTraits.None:
                if(tn.taskNTEvents.ContainsKey(TraitsData.NegativeTraits.None))
                    tn.taskNTEvents[TraitsData.NegativeTraits.None].Invoke("None :");
                break;
            case TraitsData.NegativeTraits.Slow:
                if(tn.taskNTEvents.ContainsKey(TraitsData.NegativeTraits.Slow))
                    tn.taskNTEvents[TraitsData.NegativeTraits.Slow].Invoke("Slow :");
                break;
            case TraitsData.NegativeTraits.Dull:
                if(tn.taskNTEvents.ContainsKey(TraitsData.NegativeTraits.Dull))
                    tn.taskNTEvents[TraitsData.NegativeTraits.Dull].Invoke("Dull :");
                break;
            case TraitsData.NegativeTraits.Unfocused:
                if(tn.taskNTEvents.ContainsKey(TraitsData.NegativeTraits.Unfocused))
                    tn.taskNTEvents[TraitsData.NegativeTraits.Unfocused].Invoke("Unfocused :");
                break;
            case TraitsData.NegativeTraits.Depressed:
                if(tn.taskNTEvents.ContainsKey(TraitsData.NegativeTraits.Depressed))
                    tn.taskNTEvents[TraitsData.NegativeTraits.Depressed].Invoke("Depressed :");
                break;
            default:
                break;
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
