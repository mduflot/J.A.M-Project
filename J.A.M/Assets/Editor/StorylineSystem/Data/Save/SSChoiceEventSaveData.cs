using System.Collections.Generic;
using UnityEngine;

namespace SS.Data.Save
{
    using Enumerations;
    
    public class SSChoiceEventSaveData : SSChoiceSaveData
    {
        [field: SerializeField] public List<SSChoiceType> ChoiceTypes;
    }
}