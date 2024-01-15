using System;
using UnityEngine;
using System.Collections.Generic;

public class DialogueLog : MonoBehaviour
{
    [SerializeField] private DialogueLine dialogueLinePrefab;
    [SerializeField] private Transform dialogueLogContent;
    [SerializeField] private Animator animator;

    private List<DialogueLine> dialogueLines = new();
    private bool opened;

    public void DisplayDialogueLog(List<SerializableTuple<string, string>> Dialogues)
    {
        Debug.Log("Displaying dialogue log : " + Dialogues.Count + " dialogues");
        for (var indexDialogue = 0; indexDialogue < Dialogues.Count; indexDialogue++)
        {
            var dialogue = Dialogues[indexDialogue];
            var line = Instantiate(dialogueLinePrefab, dialogueLogContent);
            dialogueLines.Add(line);
            if (dialogue.Item1 == "Sensor")
            {
                line.DisplayDialogueLine(null, dialogue.Item2);
            }
            if (dialogue.Item1 == "Expert")
            {
                line.DisplayDialogueLine(null, dialogue.Item2);
            }
            for (int indexCharacter = 0; indexCharacter < GameManager.Instance.SpaceshipManager.characters.Length; indexCharacter++)
            {
                var character = GameManager.Instance.SpaceshipManager.characters[indexCharacter];
                if (character.GetCharacterData().ID == dialogue.Item1)
                {
                    line.DisplayDialogueLine(character.GetCharacterData().characterIcon, dialogue.Item2);
                }
            }
        }
    }

    public void ClearDialogueLog()
    {
        foreach (var line in dialogueLines)
        {
            Debug.Log("Destroying line");
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