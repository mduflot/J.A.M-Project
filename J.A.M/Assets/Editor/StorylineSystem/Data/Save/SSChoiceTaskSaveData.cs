using System;
using UnityEngine;

namespace SS.Data.Save
{
    [Serializable]
    public class SSChoiceTaskSaveData : SSChoiceSaveData
    {
        [field: SerializeField] public TraitsData.Job Jobs { get; set; }
        [field: SerializeField] public TraitsData.PositiveTraits PositiveTraits { get; set; }
        [field: SerializeField] public TraitsData.NegativeTraits NegativeTraits { get; set; }
    }
}