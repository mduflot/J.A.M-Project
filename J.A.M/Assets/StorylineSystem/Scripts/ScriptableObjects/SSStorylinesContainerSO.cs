using System.Collections.Generic;
using SS.Enumerations;
using UnityEngine;

namespace SS.ScriptableObjects
{
    [CreateAssetMenu(fileName = "StorylinesContainer", menuName = "StorylineSystem/StorylinesContainer")]
    public class SSStorylinesContainerSO : ScriptableObject
    {
        [field: SerializeField] private List<Storyline> Storylines { get; set; }

        private void OnValidate()
        {
            if (Storylines == null) return;

            foreach (var storyline in Storylines)
            {
                if (storyline.NodeGroups.Count != 0) continue;
                FillGroups(storyline);
            }
        }

        private void FillGroups(Storyline storyline)
        {
            foreach (var nodeGroup in storyline.NodeContainer.NodeGroups)
            {
                storyline.NodeGroups.Add(
                    new SerializableTuple<SSStatus, SSNodeGroupSO>(SSStatus.Disabled, nodeGroup.Key));
            }
        }

        [ContextMenu("FillGroups")]
        private void FillGroups()
        {
            if (Storylines == null) return;

            foreach (var storyline in Storylines)
            {
                if (storyline.NodeContainer == null) continue;
                storyline.NodeGroups = new List<SerializableTuple<SSStatus, SSNodeGroupSO>>();
                FillGroups(storyline);
            }
        }
    }
}