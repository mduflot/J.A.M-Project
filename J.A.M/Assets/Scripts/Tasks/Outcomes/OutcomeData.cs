using System;
using UnityEngine;

public class OutcomeData : MonoBehaviour
{
    public enum OutcomeType : uint
    {
        CharacterStat = 1,
        Trait = 2,
        Gauge = 4
    }
    
    public enum OutcomeOperation : uint
    {
        Add = 8,
        Sub = 16
    }
    
    public enum OutcomeTargetStat : uint 
    {
        None = 0,
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
