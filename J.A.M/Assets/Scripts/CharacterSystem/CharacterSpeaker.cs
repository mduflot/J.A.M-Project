using SS.ScriptableObjects;
using TMPro;
using UnityEngine;

public class CharacterSpeaker : MonoBehaviour
{
    public bool IsSpeaking;

    [SerializeField] private GameObject dialogueContainer;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI dialogue;

    private CharacterBehaviour characterBehaviour;

    public void Initialize(CharacterBehaviour behaviour)
    {
        characterBehaviour = behaviour;
    }

    public void StartDialogue(SSDialogueNodeSO node)
    {
        IsSpeaking = true;
        dialogue.text = node.Text;
        characterName.text = characterBehaviour.GetCharacterData().firstName;
        dialogueContainer.SetActive(true);
    }

    public void EndDialogue()
    {
        dialogueContainer.SetActive(false);
        IsSpeaking = false;
    }
}