using System.Collections.Generic;
using SS.Enumerations;
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
                storyline.StoryStatus = storyline.StorylineContainer.StoryStatus;
                if (storyline.EnabledTimelines.Count != 0) continue;
                if (storyline.DisabledTimelines.Count != 0) continue;
                FillGroups(storyline);
            }
        }

        private void FillGroups(Storyline storyline)
        {
            storyline.EnabledTimelines = new List<SSNodeGroupSO>();
            storyline.DisabledTimelines = new List<SSNodeGroupSO>();
            foreach (var group in storyline.StorylineContainer.NodeGroups)
            {
                if (group.Key.StoryStatus == SSStoryStatus.Enabled) storyline.EnabledTimelines.Add(group.Key);
                if (group.Key.StoryStatus == SSStoryStatus.Disabled) storyline.DisabledTimelines.Add(group.Key);
            }
        }
    }
}