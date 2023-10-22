using SS.ScriptableObjects;
using UnityEngine;

namespace SS
{
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
    }
}