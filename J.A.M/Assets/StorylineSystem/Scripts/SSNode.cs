using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

namespace SS
{
    using Enumerations;
    using ScriptableObjects;

    public class SSNode : MonoBehaviour
    {
        /* UI GameObjects */
        [SerializeField] private GameObject dialogue;
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
            if (nodeSO.Choices.Count == 0)
            {
                return;
            }
            CheckNodeType(nodeSO.Choices.First().NextNode);
        }

        private void RunNode(SSDialogueNodeSO nodeSO)
        {
            dialogue.SetActive(true);
            dialogue.GetComponent<TextMeshProUGUI>().text = nodeSO.Text;
            StartCoroutine(WaiterDialogue(nodeSO));
        }

        private void RunNode(SSTaskNodeSO nodeSO)
        {
            spaceshipManager.SpawnTask(nodeSO.TaskData);
        }

        IEnumerator WaiterDialogue(SSNodeSO nodeSO)
        {
            yield return new WaitForSecondsRealtime(5);
            dialogue.SetActive(false);
            if (nodeSO.Choices.Count == 0)
            {
                yield break;
            }
            CheckNodeType(nodeSO.Choices.First().NextNode);
        }
    }
}