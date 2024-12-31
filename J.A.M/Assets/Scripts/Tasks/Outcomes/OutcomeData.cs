using UnityEngine;

public class OutcomeData : MonoBehaviour
{
    public enum OutcomeType : uint
    {
        CharacterStat = 1,
        Trait = 2,
        ShipTrait = 4,
        Gauge = 8,
        GaugeVolition = 16
    }
    
    public enum OutcomeOperation : uint
    {
        Add = 32,
        Sub = 64
    }
    
    public enum OutcomeTargetStat : uint 
    {
        None = 0,
        Mood = 128,
        Volition = 256
    }
    
    public enum OutcomeTarget
    {
        None,
        Leader,
        Assistant,
        Crew,
        Ship,
        Gauge,
        GaugeValue,
        Random
    }

}
