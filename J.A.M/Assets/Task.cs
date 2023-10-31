using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class Task : MonoBehaviour
{
    public TaskDataScriptable taskData;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Transform mandatorySlotsParent;
    [SerializeField] private Transform optionalSlotsParent;
    [SerializeField] private CharacterUISlot characterUISlotPrefab;
    private List<CharacterUISlot> characterSlots = new List<CharacterUISlot>();
    public void Initialize(TaskDataScriptable data)
    {
        taskData = data;
        titleText.text = taskData.taskName;
        for (int i = 0; i < taskData.mandatorySlots; i++)
        {
            var slot = Instantiate(characterUISlotPrefab, mandatorySlotsParent);
            slot.isMandatory = true;
            characterSlots.Add(slot);
        }

        for (int i = 0; i < taskData.optionalSlots; i++)
        {
            var slot = Instantiate(characterUISlotPrefab, optionalSlotsParent);
            characterSlots.Add(slot);
        }
        gameObject.SetActive(true);
        Debug.Log(characterSlots.Count);
    }

    public void StartTask()
    {
        if (CanStartTask())
        {
            CloseTask();
        }
    }
    public void CloseTask()
    {
        gameObject.SetActive(false);
        characterSlots.Clear();
    }

    private bool CanStartTask()
    {
        foreach (var slot in characterSlots)
        {
            if (slot.isMandatory && slot.character == null)
            {
                Debug.Log("false");
                return false;
            }
        }

        return true;
    }
}
