using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SS
{
    using Enumerations;
    using ScriptableObjects;
    
    public class SSNode : MonoBehaviour
    {
        /* UI GameObjects */
        [SerializeField] private List<Button> locations;
        [SerializeField] private GameObject dialogue;
        [SerializeField] private GameObject popupCharacters;
        [SerializeField] private GameObject eventDescription;
        [SerializeField] private Button eventButton;
        [SerializeField] private GameObject characterSlot;
        [SerializeField] private GameObject notification;

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

        [ContextMenu("StartTimeline")]
        public void StartTimeline()
        {
            CheckNodeType(node);
        }

        public void CheckNodeType(SSNodeSO nodeSO)
        {
            switch (nodeSO.NodeType)
            {
                case SSNodeType.Start:
                {
                    RunNode(nodeSO as SSStartNodeSO);
                    break;
                }
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

        private void RunNode(SSStartNodeSO startNodeSO)
        {
            foreach (var location in locations)
            {
                if (location.name == startNodeSO.LocationType.ToString())
                {
                    location.gameObject.SetActive(true);
                    location.onClick.AddListener(() => CheckNodeType(startNodeSO.Choices[0].NextNode));
                    location.onClick.AddListener(() => location.gameObject.SetActive(false));
                }
            }
        }

        private void RunNode(SSRewardNodeSO rewardNodeSo)
        {
            foreach (var location in locations)
            {
                location.gameObject.SetActive(false);
            }
            popupCharacters.SetActive(false);
            dialogue.SetActive(false);
            eventButton.gameObject.SetActive(false);
            notification.SetActive(true);
            notification.GetComponent<TextMeshProUGUI>().text = "You gain : " + rewardNodeSo.RewardTypes;
        }

        private void RunNode(SSDialogueNodeSO nodeSO)
        {
            dialogue.SetActive(true);
            dialogue.GetComponent<TextMeshProUGUI>().text = nodeSO.Text;
            StartCoroutine(WaiterDialogue(nodeSO));
        }
        
        private void RunNode(SSTaskNodeSO nodeSO)
        {
            popupCharacters.SetActive(true);
            eventDescription.SetActive(true);
            eventButton.gameObject.SetActive(true);
            eventButton.onClick.AddListener(() => CheckNodeType(nodeSO.Choices[0].NextNode));
            eventButton.onClick.AddListener(() => popupCharacters.SetActive(false));
        }

        IEnumerator WaiterDialogue(SSNodeSO nodeSO)
        {
            yield return new WaitForSecondsRealtime(5);
            dialogue.SetActive(false);
            CheckNodeType(nodeSO.Choices[0].NextNode);
        }
    }
}