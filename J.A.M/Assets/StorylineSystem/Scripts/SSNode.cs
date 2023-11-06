using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SS
{
    using Enumerations;
    using ScriptableObjects;

    public class SSNode : MonoBehaviour
    {
        /* UI GameObjects */
        [SerializeField] private GameObject dialogueLayout;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private TextMeshProUGUI nameSpeaker;
        [SerializeField] private SpaceshipManager spaceshipManager;

        private List<CharacterBehaviour> characters = new();
        private List<CharacterBehaviour> assignedCharacters = new();
        private SSTimeNodeSO timeNodeSO;
        private uint duration;
        
        /* Node Scriptable Objects */
        [SerializeField] private SSNodeContainerSO nodeContainer;
        [SerializeField] private SSNodeGroupSO nodeGroup;
        [SerializeField] private SSNodeSO node;

        /* Filters */
        [SerializeField] private bool groupedNodes;
        [SerializeField] private bool startingNodesOnly;

        /* Indexes */
        [SerializeField] private int selectedNodeGroupIndex;
        [SerializeField] private int selectedNodeIndex;

        public void StartTimeline()
        {
            characters.AddRange(spaceshipManager.characters);
            CheckNodeType(node);
        }

        public void CheckNodeType(SSNodeSO nodeSO)
        {
            switch (nodeSO.NodeType)
            {
                case SSNodeType.Dialogue:
                {
                    RunNode(nodeSO as SSDialogueNodeSO);
                    break;
                }
                case SSNodeType.Task:
                {
                    RunNode(nodeSO as SSTaskNodeSO);
                    break;
                }
                case SSNodeType.Reward:
                {
                    RunNode(nodeSO as SSRewardNodeSO);
                    break;
                }
                case SSNodeType.Time:
                {
                    RunNode(nodeSO as SSTimeNodeSO);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void RunNode(SSRewardNodeSO nodeSO)
        {
            if (nodeSO.Choices.First().NextNode == null)
            {
                return;
            }
            CheckNodeType(nodeSO.Choices.First().NextNode);
        }

        private void RunNode(SSDialogueNodeSO nodeSO)
        {
            dialogueLayout.SetActive(true);
            dialogueText.gameObject.SetActive(true);
            nameSpeaker.gameObject.SetActive(true);
            dialogueText.text = nodeSO.Text;
            switch (nodeSO.SpeakerType)
            {
                case SSSpeakerType.Random :
                    nameSpeaker.text = characters[Random.Range(0, characters.Count)].GetCharacterData().firstName;
                    break;
                case SSSpeakerType.Sensor :
                    nameSpeaker.text = "Sensor";
                    break;
                case SSSpeakerType.Character1 :
                    nameSpeaker.text = characters[0].GetCharacterData().firstName;
                    break;
                case SSSpeakerType.Character2 :
                    nameSpeaker.text = characters[1].GetCharacterData().firstName;
                    break;
                case SSSpeakerType.Assigned1 :
                    // nameSpeaker.text = assignedCharacters[0].GetCharacterData().firstName;
                    break;
                case SSSpeakerType.Assigned2 :
                    // nameSpeaker.text = assignedCharacters[1].GetCharacterData().firstName;
                    break;
                default:
                    nameSpeaker.text = "Narrator";
                    break;
            }
            StartCoroutine(WaiterDialogue(nodeSO));
        }

        private void RunNode(SSTaskNodeSO nodeSO)
        {
            spaceshipManager.SpawnTask(nodeSO.TaskData);
            assignedCharacters.AddRange(spaceshipManager.GetTaskNotification(nodeSO.TaskData).LeaderCharacters);
            assignedCharacters.AddRange(spaceshipManager.GetTaskNotification(nodeSO.TaskData).AssistantCharacters);
            StartCoroutine(WaiterTask(nodeSO));
        }

        private void RunNode(SSTimeNodeSO nodeSO)
        {
            timeNodeSO = nodeSO;
            duration = nodeSO.TimeToWait * TimeTickSystem.ticksPerHour;
            TimeTickSystem.OnTick += WaitingTime;
        }

        private void WaitingTime(object sender, TimeTickSystem.OnTickEventArgs e)
        {
            duration -= TimeTickSystem.timePerTick;
            if (duration <= 0)
            {
                if (timeNodeSO.Choices.First().NextNode == null)
                {
                    return;
                }
                CheckNodeType(timeNodeSO.Choices.First().NextNode);
                TimeTickSystem.OnTick -= WaitingTime;
            }
        }

        IEnumerator WaiterDialogue(SSDialogueNodeSO nodeSO)
        {
            yield return new WaitForSecondsRealtime(5);
            dialogueLayout.SetActive(false);
            dialogueText.gameObject.SetActive(false);
            nameSpeaker.gameObject.SetActive(false);
            if (nodeSO.Choices.First().NextNode == null)
            {
                yield break;
            }
            CheckNodeType(nodeSO.Choices.First().NextNode);
        }

        IEnumerator WaiterTask(SSTaskNodeSO nodeSO)
        {
            yield return new WaitUntil(() => spaceshipManager.GetTaskNotification(nodeSO.TaskData).TaskStarted);
            if (nodeSO.Choices.First().NextNode == null)
            {
                yield break;
            }
            CheckNodeType(nodeSO.Choices.First().NextNode);
        }
    }
}