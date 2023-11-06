using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class Task : MonoBehaviour
{
    
    [Header("Display")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI timeLeftText;
    [SerializeField] private TextMeshProUGUI durationText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI previewOutcomeText;
    [SerializeField] private Transform leaderSlotsParent;
    [SerializeField] private Transform assistantSlotsParent;
    [SerializeField] private CharacterUISlot[] inactiveSlots;
    [SerializeField] private TaskNotification taskNotification;
    [SerializeField] private WarningUI warningUI;
    
    [Header("Values")]
    [SerializeField] private float timeLeft;
    [SerializeField] private float duration;
    
    private TaskDataScriptable taskData;
    private List<CharacterUISlot> characterSlots = new List<CharacterUISlot>();
    private bool taskStarted;
    
    /*
     * NOTES :
     *      fix : Notif icon grabs raycast
     *      fix : opening menu with notif icon doesnt show assigned characters
     *      add : refreshDisplay to update values after assigning characters
     *      fix : remove characterIcon from task if menu is closed without starting task 
     */
    public void Initialize(TaskNotification tn)
    {
        warningUI.gameObject.SetActive(false);
        taskData = tn.taskData;
        titleText.text = taskData.taskName;
        timeLeft = taskData.timeLeft;
        duration = taskData.baseDuration;
        descriptionText.text = taskData.descriptionTask;
        taskStarted = false;
        taskNotification = tn;
        for (int i = 0; i < taskData.mandatorySlots; i++)
        {
            var slot = inactiveSlots[i];
            slot.isMandatory = true;
            slot.transform.SetParent(leaderSlotsParent);
            slot.gameObject.SetActive(true);
            characterSlots.Add(slot);
        }

        for (int i = 3; i < taskData.optionalSlots + 3; i++)
        {
            var slot = inactiveSlots[i];
            slot.isMandatory = false;
            slot.transform.SetParent(assistantSlotsParent);
            slot.gameObject.SetActive(true);
            characterSlots.Add(slot);
        }
        timeLeftText.SetText(timeLeft.ToString());
        
        TimeTickSystem.OnTick += UpdateTask;
        gameObject.SetActive(true);
    }
    

    public void UpdateTask(object sender, TimeTickSystem.OnTickEventArgs e)
    {
        if (!taskStarted)
        {
            if (!taskData.isPermanent)
            {
                timeLeft -= TimeTickSystem.timePerTick;
                if (timeLeft <= 0)
                {
                    StartTask();
                }
            }
            
            if (CanStartTask())
            {
                if(characterSlots[0] == null) return;
                previewOutcomeText.text = "+ " + (int)characterSlots[0].icon.character.GetVolition() + " " + taskData.previewOutcome;
                var assistantCharacters = 0;
                foreach (var slot in characterSlots)
                {
                    if (!slot.isMandatory && slot.icon != null) assistantCharacters++;
                }
                Debug.Log(assistantCharacters);
                duration = assistantCharacters > 0 ? taskData.baseDuration/(Mathf.Pow(assistantCharacters + 1, taskData.taskHelpFactor)) : taskData.baseDuration;
                durationText.text = duration.ToString("F2") + " hours";
            }
            else
            {
                previewOutcomeText.text = null;
            }
        }
    }

    public void StartTask()
    {
        if (CanStartTask())
        {
            if (!CharactersWorking())
            {
                taskNotification.StartTask(taskData, characterSlots);
                taskStarted = true;
                CloseTask();
            }
        }
    }
    public void CloseTask()
    {
        foreach (var slot in characterSlots)
        {
            if(slot.icon != null)slot.icon.ResetTransform();
            slot.ClearCharacter();
            slot.gameObject.SetActive(false);
        }
        TimeTickSystem.OnTick -= UpdateTask;
        previewOutcomeText.text = null;
        characterSlots.Clear();
        GameManager.Instance.RefreshCharacterIcons();
        gameObject.SetActive(false);
    }

    public void CloseNotification()
    {
        if (taskData.isPermanent)
        {
            taskNotification.CancelTask();
        }
    }

    private bool CanStartTask()
    {
        foreach (var slot in characterSlots)
        {
            if (slot.isMandatory && slot.icon == null)
            {
                return false;
            }
        }
        return true;
    }

    private bool CharactersWorking()
    {
        foreach (var character in characterSlots)
        {
            if (character.icon != null && character.icon.character.IsWorking())
            {
                warningUI.gameObject.SetActive(true);
                warningUI.Init(character.icon.character);
                return true;
            }
        }

        return false;
    }
}
