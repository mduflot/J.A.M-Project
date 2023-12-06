using SS.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Speaker : MonoBehaviour
{
    public bool IsSpeaking;

    [SerializeField] private GameObject dialogueContainer;
    [FormerlySerializedAs("characterName")] [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI dialogue;

    public void StartDialogue(SSDialogueNodeSO node, string characterName)
    {
        IsSpeaking = true;
        dialogue.text = node.Text;
        characterNameText.text = characterName;
        dialogueContainer.SetActive(true);
    }

    public void EndDialogue()
    {
        dialogueContainer.SetActive(false);
        IsSpeaking = false;
    }
}