using System;
using UnityEngine;

public class TraitsData
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

    [Flags]
    public enum StatTraits
    {
        None = 0,
        Stressed = 1,
        BadMood = 2,
        Peaceful = 4,
        GoodMood = 8,
    }
    
    [System.Serializable]
    public class Traits
    {
        [SerializeField] private SerializableTuple<Job, PositiveTraits, NegativeTraits> traits;
        
        public Traits(Job job, PositiveTraits positiveTraits, NegativeTraits negativeTraits)
        {
            traits = new SerializableTuple<Job, PositiveTraits, NegativeTraits>(job, positiveTraits, negativeTraits);
        }
        
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
}
