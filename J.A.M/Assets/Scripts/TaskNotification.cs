using System.Collections.Generic;
using UnityEngine;
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
    private List<CharacterBehaviour> leaderCharacters = new List<CharacterBehaviour>();
    private List<CharacterBehaviour> assistantCharacters = new List<CharacterBehaviour>();
    
    public bool TaskStarted
    {
        get => taskStarted;
    }
    
    public void StartTask(TaskDataScriptable t, List<CharacterUISlot> characters)
    {
        taskData = t;
        duration = t.baseDuration;
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
        taskStarted = true;
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
            var completionValue = 1 - duration / taskData.baseDuration;
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
            outcome.Outcome();
        }
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
