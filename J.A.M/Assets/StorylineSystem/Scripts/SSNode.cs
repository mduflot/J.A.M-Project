using System;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    using ScriptableObjects;
    
    public class SSNode : MonoBehaviour
    {
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
        
        /* Locations */
        [SerializeField] private List<GameObject> locations;

        public void ShowTimeline()
        {
            if (node is SSStartNodeSO startNode)
            {
                foreach (GameObject location in locations)
                {
                    if (location.name == startNode.LocationType.ToString())
                    {
                        location.SetActive(true);
                    }
                }
            }
        }

        public void StartTimeline()
        {
            
        }
    }
}