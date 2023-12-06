using TMPro;
using System.Collections.Generic;
using System.Linq;
using SS.Enumerations;
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
    private List<CharacterUISlot> characterSlots = new();
    private bool taskStarted;

    public void Initialize(Notification notification, bool needToDisplay = false)
    {
        this.notification = notification;
        warningUI.gameObject.SetActive(false);
        titleText.text = this.notification.Task.Name;
        timeLeft = this.notification.Task.TimeLeft;
        duration = this.notification.Task.Duration;
        descriptionText.text = this.notification.Task.Description;
        taskStarted = false;
        for (int i = 0; i < this.notification.Task.MandatorySlots; i++)
        {
            var slot = inactiveSlots[i];
            slot.isMandatory = true;
            slot.transform.SetParent(characterSlotsParent);
            slot.gameObject.SetActive(true);
            characterSlots.Add(slot);
        }

        for (int i = 3; i < this.notification.Task.OptionalSlots + 3; i++)
        {
            var slot = inactiveSlots[i];
            slot.isMandatory = false;
            slot.transform.SetParent(characterSlotsParent);
            slot.gameObject.SetActive(true);
            characterSlots.Add(slot);
        }

        if (needToDisplay)
        {
            timeLeftText.SetText(timeLeft.ToString());
            TimeTickSystem.OnTick += UpdateTask;
            gameObject.SetActive(true);
        }
    }

    private void UpdateTask(object sender, TimeTickSystem.OnTickEventArgs e)
    {
        if (!taskStarted)
        {
            for (int index = 0; index < notification.Task.Conditions.Count; index++)
            {
                bool condition = false;
                switch (notification.Task.Conditions[index].Item1.target)
                {
                    case OutcomeData.OutcomeTarget.Leader:
                        for (int j = 0; j < characterSlots.Count; j++)
                        {
                            if (!characterSlots[j].isMandatory) continue;
                            var character = characterSlots[j];
                            condition = ConditionSystem.CheckCharacterCondition(character.icon.character.GetTraits(),
                                notification.Task.Conditions[index].Item1);
                        }

                        break;

                    case OutcomeData.OutcomeTarget.Assistant:
                        for (int j = 0; j < characterSlots.Count; j++)
                        {
                            if (characterSlots[j].isMandatory) continue;
                            var character = characterSlots[j];
                            condition = ConditionSystem.CheckCharacterCondition(character.icon.character.GetTraits(),
                                notification.Task.Conditions[index].Item1);
                        }

                        break;

                    case OutcomeData.OutcomeTarget.Crew:
                        condition = ConditionSystem.CheckCrewCondition(notification.Task.Conditions[index].Item1);
                        break;

                    case OutcomeData.OutcomeTarget.Gauge:
                        condition = ConditionSystem.CheckGaugeCondition(notification.Task.Conditions[index].Item1);
                        break;

                    case OutcomeData.OutcomeTarget.Ship:
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
        if (notification.Task.TaskType.Equals(SSTaskType.Permanent))
            if (!CanStartTask())
                return;
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
        if (notification.Task.TaskType.Equals(SSTaskType.Permanent)) notification.OnCancel();
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