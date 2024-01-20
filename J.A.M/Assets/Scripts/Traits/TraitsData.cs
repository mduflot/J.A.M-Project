using System;
using UnityEngine;

public class TraitsData
{
    [Flags]
    public enum Job
    {
        None = 0,
        Military = 1,
        Electrician = 2,
        Cook = 4,
        FieldEngineer = 8,
        Pilot = 16,
        Scientist = 32,
        Mechanic = 64,
        Civilian = 128,
        Elrenda = 256,
        Leisin = 512,
        Malda = 1024,
        Varus = 2048,
        Lammy = 4096,
        Seltis = 8192
    };

    [Flags]
    public enum PositiveTraits
    {
        None = 0,
        Authority = 1,
        Calm = 2,
        Cautious = 4,
        Curious = 8,
        Extroverted = 16,
        CalmUnderPressure = 32,
        ArtifactKnowledge = 64
    };

    [Flags]
    public enum NegativeTraits
    {
        None = 0,
        HotHeaded = 1,
        Old = 2,
        ShellShocked = 4,
        HighOnFumes = 8,
        Hallucinating = 16,
        Distrustful = 32,
        Crippled = 34,
        Parasited = 128,
        LockedUp = 256,
        Scarred = 512,
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
        None = 0,
        ShipLostCargo = 1,
        NewMouthToFeed = 2,
        SparedRepairParts = 4,
        RuinedLab = 8,
        SpeedMode = 16,
        FightMode = 32,
        StealthMode = 64,
        DamagedMedicalRoom = 128,
        Restriction = 256,
        ObstructedVentilation = 512,
        Rot = 1024,
        Malfunction = 2048,
        BadInsulation = 4096,
        Leak = 8192,
        WeakenedHull = 16384,
        DamagedRations = 32768,
        DamagedElectricalRoom = 65536,
        DamagedDockingBay = 131072,
        DamagedCargoBays = 262144,
        DamagedCamp = 524288,
        DamagedBridge = 1048576,
        DamagedAI = 2097152,
        DamagedCommonRoom = 4194304,
        DamagedBedrooms = 8388608,
        DamagedArtifactRoom = 16777216,
        DamagedCommodities = 33554432,
        Fertilization = 67108864,
    }
    
    [Flags]
    public enum HiddenSpaceshipTraits
    {
        None = 0,
        WreckedPirateShip = 1,
        ArtifactInstalled = 2,
        FreeWill = 4,
        Act1Over = 8,
        Act2Over = 16,
        Act3Over = 32,
    }
    
    public enum TraitOperator
    {
        AND,
        OR,
        XOR,
        NAND,
        NOT
    }
    
    [Serializable]
    public class Traits
    {
        [SerializeField] public SerializableTuple<Job, PositiveTraits, NegativeTraits> traits;
        
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

        public void AddTraits(Traits traits)
        {
            AddJob(traits.GetJob());
            AddPositiveTrait(traits.GetPositiveTraits());
            AddNegativeTrait(traits.GetNegativeTraits());
        }
        
        public void RemoveTraits(Traits traits)
        {
            RemoveJob(traits.GetJob());
            RemovePositiveTrait(traits.GetPositiveTraits());
            RemoveNegativeTrait(traits.GetNegativeTraits());
        }
        
        private void AddJob(Job j)
        {
            traits.Item1 |= j;
        }
 
        private void RemoveJob(Job j)
        {
            traits.Item1 &= ~j;
        }

        private void AddPositiveTrait(PositiveTraits pt)
        {
            traits.Item2 |= pt;
        }

        private void RemovePositiveTrait(PositiveTraits pt)
        {
            traits.Item2 &= ~pt;
        }

        private void AddNegativeTrait(NegativeTraits nt)
        {
            traits.Item3 |= nt;
        }
        
        private void RemoveNegativeTrait(NegativeTraits nt)
        {
            traits.Item3 &= ~nt;
        }
    }
}
