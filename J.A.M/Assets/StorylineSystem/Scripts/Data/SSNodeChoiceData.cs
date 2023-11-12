using System;
using UnityEngine;

namespace SS.Data
{
    using ScriptableObjects;
    
    [Serializable]
    public class SSNodeChoiceData 
    {
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public SSNodeSO NextNode { get; set; }
        [field: SerializeField] public SSNodeSO PreviousNode { get; set; }
    }
}