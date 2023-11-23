using System;
using UnityEngine;

namespace SS.Data.Save
{
    using Enumerations;

    [Serializable]
    public class SSGroupSaveData
    {
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public SSStatus Status { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; }
    }
}