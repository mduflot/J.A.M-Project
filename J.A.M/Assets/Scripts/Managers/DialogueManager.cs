using System.Collections.Generic;
using System.Linq;
using Managers;
using SS.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {
    [Header("Dialogue")]
    [SerializeField] private GameObject dialoguePrefab;
    [SerializeField] private Transform dialogueParent;
    private Pool<GameObject> dialoguesPool;
    private List<GameObject> dialogues = new();

    [Header("Sprites")]
    [SerializeField] private Sprite sensor;
    [SerializeField] private Sprite expert;
    [SerializeField] private Sprite incomingSignal;

    [Header("Menu")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI buttonText;

    private void Awake() {
        dialoguesPool = new Pool<GameObject>(dialoguePrefab, 5);
    }

    public void InitializeMenu(string title) {
        TimeTickSystem.ModifyTimeScale(0);
        gameObject.SetActive(true);
        button.interactable = false;
        titleText.text = $"{title}";
        buttonText.text = "...";
    }

    public void AddDialogue(SSDialogueNodeSO node, string characterName) {
        var dialogueGO = dialoguesPool.GetFromPool();
        dialogueGO.transform.SetParent(dialogueParent);
        dialogues.Add(dialogueGO);
        var dialogueComponent = dialogueGO.GetComponent<Dialogue>();
        switch (characterName) {
            case "Sensor":
                dialogueComponent.DisplayDialogue(sensor, characterName, node);
                break;
            case "Expert":
                dialogueComponent.DisplayDialogue(expert, characterName, node);
                break;
            case "Incoming Signal":
                dialogueComponent.DisplayDialogue(incomingSignal, characterName, node);
                break;
            default:
                dialogueComponent.DisplayDialogue(
                    GameManager.Instance.SpaceshipManager.characters
                        .First(character => character.GetCharacterData().firstName == characterName)
                        .GetCharacterData()
                        .characterIcon, characterName, node);
                break;
        }
    }

    public void ActivateButton() {
        button.interactable = true;
        buttonText.text = "Leave";
    }

    public void CloseDialogueMenu() {
        foreach (var dialogue in dialogues) {
            dialoguesPool.AddToPool(dialogue);
        }

        TimeTickSystem.ModifyTimeScale(1);
        gameObject.SetActive(false);
    }
}