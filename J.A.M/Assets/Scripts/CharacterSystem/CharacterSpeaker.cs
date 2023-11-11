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
    private bool isSpeaking;

    public void Initialize(CharacterBehaviour behaviour)
    {
        characterBehaviour = behaviour;
    }
    
    public void StartDialogue(SSDialogueNodeSO node)
    {
        StartCoroutine(DisplayDialogue(node));
    } 
    
    private IEnumerator DisplayDialogue(SSDialogueNodeSO node)
    {
        yield return new WaitUntil(() => isSpeaking == false);
        if (node.IsDialogueTask)
        {
            // TODO : Get PercentageTask here
            // yield return new WaitUntil();
        }
        dialogue.text = node.Text;
        characterName.text = characterBehaviour.GetCharacterData().firstName;
        dialogueContainer.SetActive(true);
        isSpeaking = true;
        
        yield return new WaitForSeconds(node.Duration);
        dialogueContainer.SetActive(false);
        isSpeaking = false;
    }
}