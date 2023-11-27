using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskNotification : MonoBehaviour
{
    public TaskDataScriptable taskData;
    [SerializeField] private Image completionGauge;
    private float duration;
    private float timeLeft;
    public bool isCompleted;
    private bool taskStarted;
    [SerializeField] private Image taskIcon;
    private List<CharacterBehaviour> leaderCharacters = new();
    private List<CharacterBehaviour> assistantCharacters = new();
    public List<Tuple<Sprite, string, string>> dialogues;

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

        duration = assistantCharacters.Count > 0
            ? t.baseDuration / (Mathf.Pow(assistantCharacters.Count + leaderCharacters.Count, taskData.taskHelpFactor))
            : t.baseDuration; // based on formula time/helpers^0.75
        duration *= TimeTickSystem.ticksPerHour;
        taskStarted = true;
    }

    public void DisplayTaskInfo()
    {
        GameManager.Instance.UIManager.SpawnTaskUI(this);
    }

    public void InitTask(TaskDataScriptable t, List<Tuple<Sprite, string, string>> dialoguesTask = null)
    {
        taskData = t;
        timeLeft = t.timeLeft;
        taskIcon.sprite = t.taskIcon;
        dialogues = dialoguesTask;
    }

    public void UpdateTask()
    {
        if (!taskStarted) return;
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
            outcome.leaderCharacters = leaderCharacters;
            outcome.assistantCharacters = assistantCharacters;
            outcome.Outcome();
        }

        isCompleted = true;
        taskData = null;
        ResetCharacters();
        GameManager.Instance.RefreshCharacterIcons();
        GameManager.Instance.SpaceshipManager.RemoveTask(this);
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