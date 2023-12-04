using System.Collections.Generic;
using UnityEngine;

namespace SS.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Campaign", menuName = "SSSystem/Campaign")]
    public class SSCampaignSO : ScriptableObject
    {
        [field: SerializeField] public List<Storyline> Storylines { get; set; }

        private void OnValidate()
        {
            if (Storylines == null) return;

            foreach (var storyline in Storylines)
            {
                if (storyline.StorylineContainer == null) continue;
                if (storyline.Timelines.Count != 0) continue;
                FillGroups(storyline);
            }
        }

        private void FillGroups(Storyline storyline)
        {
            storyline.Timelines = new List<SSNodeGroupSO>();
            foreach (var group in storyline.StorylineContainer.NodeGroups)
            {
                storyline.Timelines.Add(group.Key);
            }
        }
    }
}