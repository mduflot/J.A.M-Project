using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class Task : MonoBehaviour
{
    
    [Header("Display")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI timeLeftText;
    [SerializeField] private TextMeshProUGUI durationText;
    [SerializeField] private Transform leaderSlotsParent;
    [SerializeField] private Transform assistantSlotsParent;
    [SerializeField] private CharacterUISlot[] inactiveSlots;
    
    [Header("Values")]
    [SerializeField] private float timeLeft;
    [SerializeField] private float duration;
    
    private TaskDataScriptable taskData;
    private List<CharacterUISlot> characterSlots = new List<CharacterUISlot>();
    private bool taskStarted;
    
    public void Initialize(TaskDataScriptable data)
    {
        taskData = data;
        titleText.text = taskData.taskName;
        timeLeft = data.timeLeft;
        duration = data.baseDuration;
        Debug.Log("J'initialise");
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
        gameObject.SetActive(true);
    }

    private void Start()
    {
        TimeTickSystem.OnTick += UpdateTask;
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
        }
        else
        {
            Debug.Log(duration);
            if (duration > 0)
            {
                duration -= TimeTickSystem.timePerTick;
            }
            else
            {
                OnTaskComplete();
            }
        }
    }

    public void StartTask()
    {
        if (CanStartTask())
        {
            taskStarted = true;
            CloseTask();
        }
    }
    public void CloseTask()
    {
        foreach (var slot in characterSlots)
        {
            if (slot.icon != null) slot.icon.ResetTransform();
            slot.ClearCharacter();
        }
        characterSlots.Clear();
        gameObject.SetActive(false);
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

    private void OnTaskComplete()
    {
        foreach (var outcome in taskData.outcomes)
        {
            outcome.Outcome();
        }
        taskData = null;
    }
}
