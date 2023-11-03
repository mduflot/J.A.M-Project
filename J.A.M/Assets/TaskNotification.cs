using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskNotification : MonoBehaviour
{
    public TaskDataScriptable taskData;
    [SerializeField] private Image image;
    private float duration;
    private float timeLeft;
    public bool isCompleted = false;
    private bool taskStarted = false;
    private List<CharacterBehaviour> leaderCharacters = new List<CharacterBehaviour>();
    private List<CharacterBehaviour> assistantCharacters = new List<CharacterBehaviour>();
    
    public void StartTask(TaskDataScriptable t, List<CharacterUISlot> characters)
    {
        taskData = t;
        duration = t.baseDuration;
        foreach (var character in characters)
        {
            if (character.isMandatory)
            {
                leaderCharacters.Add(character.icon.character);
                character.icon.character.currentTask = this;
                character.icon.character.isWorking = true;
            }
            else
            {
                if (character.icon != null)
                {
                    assistantCharacters.Add(character.icon.character);
                    character.character.currentTask = this;
                    character.icon.character.isWorking = true;
                }
            }
        }
        taskStarted = true;
    }

    public void DisplayTaskInfo()
    {
        GameManager.Instance.UIManager.SpawnTaskUI(taskData, this);
    }
    
    public void InitTask(TaskDataScriptable t)
    {
        taskData = t;
        timeLeft = t.timeLeft;
    }

    public void UpdateTask()
    {
        if(!taskStarted) return;
        if (duration > 0)
        {
            duration -= TimeTickSystem.timePerTick;
            var completionValue = 1 - duration / taskData.baseDuration;
            image.fillAmount = completionValue;
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
        Destroy(gameObject);
    }

    private void ResetCharacters()
    {
        foreach (var character in leaderCharacters)
        {
            character.isWorking = false;
            character.currentTask = null;
        }
        foreach (var character in assistantCharacters)
        {
            character.isWorking = false;
            character.currentTask = null;
        }
    }
    public void CancelTask()
    {
        taskData = null;
        ResetCharacters();
        Destroy(gameObject);
    }
    
}
