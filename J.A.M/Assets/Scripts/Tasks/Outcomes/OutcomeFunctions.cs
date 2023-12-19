public class OutcomeFunctions
{
    public const uint ADD_MOOD = (uint)OutcomeData.OutcomeType.CharacterStat + (uint)OutcomeData.OutcomeOperation.Add +
                                 (uint)OutcomeData.OutcomeTargetStat.Mood;

    public const uint SUB_MOOD = (uint)OutcomeData.OutcomeType.CharacterStat + (uint)OutcomeData.OutcomeOperation.Sub +
                                 (uint)OutcomeData.OutcomeTargetStat.Mood;

    public const uint ADD_VOLITION = (uint)OutcomeData.OutcomeType.CharacterStat +
                                     (uint)OutcomeData.OutcomeOperation.Add +
                                     (uint)OutcomeData.OutcomeTargetStat.Volition;

    public const uint SUB_VOLITION = (uint)OutcomeData.OutcomeType.CharacterStat +
                                     (uint)OutcomeData.OutcomeOperation.Sub +
                                     (uint)OutcomeData.OutcomeTargetStat.Volition;

    public const uint ADD_TRAIT = (uint)OutcomeData.OutcomeType.Trait + (uint)OutcomeData.OutcomeOperation.Add;
    public const uint SUB_TRAIT = (uint)OutcomeData.OutcomeType.Trait + (uint)OutcomeData.OutcomeOperation.Sub;
    public const uint ADD_SHIPTRAIT = (uint)OutcomeData.OutcomeType.ShipTrait + (uint)OutcomeData.OutcomeOperation.Add;
    public const uint SUB_SHIPTRAIT = (uint)OutcomeData.OutcomeType.ShipTrait + (uint)OutcomeData.OutcomeOperation.Sub;
    public const uint ADD_GAUGE = (uint)OutcomeData.OutcomeType.Gauge + (uint)OutcomeData.OutcomeOperation.Add;
    public const uint SUB_GAUGE = (uint)OutcomeData.OutcomeType.Gauge + (uint)OutcomeData.OutcomeOperation.Sub;

    public const uint ADD_VOLITION_GAUGE =
        (uint)OutcomeData.OutcomeType.GaugeVolition + (uint)OutcomeData.OutcomeOperation.Add;

    public const uint SUB_VOLITION_GAUGE =
        (uint)OutcomeData.OutcomeType.GaugeVolition + (uint)OutcomeData.OutcomeOperation.Sub;

    public static OutcomeSystem.OutcomeEvent GetOutcomeFunction(uint outcomeFlag)
    {
        var evt = new OutcomeSystem.OutcomeEvent();

        switch (outcomeFlag)
        {
            case ADD_MOOD:
                evt.AddListener(AddMood);
                break;

            case SUB_MOOD:
                evt.AddListener(SubMood);
                break;

            case ADD_VOLITION:
                evt.AddListener(AddVolition);
                break;

            case SUB_VOLITION:
                evt.AddListener(SubVolition);
                break;

            case ADD_TRAIT:
                evt.AddListener(AddTrait);
                break;

            case SUB_TRAIT:
                evt.AddListener(SubTrait);
                break;

            case ADD_SHIPTRAIT:
                evt.AddListener(AddShipTrait);
                break;

            case SUB_SHIPTRAIT:
                evt.AddListener(SubShipTrait);
                break;

            case ADD_GAUGE:
                evt.AddListener(AddGauge);
                break;

            case SUB_GAUGE:
                evt.AddListener(SubGauge);
                break;

            case ADD_VOLITION_GAUGE:
                evt.AddListener(AddVolitionGauge);
                break;

            case SUB_VOLITION_GAUGE:
                evt.AddListener(SubVolitionGauge);
                break;
        }

        return evt;
    }

    private static void AddMood(OutcomeSystem.OutcomeEventArgs e)
    {
        foreach (var character in e.targets)
        {
            character.IncreaseMood(e.value);
        }
    }

    private static void SubMood(OutcomeSystem.OutcomeEventArgs e)
    {
        foreach (var character in e.targets)
        {
            character.IncreaseMood(-e.value);
        }
    }

    private static void AddVolition(OutcomeSystem.OutcomeEventArgs e)
    {
        foreach (var character in e.targets)
        {
            character.IncreaseVolition(e.value);
        }
    }

    private static void SubVolition(OutcomeSystem.OutcomeEventArgs e)
    {
        foreach (var character in e.targets)
        {
            character.IncreaseVolition(-e.value);
        }
    }

    private static void AddTrait(OutcomeSystem.OutcomeEventArgs e)
    {
        foreach (var character in e.targets)
        {
            character.AddTrait(e.outcomeTargetTrait);
        }
    }

    private static void SubTrait(OutcomeSystem.OutcomeEventArgs e)
    {
        foreach (var character in e.targets)
        {
            character.SubTrait(e.outcomeTargetTrait);
        }
    }

    private static void AddShipTrait(OutcomeSystem.OutcomeEventArgs e)
    {
        GameManager.Instance.SpaceshipManager.SpaceshipTraits |= e.outcomeSpaceshipTrait;
        GameManager.Instance.SpaceshipManager.HiddenSpaceshipTraits |= e.outcomeHSpaceshipTrait;
    }

    private static void SubShipTrait(OutcomeSystem.OutcomeEventArgs e)
    {
        GameManager.Instance.SpaceshipManager.SpaceshipTraits &= ~e.outcomeSpaceshipTrait;
        GameManager.Instance.SpaceshipManager.HiddenSpaceshipTraits &= ~e.outcomeHSpaceshipTrait;
    }

    private static void AddGauge(OutcomeSystem.OutcomeEventArgs e)
    {
        GameManager.Instance.SpaceshipManager.GaugeValueOperation(e.gauge, e.value);
    }

    private static void SubGauge(OutcomeSystem.OutcomeEventArgs e)
    {
        GameManager.Instance.SpaceshipManager.GaugeValueOperation(e.gauge, -e.value);
    }

    private static void AddVolitionGauge(OutcomeSystem.OutcomeEventArgs e)
    {
        GameManager.Instance.SpaceshipManager.GaugeValueOperation(e.gauge, e.value);
    }

    private static void SubVolitionGauge(OutcomeSystem.OutcomeEventArgs e)
    {
        GameManager.Instance.SpaceshipManager.GaugeValueOperation(e.gauge, -e.value);
    }
}