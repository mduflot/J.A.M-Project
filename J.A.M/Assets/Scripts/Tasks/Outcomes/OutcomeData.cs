using System;
using UnityEngine;

public class OutcomeData : MonoBehaviour
{
    [Flags]
    public enum OutcomeType
    {
        CharacterStat = 1,
        Trait = 2,
        Gauge = 4
    }

    [Flags]
    public enum OutcomeOperation
    {
        Add = 8,
        Sub = 16
    }

    [Flags]
    public enum OutcomeTargetStat
    {
        Mood = 32,
        Volition = 64
    }
    
    public enum OutcomeTarget
    {
        Leader,
        Assistant,
        Crew,
        Ship,
        Gauge
    }

}
