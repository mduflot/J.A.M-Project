using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskUI : MonoBehaviour
{
    [Header("Task")] 
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI timeLeftText;
    [SerializeField] private TextMeshProUGUI durationText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI previewOutcomeText;
    [SerializeField] private Transform characterSlotsParent;
    [SerializeField] private CharacterUISlot[] inactiveSlots;
    [SerializeField] private WarningUI warningUI;

    [Header("Dialogues")] 
    [SerializeField] private GameObject dialogueContainer;

    [Header("Values")] 
    [SerializeField] private float timeLeft;
    [SerializeField] private float duration;

    private Notification notification;
    [SerializeField] private List<CharacterUISlot> characterSlots = new();
    private bool taskStarted;

    /*
     * NOTES :
     *      fix : Notif icon grabs raycast
     *      fix : opening menu with notif icon doesnt show assigned characters
     *      add : refreshDisplay to update values after assigning characters
     *      fix : remove characterIcon from task if menu is closed without starting task
     */
    public void Initialize(Notification tn)
    {
        notification = tn;
        warningUI.gameObject.SetActive(false);
        titleText.text = notification.Task.Name;
        timeLeft = notification.Task.TimeLeft;
        duration = notification.Task.Duration;
        descriptionText.text = notification.Task.Description;
        taskStarted = false;
        notification = tn;
        for (int i = 0; i < notification.Task.MandatorySlots; i++)
        {
            var slot = inactiveSlots[i];
            slot.isMandatory = true;
            slot.transform.SetParent(characterSlotsParent);
            slot.gameObject.SetActive(true);
            characterSlots.Add(slot);
        }

        for (int i = 3; i < notification.Task.OptionalSlots + 3; i++)
        {
            var slot = inactiveSlots[i];
            slot.isMandatory = false;
            slot.transform.SetParent(characterSlotsParent);
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
            for (int index = 0; index < notification.Task.Conditions.Count; index++)
            {
                bool condition = false;
                switch (notification.Task.Conditions[index].Item1.target)
                {
                    case OutcomeData.OutcomeTarget.Leader :
                        for (int j = 0; j < characterSlots.Count; j++)
                        {
                            if (!characterSlots[j].isMandatory) continue;
                            var character = characterSlots[j];
                            condition = ConditionSystem.CheckCharacterCondition(character.icon.character.GetTraits(), notification.Task.Conditions[index].Item1);
                        }
                        break;
                    
                    case OutcomeData.OutcomeTarget.Assistant :
                        for (int j = 0; j < characterSlots.Count; j++)
                        {
                            if (characterSlots[j].isMandatory) continue;
                            var character = characterSlots[j];
                            condition = ConditionSystem.CheckCharacterCondition(character.icon.character.GetTraits(), notification.Task.Conditions[index].Item1);
                        }
                        break;
                    
                    case OutcomeData.OutcomeTarget.Crew :
                        condition = ConditionSystem.CheckCrewCondition(notification.Task.Conditions[index].Item1);
                        break;
                    
                    case OutcomeData.OutcomeTarget.Gauge :
                        condition = ConditionSystem.CheckGaugeCondition(notification.Task.Conditions[index].Item1);
                        break;
                    
                    case OutcomeData.OutcomeTarget.Ship :
                        condition = ConditionSystem.CheckSpaceshipCondition(notification.Task.Conditions[index].Item1);
                        break;
                }
                if (condition)
                {
                    previewOutcomeText.text = notification.Task.Conditions[index].Item2;
                    notification.Task.conditionIndex = index;
                    break;
                }
            }
            
            var assistantCharacters = characterSlots.Count(slot => !slot.isMandatory && slot.icon != null);

            duration = assistantCharacters > 0
                ? notification.Task.Duration /
                  (Mathf.Pow(assistantCharacters + 1, notification.Task.HelpFactor))
                : notification.Task.Duration;
            durationText.text = duration.ToString("F2") + " hours";
        }
    }

    public void StartTask()
    {
        if (notification.Task.IsPermanent) if (CanStartTask()) return;
        if (CharactersWorking()) return;
        notification.OnStart(characterSlots);
        taskStarted = true;
        CloseTask();
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

    /// <summary>
    /// Close the task UI
    /// </summary>
    public void CloseTask()
    {
        foreach (var slot in characterSlots)
        {
            if (slot.icon != null) slot.icon.ResetTransform();
            slot.ClearCharacter();
            slot.gameObject.SetActive(false);
        }

        TimeTickSystem.OnTick -= UpdateTask;
        previewOutcomeText.text = null;
        characterSlots.Clear();
        GameManager.Instance.RefreshCharacterIcons();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Close the notification if it is permanent
    /// </summary>
    public void CloseNotification()
    {
        TimeTickSystem.ModifyTimeScale(1.0f);
        if (notification.Task.IsPermanent) notification.OnCancel();
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