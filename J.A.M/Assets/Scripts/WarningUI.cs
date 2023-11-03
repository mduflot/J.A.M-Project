using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarningUI : MonoBehaviour
{
    public CharacterBehaviour character;
    public Image characterIcon;
    public TextMeshProUGUI warningDescription;

    public void CloseWarning()
    {
        gameObject.SetActive(false);
    }

    public void Init(CharacterBehaviour c)
    {
        character = c;
        characterIcon.sprite = c.data.characterIcon;
        if (c.isTaskLeader)
        {
            warningDescription.text = character.data.firstName + " is already assigned to " +
                                      character.currentTask.taskData.taskName +
                                      ". Assigning him here will cancel his current Task. Do you want to proceed?";
        }
        else
        {
            warningDescription.text = character.data.firstName + " is already assigned to " +
                                      character.currentTask.taskData.taskName +
                                      ". Assigning him here will slow down his current Task. Do you want to proceed?";
        }
    }
    public void CancelTask()
    {
        character.isWorking = false;
        if (character.isTaskLeader)
        {
            GameManager.Instance.SpaceshipManager.CancelTask(character.currentTask.taskData);
            
        }
        else
        {
            //calculate new duration
        }
        gameObject.SetActive(false);
    }
}
