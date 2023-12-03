using System.Collections.Generic;
using UnityEngine;

namespace SS.ScriptableObjects
{
    using Enumerations;

    [CreateAssetMenu(fileName = "New Campaign", menuName = "SSSystem/Campaign")]
    public class SSCampaignSO : ScriptableObject
    {
        [field: SerializeField] public List<Storyline> Storylines { get; set; }
        
        [HideInInspector] public List<Storyline> ActivatableStorylines;
        [HideInInspector] public List<Storyline> DisabledStorylines;

        private void OnValidate()
        {
            if (Storylines == null) return;

            foreach (var storyline in Storylines)
            {
                if (storyline.StorylineContainer == null) continue;
                if (storyline.StorylineContainer.StoryStatus == SSStoryStatus.Activatable || storyline.StorylineContainer.StoryStatus == SSStoryStatus.Beginning)
                    ActivatableStorylines.Add(storyline);
                else
                    DisabledStorylines.Add(storyline);
                if (storyline.ActivatableTimelines.Count != 0) continue;
                if (storyline.DisabledTimelines.Count != 0) continue;
                FillGroups(storyline);
            }
        }

        private void FillGroups(Storyline storyline)
        {
            foreach (var nodeGroup in storyline.StorylineContainer.NodeGroups)
            {
                if (nodeGroup.Key.StoryStatus == SSStoryStatus.Activatable || nodeGroup.Key.StoryStatus == SSStoryStatus.Beginning)
                    storyline.ActivatableTimelines.Add(
                        new SerializableTuple<SSStoryStatus, SSNodeGroupSO>(nodeGroup.Key.StoryStatus, nodeGroup.Key));
                else
                    storyline.DisabledTimelines.Add(
                        new SerializableTuple<SSStoryStatus, SSNodeGroupSO>(nodeGroup.Key.StoryStatus, nodeGroup.Key));
            }
        }

        [ContextMenu("FillGroups")]
        private void FillGroups()
        {
            if (Storylines == null) return;

            foreach (var storyline in Storylines)
            {
                if (storyline.StorylineContainer == null) continue;
                storyline.ActivatableTimelines = new List<SerializableTuple<SSStoryStatus, SSNodeGroupSO>>();
                storyline.DisabledTimelines = new List<SerializableTuple<SSStoryStatus, SSNodeGroupSO>>();
                FillGroups(storyline);
            }
        }
    }
}