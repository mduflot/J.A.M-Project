using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueLine : MonoBehaviour
{
    [SerializeField] private Image characterIcon;
    [SerializeField] private TextMeshProUGUI dialogueText;

    public void DisplayDialogueLine(Sprite icon, string text)
    {
        characterIcon.sprite = icon;
        dialogueText.text = text;
    }
    
}