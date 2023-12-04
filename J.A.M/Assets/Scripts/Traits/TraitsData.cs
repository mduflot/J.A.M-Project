using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraitsData
{
    [Flags]
    public enum Job
    {
        None = 0,
        Soldier = 1,
        Electrician = 2,
        Gardener = 4,
        Mechanic = 8
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
    
    [Flags]
    public enum SpaceshipTraits
    {
        None,
    }
    
    [Flags]
    public enum HiddenSpaceshipTraits
    {
        None,
    }
    
    public enum TraitOperator
    {
        AND,
        OR,
        XOR,
        NAND,
        NOT
    }
    
    [System.Serializable]
    public class Traits
    {
        [SerializeField] private SerializableTuple<Job, PositiveTraits, NegativeTraits> traits;
        public Job GetJob()
        {
            return traits.Job;
        }

        public PositiveTraits GetPositiveTraits()
        {
            return traits.Positive;
        }

        public NegativeTraits GetNegativeTraits()
        {
            return traits.Negative;
        }

        public void AddTraits(Traits traits)
        {
            AddJob(traits.GetJob());
            AddPositiveTrait(traits.GetPositiveTraits());
            AddNegativeTrait(traits.GetNegativeTraits());
        }
        
        public void RemoveTraits(Traits traits)
        {
            Debug.Log("Trait removed");
            RemoveJob(traits.GetJob());
            RemovePositiveTrait(traits.GetPositiveTraits());
            RemoveNegativeTrait(traits.GetNegativeTraits());
        }
        
        private void AddJob(Job j)
        {
            traits.Job |= j;
        }
 
        private void RemoveJob(Job j)
        {
            Debug.Log("Trait removed");
            traits.Job &= ~j;
        }

        private void AddPositiveTrait(PositiveTraits pt)
        {
            traits.Positive |= pt;
        }

        private void RemovePositiveTrait(PositiveTraits pt)
        {
            traits.Positive &= ~pt;
        }

        private void AddNegativeTrait(NegativeTraits nt)
        {
            traits.Negative |= nt;
        }
        
        private void RemoveNegativeTrait(NegativeTraits nt)
        {
            traits.Negative &= ~nt;
        }
    }
}
