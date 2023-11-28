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
        characterIcon.sprite = c.GetCharacterData().characterIcon;
        if (c.IsTaskLeader())
        {
            warningDescription.text = character.GetCharacterData().firstName + " is already assigned to " +
                                      character.GetTask().Name +
                                      ". Assigning him here will cancel his current Task. Do you want to proceed?";
        }
        else
        {
            warningDescription.text = character.GetCharacterData().firstName + " is already assigned to " +
                                      character.GetTask().Name +
                                      ". Assigning him here will slow down his current Task. Do you want to proceed?";
        }
    }
    public void CancelTask()
    {
        if (character.IsTaskLeader())
        {
            GameManager.Instance.SpaceshipManager.CancelTask(character.GetTask());
        }
        else
        {
            character.StopTask();
        }
        GameManager.Instance.RefreshCharacterIcons();
        gameObject.SetActive(false);
    }
}
