using TMPro;
using UnityEngine;

public class Task : MonoBehaviour
{
    public TaskDataScriptable taskData;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Transform mandatorySlotsParent;
    [SerializeField] private Transform optionalSlotsParent;
    [SerializeField] private CharacterUISlot characterUISlotPrefab;

    public void Initialize(TaskDataScriptable data)
    {
        taskData = data;
        titleText.text = taskData.taskName;
        for (int i = 0; i < taskData.mandatorySlots; i++)
        {
            Instantiate(characterUISlotPrefab, mandatorySlotsParent);
        }

        for (int i = 0; i < taskData.optionalSlots; i++)
        {
            Instantiate(characterUISlotPrefab, optionalSlotsParent);
        }
    }
    
}
