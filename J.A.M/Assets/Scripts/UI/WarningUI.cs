using System.Collections.Generic;
using CharacterSystem;
using TMPro;
using UnityEngine;

public class WarningUI : MonoBehaviour
{
    public List<CharacterBehaviour> characters;
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

    public void CharacterWarning(List<CharacterBehaviour> c)
    {
        characters = c;
        warning.SetActive(false);
        characterWarning.SetActive(true);
        characterWarningDescription.text = "";
        for (int i = 0; i < c.Count; i++)
        {
            var character = c[i];
            if (character.IsTaskLeader())
            {
                characterWarningDescription.text = character.GetCharacterData().firstName + " is already assigned to " +
                                                   character.GetTask().Name +
                                                   ". Assigning him here will cancel his current Task. Do you want to proceed?";
            }
            else
            {
                characterWarningDescription.text += character.GetCharacterData().firstName + " is already assigned to " +
                                                   character.GetTask().Name +
                                                   ". Assigning him here will slow down his current Task. Do you want to proceed?";
            }
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
        if (characterWarning.activeSelf)
        {
            for (int i = 0; i < characters.Count; i++)
            {
                var character = characters[i];
                if (character.IsTaskLeader())
                {
                    character.IncreaseMood(-10);
                    GameManager.Instance.SpaceshipManager.CancelTask(character.GetTask());
                }
                else
                {
                    character.IncreaseMood(-10);
                    character.StopTask();
                }
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