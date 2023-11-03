using System.Collections.Generic;
using UnityEngine;

namespace SS.Data
{
    using Enumerations;
    
    public class SSNodeEventChoiceData : SSNodeChoiceData
    {
        [field: SerializeField] public List<SSChoiceType> ChoiceTypes;
    }
}