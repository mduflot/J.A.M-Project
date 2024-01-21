using System.Collections;
using System.Linq;
using Managers;
using SS.ScriptableObjects;
using Tasks;
using UnityEngine;

namespace CharacterSystem
{
    public class DialogueManager : MonoBehaviour
    {
        [Header("Dialogue")] 
        [SerializeField] private GameObject dialoguePrefab;
        [SerializeField] private Transform dialogueParent;

        [SerializeField] private Sprite sensor;
        [SerializeField] private Sprite expert;
        [SerializeField] private Sprite incomingSignal;

        public void AddDialogue(Task task, SSDialogueNodeSO node, string characterName)
        {
            StartCoroutine(InitializeDialogue(task, node, characterName));
        }

        private IEnumerator InitializeDialogue(Task task, SSDialogueNodeSO node, string characterName)
        {
            if (node.IsDialogueTask)
                yield return new WaitUntil(() => 100 - Mathf.Clamp(task.Duration / task.BaseDuration, 0, 1) * 100 > node.PercentageTask);
            var duration = node.Duration * TimeTickSystem.ticksPerHour;
            var dialogueContainer = Instantiate(dialoguePrefab, dialogueParent);
            var dialogueComponent = dialogueContainer.GetComponent<Dialogue>();
            switch (characterName)
            {
                case "Sensor":
                    dialogueComponent.DisplayDialogue(sensor, characterName, node, duration);
                    break;
                case "Expert":
                    dialogueComponent.DisplayDialogue(expert, characterName, node, duration);
                    break;
                case "Incoming Signal":
                    dialogueComponent.DisplayDialogue(incomingSignal, characterName, node, duration);
                    break;
                default:
                    dialogueComponent.DisplayDialogue(
                        GameManager.Instance.SpaceshipManager.characters
                            .First(character => character.GetCharacterData().firstName == characterName).GetCharacterData()
                            .characterIcon,
                        characterName, node, duration);
                    break;
            }
        }
    }
}