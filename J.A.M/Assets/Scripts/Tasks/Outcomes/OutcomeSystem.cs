using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class OutcomeSystem
{
    public class OutcomeEvent : UnityEvent<OutcomeEventArgs> {};
    
    public class OutcomeEventArgs
    {
        public OutcomeData.OutcomeType outcomeType;
        public OutcomeData.OutcomeOperation outcomeOperation;
        public float value;
        public TraitsData.Traits outcomeTargetTrait;
        
        public CharacterBehaviour[] targets;
        public uint outcomeFunctionFlag;
    }
    
    public static OutcomeEventArgs GenerateEventArgs(Outcome outcome, CharacterBehaviour singleCharacter)
    {
        var newOutcome = FillArgsData(outcome);
        newOutcome.targets.Append(singleCharacter);
        newOutcome.outcomeFunctionFlag = (uint) outcome.OutcomeType | (uint) outcome.OutcomeOperation | (uint) outcome.OutcomeTargetStat;
        return newOutcome;
    }
    
    public static OutcomeEventArgs GenerateEventArgs(Outcome outcome, CharacterBehaviour[] multipleCharacter)
    {
        var newOutcome = FillArgsData(outcome);
        newOutcome.targets.AddRange(multipleCharacter);
        newOutcome.outcomeFunctionFlag = (uint) outcome.OutcomeType | (uint) outcome.OutcomeOperation | (uint) outcome.OutcomeTargetStat;
        return newOutcome;
    }
    
    private static OutcomeEventArgs FillArgsData(Outcome outcome)
    {
        var outcomeArgs = new OutcomeEventArgs();
        var newOutcome = new OutcomeEventArgs();
        newOutcome.outcomeType = outcome.OutcomeType;
        newOutcome.outcomeOperation = outcome.OutcomeOperation;
        newOutcome.value = outcome.value;
        newOutcome.outcomeTargetTrait = outcome.OutcomeTargetTrait;

        return outcomeArgs;
    }
}
