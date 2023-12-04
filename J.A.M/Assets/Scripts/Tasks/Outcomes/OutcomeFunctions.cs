using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
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
            
            case ADD_GAUGE:
                evt.AddListener(AddGauge);
                break;
            
            case SUB_GAUGE:
                evt.AddListener(SubGauge);
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
        GameManager.Instance.SpaceshipManager.GaugeValueOperation(e.gauge, e.value);
    }

    private static void SubGauge(OutcomeSystem.OutcomeEventArgs e)
    {
        GameManager.Instance.SpaceshipManager.GaugeValueOperation(e.gauge, -e.value);
    }
}
