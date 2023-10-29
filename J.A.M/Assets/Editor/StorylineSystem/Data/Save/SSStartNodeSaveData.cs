using System;
using UnityEngine;

namespace SS.Data.Save
{
    using Enumerations;
    
    [Serializable]
    public class SSStartNodeSaveData : SSNodeSaveData
    {
        [field: SerializeField] public SSLocationType LocationType { get; set; }
    }
}