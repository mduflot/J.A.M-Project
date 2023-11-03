using System.Collections;
using System.Collections.Generic;
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

    public void CancelTask()
    {
        character.isWorking = false;
        GameManager.Instance.SpaceshipManager.CancelTask(character.currentTask.taskData);
        gameObject.SetActive(false);
    }
}
