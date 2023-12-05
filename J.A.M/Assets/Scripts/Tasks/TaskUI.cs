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
    private List<CharacterUISlot> characterSlots = new();
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
            // TODO : Update PreviewOutcome
            //previewOutcomeText.text = notification.Task.Conditions;

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
        // TODO : Feedback on characters are working
        if (CharactersWorking()) return;
        notification.OnStart(characterSlots);
        taskStarted = true;
        CloseTask();
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