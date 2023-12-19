using CharacterSystem;
using UnityEngine.Events;

public class OutcomeSystem
{
    public class OutcomeEvent : UnityEvent<OutcomeEventArgs> { }

    public class OutcomeEventArgs
    {
        public OutcomeData.OutcomeType outcomeType;
        public OutcomeData.OutcomeOperation outcomeOperation;
        public float value;
        public TraitsData.Traits outcomeTargetTrait;
        public TraitsData.SpaceshipTraits outcomeSpaceshipTrait;
        public TraitsData.HiddenSpaceshipTraits outcomeHSpaceshipTrait;

        public CharacterBehaviour[] targets;
        public SystemType gauge;
        public uint outcomeFunctionFlag;
    }

    public static OutcomeEventArgs GenerateEventArgs(Outcome outcome, CharacterBehaviour singleCharacter)
    {
        var newOutcome = FillArgsData(outcome, 1);
        newOutcome.targets[0] = singleCharacter;
        newOutcome.gauge = SystemType.None;
        newOutcome.outcomeFunctionFlag = (uint)outcome.OutcomeType | (uint)outcome.OutcomeOperation |
                                         (uint)outcome.OutcomeTargetStat;
        return newOutcome;
    }

    public static OutcomeEventArgs GenerateEventArgs(Outcome outcome, CharacterBehaviour[] multipleCharacter)
    {
        var newOutcome = FillArgsData(outcome, multipleCharacter.Length);
        for (uint i = 0; i < multipleCharacter.Length; i++)
        {
            newOutcome.targets[i] = multipleCharacter[i];
        }

        newOutcome.gauge = SystemType.None;
        newOutcome.outcomeFunctionFlag = (uint)outcome.OutcomeType | (uint)outcome.OutcomeOperation |
                                         (uint)outcome.OutcomeTargetStat;
        return newOutcome;
    }

    public static OutcomeEventArgs GenerateEventArgs(Outcome outcome, SystemType system)
    {
        var newOutcome = FillArgsData(outcome, 0);
        newOutcome.gauge = system;
        newOutcome.outcomeFunctionFlag = (uint)outcome.OutcomeType | (uint)outcome.OutcomeOperation |
                                         (uint)outcome.OutcomeTargetStat;
        return newOutcome;
    }

    public static OutcomeEventArgs GenerateEventArgs(Outcome outcome, SystemType system, float value)
    {
        var newOutcome = FillArgsData(outcome, 0);
        newOutcome.gauge = system;
        newOutcome.value = value;
        newOutcome.outcomeFunctionFlag = (uint)outcome.OutcomeType | (uint)outcome.OutcomeOperation |
                                         (uint)outcome.OutcomeTargetStat;
        return newOutcome;
    }

    public static OutcomeEventArgs GenerateEventArgs(Outcome outcome)
    {
        var newOutcome = FillArgsData(outcome, 0);
        newOutcome.outcomeFunctionFlag = (uint)outcome.OutcomeType | (uint)outcome.OutcomeOperation |
                                         (uint)outcome.OutcomeTargetStat;
        return newOutcome;
    }

    private static OutcomeEventArgs FillArgsData(Outcome outcome, int numberOfTargets)
    {
        var newOutcome = new OutcomeEventArgs();
        newOutcome.outcomeType = outcome.OutcomeType;
        newOutcome.outcomeOperation = outcome.OutcomeOperation;
        newOutcome.value = outcome.value;
        newOutcome.outcomeTargetTrait = outcome.OutcomeTargetTrait;
        newOutcome.outcomeSpaceshipTrait = outcome.OutcomeShipTrait;
        newOutcome.outcomeHSpaceshipTrait = outcome.OutcomeHiddenShipTrait;
        newOutcome.targets = new CharacterBehaviour[numberOfTargets];
        newOutcome.gauge = outcome.OutcomeTargetGauge;
        return newOutcome;
    }

    public static OutcomeEvent GenerateOutcomeEvent(OutcomeEventArgs evtArgs)
    {
        return OutcomeFunctions.GetOutcomeFunction(evtArgs.outcomeFunctionFlag);
    }
}