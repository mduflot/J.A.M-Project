using System;
using CharacterSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarningUI : MonoBehaviour
{
    public CharacterBehaviour character;
    public Image characterIcon;
    public TextMeshProUGUI warningDescription;
    public TextMeshProUGUI characterWarningDescription;
    [SerializeField] private GameObject warning;
    [SerializeField] private GameObject characterWarning;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void CloseWarning()
    {
        Appear(false);
    }

    public void CharacterWarning(CharacterBehaviour c)
    {
        character = c;
        characterIcon.sprite = c.GetCharacterData().characterIcon;
        warning.SetActive(false);
        characterWarning.SetActive(true);
        if (c.IsTaskLeader())
        {
            characterWarningDescription.text = character.GetCharacterData().firstName + " is already assigned to " +
                                               character.GetTask().Name +
                                               ". Assigning him here will cancel his current Task. Do you want to proceed?";
        }
        else
        {
            characterWarningDescription.text = character.GetCharacterData().firstName + " is already assigned to " +
                                               character.GetTask().Name +
                                               ". Assigning him here will slow down his current Task. Do you want to proceed?";
        }

        Appear(true);
    }

    public void Warning()
    {
        warning.SetActive(true);
        characterWarning.SetActive(false);
        warningDescription.SetText("Are you sure you want to cancel this task?");
        Appear(true);
    }

    public void CancelTask()
    {
        // TODO: if condition is never true
        if (characterWarning.activeSelf)
        {
            if (character.IsTaskLeader())
            {
                Debug.Log("CancelTask Leader");
                GameManager.Instance.SpaceshipManager.CancelTask(character.GetTask());
            }
            else
            {
                character.StopTask();
            }

            GameManager.Instance.RefreshCharacterIcons();
        }
        else
        {
            GameManager.Instance.UIManager.taskUI.CancelTask();
        }
        Appear(false);
    }

    private void Appear(bool state)
    {
        animator.SetBool("Appear", state);
    }
}