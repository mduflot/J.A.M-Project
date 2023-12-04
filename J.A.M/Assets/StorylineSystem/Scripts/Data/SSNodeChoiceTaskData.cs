using System;
using System.Collections.Generic;
using UnityEngine;

namespace SS.Data
{
    using Enumerations;
    using ScriptableObjects;

    [Serializable]
    public class SSNodeChoiceTaskData : SSNodeChoiceData
    {
        [field: SerializeField] public ConditionSO Condition { get; set; }
    }
}