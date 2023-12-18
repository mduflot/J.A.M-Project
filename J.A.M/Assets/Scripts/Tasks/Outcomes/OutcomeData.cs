using System;
using UnityEngine;

public class OutcomeData : MonoBehaviour
{
    public enum OutcomeType : uint
    {
        CharacterStat = 1,
        Trait = 2,
        ShipTrait = 4,
        Gauge = 8
    }
    
    public enum OutcomeOperation : uint
    {
        Add = 16,
        Sub = 32
    }
    
    public enum OutcomeTargetStat : uint 
    {
        None = 0,
        Mood = 64,
        Volition = 128
    }
    
    public enum OutcomeTarget
    {
        None,
        Leader,
        Assistant,
        Crew,
        Ship,
        Gauge
    }

}
