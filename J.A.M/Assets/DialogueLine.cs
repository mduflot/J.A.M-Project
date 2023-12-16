using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueLine : MonoBehaviour
{
    [SerializeField] private Image characterIcon;
    [SerializeField] private TextMeshProUGUI dialogueText;

    public void DisplayDialogueLine(string text, Sprite icon)
    {
        dialogueText.text = text;
        characterIcon.sprite = icon;
    }

}
