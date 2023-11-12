using System;
using System.Collections.Generic;
using UnityEngine;

namespace SS.Data.Save
{
    using Enumerations;
    
    [Serializable]
    public class SSChoiceTaskSaveData : SSChoiceSaveData
    {
        [field: SerializeField] public List<SSChoiceType> ChoiceTypes;
    }
}