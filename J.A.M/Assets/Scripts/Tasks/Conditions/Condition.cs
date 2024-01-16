using System;

[Serializable]
public class Condition
{
    public OutcomeData.OutcomeTarget target;

    public TraitsData.Traits Traits;
    public TraitsData.SpaceshipTraits SpaceshipTraits;
    public TraitsData.HiddenSpaceshipTraits HiddenSpaceshipTraits;
    public GaugeCheck.GaugeValue GaugeValue;
    public float GaugeValueToCheck;
}