using System.Collections;
using SS.ScriptableObjects;
using TMPro;
using UnityEngine;

public class CharacterSpeaker : MonoBehaviour
{
    [SerializeField] private GameObject dialogueContainer;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI dialogue;

    private CharacterBehaviour characterBehaviour;
    public bool isSpeaking;

    public void Initialize(CharacterBehaviour behaviour)
    {
        characterBehaviour = behaviour;
    }

    public void StartDialogue(SSDialogueNodeSO node)
    {
        isSpeaking = true;
        if (node.IsDialogueTask)
        {
            // TODO : Call TaskNotification here to get the percentage of the task
            // yield return new WaitUntil();
        }

        dialogue.text = node.Text;
        characterName.text = characterBehaviour.GetCharacterData().firstName;
        dialogueContainer.SetActive(true);
    }

    public void EndDialogue()
    {
        dialogueContainer.SetActive(false);
        isSpeaking = false;
    }
}