using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskNotification : MonoBehaviour
{
    public TaskDataScriptable taskData;
    [SerializeField] private Image image;
    private float duration;
    private List<CharacterDataScriptable> leaderCharacters = new List<CharacterDataScriptable>();
    private List<CharacterDataScriptable> assistantCharacters = new List<CharacterDataScriptable>();
    
    public void StartTask(TaskDataScriptable t, List<CharacterUISlot> characters)
    {
        taskData = t;
        duration = t.baseDuration;
        foreach (var character in characters)
        {
            if (character.isMandatory)
            {
                leaderCharacters.Add(character.icon.character);
            }
            else
            {
                if (character.icon != null)
                {
                    assistantCharacters.Add(character.icon.character);
                }
            }
        }
        TimeTickSystem.OnTick += UpdateTask;
    }

    public void InitTask(TaskDataScriptable t)
    {
        
    }

    private void UpdateTask(object sender, TimeTickSystem.OnTickEventArgs e)
    {
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
        taskData = null;
        TimeTickSystem.OnTick -= UpdateTask;
    }
    
}
