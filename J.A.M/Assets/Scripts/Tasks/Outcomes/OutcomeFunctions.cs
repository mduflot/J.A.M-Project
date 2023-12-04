using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OutcomeFunctions
{
    public const uint ADD_MOOD = 41;
    public const uint SUB_MOOD = 49;
    public const uint ADD_VOLITION = 73;
    public const uint SUB_VOLITION = 81;
    public const uint ADD_TRAIT = 10;
    public const uint SUB_TRAIT = 18;
    public const uint ADD_GAUGE = 12;
    public const uint SUB_GAUGE = 20;

    public static OutcomeSystem.OutcomeEvent GetOutcomeFunction(uint outcomeFlag)
    {
        var outcomeEvent = new OutcomeSystem.OutcomeEvent();
        switch (outcomeFlag)
        {
            case ADD_MOOD:
                outcomeEvent.AddListener(AddMood);
                break;
            
            case SUB_MOOD:
                outcomeEvent.AddListener(SubMood);
                break;
            
            case ADD_VOLITION:
                outcomeEvent.AddListener(AddVolition);
                break;
            
            case SUB_VOLITION:
                outcomeEvent.AddListener(SubVolition);
                break;
            
            case ADD_TRAIT:
                outcomeEvent.AddListener(AddTrait);
                break;
            
            case SUB_TRAIT:
                outcomeEvent.AddListener(SubTrait);
                break;
            
            case ADD_GAUGE:
                outcomeEvent.AddListener(AddGauge);
                break;
            
            case SUB_GAUGE:
                outcomeEvent.AddListener(SubGauge);
                break;
        }

        return outcomeEvent;
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
            character.IncreaseVolition(e.value);;
        }
    }
    
    private static void SubVolition(OutcomeSystem.OutcomeEventArgs e)
    {
        foreach (var character in e.targets)
        {
            character.IncreaseVolition(-e.value);;
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

    private static void AddGauge(OutcomeSystem.OutcomeEventArgs e)
    {
        Debug.LogWarning("ADD GAUGE NOT IMPLEMENTED");
    }

    private static void SubGauge(OutcomeSystem.OutcomeEventArgs e)
    {
        Debug.LogWarning("SUB GAUGE NOT IMPLEMENTED");
    }
}
