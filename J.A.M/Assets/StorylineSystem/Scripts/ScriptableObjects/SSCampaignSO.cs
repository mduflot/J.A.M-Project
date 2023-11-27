using System.Collections.Generic;
using UnityEngine;

namespace SS.ScriptableObjects
{
    using Enumerations;

    [CreateAssetMenu(fileName = "New Campaign", menuName = "SSSystem/Campaign")]
    public class SSCampaignSO : ScriptableObject
    {
        [field: SerializeField] public List<Storyline> Storylines { get; set; }

        private void OnValidate()
        {
            if (Storylines == null) return;

            foreach (var storyline in Storylines)
            {
                if (storyline.NodeContainer == null) continue;
                if (storyline.NodeGroups.Count != 0) continue;
                FillGroups(storyline);
            }
        }

        private void FillGroups(Storyline storyline)
        {
            foreach (var nodeGroup in storyline.NodeContainer.NodeGroups)
            {
                storyline.NodeGroups.Add(
                    new SerializableTuple<SSStatus, SSNodeGroupSO>(nodeGroup.Key.Status, nodeGroup.Key));
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