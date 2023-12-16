using System;
using UnityEngine;
using System.Collections.Generic;
public class DialogueLog : MonoBehaviour
{
    [SerializeField] private DialogueLine dialogueLinePrefab;
    [SerializeField] private Transform dialogueLogContent;
    private List<DialogueLine> dialogueLines = new List<DialogueLine>();
    [SerializeField] private Animator animator;
    private bool opened;

    public void DisplayDialogueLog(List<Tuple<Sprite, string, string>> Dialogues)
    {
        foreach (var dialogue in Dialogues)
        {
            var line = Instantiate(dialogueLinePrefab, dialogueLogContent);
            line.DisplayDialogueLine(dialogue.Item3, dialogue.Item1);
        }
    }

    public void ClearDialogueLog()
    {
        foreach (var line in dialogueLines)
        {
            Destroy(line);
        }
        dialogueLines.Clear();
        animator.SetBool("Opened", false);
    }

    public void DisplayMenu()
    {
        opened = !opened;
        animator.SetBool("Opened", opened);
    }
    
}
