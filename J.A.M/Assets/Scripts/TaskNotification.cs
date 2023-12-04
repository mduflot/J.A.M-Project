using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class TaskNotification : MonoBehaviour
{
    public TaskDataScriptable taskData;
    [SerializeField] private Image completionGauge;
    private float duration;
    private float timeLeft;
    public bool isCompleted = false;
    private bool taskStarted = false;
    [SerializeField] private Image taskIcon;
    private List<CharacterBehaviour> leaderCharacters = new();
    private List<CharacterBehaviour> assistantCharacters = new();

    private ConditionSO taskCondition;
    private OutcomeSystem.OutcomeEvent[] outcomeEvents;
    private OutcomeSystem.OutcomeEventArgs[] outcomeEventArgs;
    
    public bool TaskStarted
    {
        get => taskStarted;
    }

    public List<CharacterBehaviour> LeaderCharacters
    {
        get => leaderCharacters;
    }

    public List<CharacterBehaviour> AssistantCharacters
    {
        get => assistantCharacters;
    }
    
    public void StartTask(TaskDataScriptable t, List<CharacterUISlot> characters)
    {
        taskData = t;
        foreach (var character in characters)
        {
            if (character.isMandatory)
            {
                leaderCharacters.Add(character.icon.character);
                character.icon.character.AssignTask(this, true);
            }
            else
            {
                if (character.icon != null)
                {
                    assistantCharacters.Add(character.icon.character);
                    character.icon.character.AssignTask(this);
                }
            }
        }


        //checkCondition & reference
        var validatedCondition = false;
        
        for (uint i = 0; i < t.conditions.Length; i++)
        {
            taskCondition = t.conditions[i];
            validatedCondition = RouteCondition(taskCondition.target);
            if (validatedCondition)
                break;
        }
        
        if (validatedCondition)
        {
            //Generate event args
            outcomeEventArgs = new OutcomeSystem.OutcomeEventArgs[taskCondition.outcomes.Outcomes.Length];
            for (var i = 0; i < taskCondition.outcomes.Outcomes.Length; i++)
            {
                var outcome = taskCondition.outcomes.Outcomes[i];
                switch (outcome.OutcomeTarget)
                {
                    case OutcomeData.OutcomeTarget.Leader:
                        outcomeEventArgs[i] = OutcomeSystem.GenerateEventArgs(outcome, leaderCharacters[0]);
                        break;

                    case OutcomeData.OutcomeTarget.Assistant:
                        outcomeEventArgs[i] = OutcomeSystem.GenerateEventArgs(outcome, assistantCharacters[0]);
                        break;

                    case OutcomeData.OutcomeTarget.Crew:
                        outcomeEventArgs[i] = OutcomeSystem.GenerateEventArgs(outcome, GameManager.Instance.SpaceshipManager.characters);
                        break;
                    
                    case OutcomeData.OutcomeTarget.Gauge:
                        outcomeEventArgs[i] = OutcomeSystem.GenerateEventArgs(outcome, outcome.OutcomeTargetGauge);
                        break;
                }
            }

            //Generate events
            outcomeEvents = new OutcomeSystem.OutcomeEvent[taskCondition.outcomes.Outcomes.Length];
            for (var i = 0; i < outcomeEventArgs.Length; i++)
            {
                outcomeEvents[i] = OutcomeSystem.GenerateOutcomeEvent(outcomeEventArgs[i]);
            }
        }
        else
        {
            
            outcomeEventArgs = Array.Empty<OutcomeSystem.OutcomeEventArgs>();
            outcomeEvents = Array.Empty<OutcomeSystem.OutcomeEvent>();
        }
        
        duration = assistantCharacters.Count > 0 ? t.baseDuration/(Mathf.Pow(assistantCharacters.Count + leaderCharacters.Count, taskData.taskHelpFactor)) : t.baseDuration; // based on formula time/helpers^0.75
        duration *= TimeTickSystem.ticksPerHour;
        taskStarted = true;
    }

    private bool RouteCondition(OutcomeData.OutcomeTarget target)
    {
        bool validateCondition = false;
        switch (target)
        {
            case OutcomeData.OutcomeTarget.Leader:
                validateCondition = ConditionSystem.CheckCondition(leaderCharacters[0].GetTraits(), TraitsData.SpaceshipTraits.None, TraitsData.HiddenSpaceshipTraits.None, taskCondition);
                break;
            
            case OutcomeData.OutcomeTarget.Assistant:
                if(assistantCharacters.Count >= 1)
                    validateCondition = ConditionSystem.CheckCondition(assistantCharacters[0].GetTraits(), TraitsData.SpaceshipTraits.None, TraitsData.HiddenSpaceshipTraits.None, taskCondition);
                break;
        }

        return validateCondition;
    }
    
    public void DisplayTaskInfo()
    {
        GameManager.Instance.UIManager.SpawnTaskUI(this);
    }
    
    public void InitTask(TaskDataScriptable t)
    {
        taskData = t;
        timeLeft = t.timeLeft;
        taskIcon.sprite = t.taskIcon;
    }

    public void UpdateTask()
    {
        if(!taskStarted) return;
        if (duration > 0)
        {
            duration -= TimeTickSystem.timePerTick;
            var completionValue = 1 - duration / (taskData.baseDuration * TimeTickSystem.ticksPerHour);
            completionGauge.fillAmount = completionValue;
        }
        else
        {
            OnTaskComplete();
        }
    }
    
    private void OnTaskComplete()
    {
        foreach (var outcome in taskData.outcomes)
        {
            outcome.Outcome(this);
        }
        
        for(uint i = 0; i < outcomeEvents.Length; i++)
            outcomeEvents[i].Invoke(outcomeEventArgs[i]);
        
        isCompleted = true;
        taskData = null;
        ResetCharacters();
        GameManager.Instance.RefreshCharacterIcons();
        Destroy(gameObject);
    }

    private void ResetCharacters()
    {
        foreach (var character in leaderCharacters)
        {
            character.StopTask();
        }
        foreach (var character in assistantCharacters)
        {
            character.StopTask();
        }
    }
    public void CancelTask()
    {
        taskData = null;
        ResetCharacters();
        Destroy(gameObject);
    }
    
}
