using System.Collections.Generic;
using UnityEngine;

namespace SS.ScriptableObjects
{
    [CreateAssetMenu(fileName = "StorylinesContainer", menuName = "StorylineSystem/StorylinesContainer")]
    public class SSStorylinesContainerSO : ScriptableObject
    {
        [field: SerializeField] private List<Storyline> Storylines { get; set; }
    }
}