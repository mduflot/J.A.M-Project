using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
        [SerializeField] private List<CharacterBehaviour> characters;
        [SerializeField] private SpaceshipManager spaceshipManager;

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
                    nameSpeaker.text = characters[Random.Range(0, characters.Count)].data.firstName;
                    break;
                case SSSpeakerType.Sensor :
                    nameSpeaker.text = "Sensor";
                    break;
                case SSSpeakerType.Character1 :
                    nameSpeaker.text = characters[0].data.firstName;
                    break;
                case SSSpeakerType.Character2 :
                    nameSpeaker.text = characters[1].data.firstName;
                    break;
                default:
                    nameSpeaker.text = "Wait... is not working for now";
                    break;
            }
            StartCoroutine(WaiterDialogue(nodeSO));
        }

        private void RunNode(SSTaskNodeSO nodeSO)
        {
            spaceshipManager.SpawnTask(nodeSO.TaskData);
            StartCoroutine(WaiterTask(nodeSO));
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