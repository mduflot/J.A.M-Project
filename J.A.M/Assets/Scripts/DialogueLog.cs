using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class DialogueLog : MonoBehaviour
{
    [SerializeField] private DialogueLine dialogueLinePrefab;
    [SerializeField] private Transform dialogueLogContent;
    [SerializeField] private Animator animator;

    private List<DialogueLine> dialogueLines = new();
    private bool opened;
    
    public void DisplayDialogueLog(List<SerializableTuple<string, string>> Dialogues)
    {
        for (var indexDialogue = 0; indexDialogue < Dialogues.Count; indexDialogue++)
        {
            var dialogue = Dialogues[indexDialogue];
            var line = Instantiate(dialogueLinePrefab, dialogueLogContent);
            dialogueLines.Add(line);
            if (dialogue.Item1 == "Sensor") line.DisplayDialogueLine(null, dialogue.Item2);
            if (dialogue.Item1 == "Expert") line.DisplayDialogueLine(null, dialogue.Item2);
            for (int indexCharacter = 0; indexCharacter < GameManager.Instance.SpaceshipManager.characters.Length; indexCharacter++)
            {
                var character = GameManager.Instance.SpaceshipManager.characters[indexCharacter];
                if (character.GetCharacterData().ID == dialogue.Item1)
                {
                    line.DisplayDialogueLine(character.GetCharacterData().characterIcon, dialogue.Item2);
                    line.transform.localScale = Vector3.one;
                    LayoutRebuilder.ForceRebuildLayoutImmediate(dialogueLogContent.GetComponent<RectTransform>());
                }
            }
        }
    }

    public void ClearDialogueLog()
    {
        for (var index = 0; index < dialogueLines.Count; index++)
        {
            var line = dialogueLines[index];
            Destroy(line.gameObject);
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