using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TraitSystem
{
    [Flags]
    public enum Job
    {
        None = 0,
        Medic = 1,
        Mechanic = 2,
        Cook = 4,
        Security = 8,
        Pilot = 16,
        Scientist = 32,
    };

    [Flags]
    public enum PositiveTraits
    {
        None = 0,
        Crafty = 1,
        Smart = 2,
        Quick = 4,
        GreenHanded = 8,
    };

    [Flags]
    public enum NegativeTraits
    {
        None = 0,
        Slow = 1,
        Dull = 2,
        Unfocused = 4,
        Depressed = 8,
    }

    [System.Serializable]
    public class Traits
    {
        [SerializeField] private SerializableTuple<TraitSystem.Job, TraitSystem.PositiveTraits, TraitSystem.NegativeTraits> traits;
        public Job GetJob()
        {
            return traits.Item1;
        }

        public PositiveTraits GetPositiveTraits()
        {
            return traits.Item2;
        }

        public NegativeTraits GetNegativeTraits()
        {
            return traits.Item3;
        }

    }
    
    public static Job MatchJobFlags(Job job, Job flagsToMatch)
    {
        return job & flagsToMatch;
    }
    
    public static PositiveTraits MatchPositiveFlags(PositiveTraits flags, PositiveTraits flagsToMatch)
    {
        return flags & flagsToMatch;
    }
    
    public static NegativeTraits MatchNegativeFlags(NegativeTraits flags, NegativeTraits flagsToMatch)
    {
        return flags & flagsToMatch;
    }
    
    // Add task to param to get all flags in one param
    public static void ApplyBonuses(Character character, Job taskJob, PositiveTraits taskPosTraits, NegativeTraits taskNegTraits)
    {
        Job matchedJob = MatchJobFlags(character.GetJob(), taskJob);
        PositiveTraits pTraits = MatchPositiveFlags(character.GetPositiveTraits(), taskPosTraits);
        NegativeTraits nTraits = MatchNegativeFlags(character.GetNegativeTraits(), taskNegTraits);

        Debug.Log("Character Job :");
        ApplyJobBonus(matchedJob);

        //Apply positive bonus for all flags
        Debug.Log("Character Positives : ");
        foreach(PositiveTraits matchedValue in Enum.GetValues(typeof(PositiveTraits)))
        {
            if((matchedValue & pTraits) != 0)
            {
                ApplyPositiveTraitBonus(matchedValue);
            }
        }

        //Apply negative bonus for all flags
        Debug.Log("Character Negatives : ");
        foreach(NegativeTraits matchedValue in Enum.GetValues(typeof(NegativeTraits)))
        {
            if((matchedValue & nTraits) != 0)
            {
                ApplyNegativeTraitBonus(matchedValue);
            }
        }
    }

    // call like : ApplyJobBonus(MatchJobFlags(NPC.Job, Task.Job))
    public static void ApplyJobBonus(Job jobMatch)
    {
        switch(jobMatch)
        {
            case Job.None:
                //No bonus ?
                Debug.Log("no job");
                break;
            case Job.Medic:
                Debug.Log("medic");
                break;
            case Job.Mechanic:
                Debug.Log("mechanic");
                break;
            case Job.Cook:
                Debug.Log("cook");
                break;
            case Job.Security:
                Debug.Log("security");
                break;
            case Job.Pilot:
                Debug.Log("pilot");
                break;
            case Job.Scientist:
                Debug.Log("scientist");
                break;
            default:
                break;
        }
    }
    
    public static void ApplyPositiveTraitBonus(PositiveTraits pt)
    {
        switch(pt)
        {
            case PositiveTraits.None:
                //No bonus ?
                Debug.Log("nothing positive about you");
                break;
            case PositiveTraits.Crafty:
                Debug.Log("crafty guy");
                break;
            case PositiveTraits.Smart:
                //durée * 0.9, efficacité +
                Debug.Log("smart guy");
                break;
            case PositiveTraits.Quick:
                //durée * 0.5 ?
                Debug.Log("quick guy");
                break;
            case PositiveTraits.GreenHanded:
                // + 15 food généré ?
                Debug.Log("greenhanded guy");
                break;
            default:
                break;
        }
    }
    
    public static void ApplyNegativeTraitBonus(NegativeTraits nt)
    {
        switch(nt)
        {
            case NegativeTraits.None:
                //No bonus ?
                Debug.Log("nothing negative about you");
                break;
            case NegativeTraits.Slow:
                //durée tache * 1.5 ?
                Debug.Log("slow guy");
                break;
            case NegativeTraits.Dull:
                //durée +, efficacité - ?
                Debug.Log("dull guy");
                break;
            case NegativeTraits.Unfocused:
                //durée +, efficacité - ?
                Debug.Log("unfocused guy");
                break;
            case NegativeTraits.Depressed:
                //morale - 10 ?
                Debug.Log("depressed guy");
                break;
            default:
                break;
        }
    }
}
