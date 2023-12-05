using System.Collections.Generic;
using SS.Enumerations;
using UnityEngine;

namespace SS.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Campaign", menuName = "SSSystem/Campaign")]
    public class SSCampaignSO : ScriptableObject
    {
        [field: SerializeField] public List<Storyline> Storylines { get; set; }
        
        public List<Storyline> PrincipalStorylines = new();
        public List<Storyline> SecondaryStorylines = new();
        public List<Storyline> TrivialStorylines = new();

        private void OnValidate()
        {
            if (Storylines == null) return;

            for (var index = 0; index < Storylines.Count; index++)
            {
                var storyline = Storylines[index];
                switch(storyline.StorylineContainer.StoryType)
                {
                    case SSStoryType.Principal:
                        PrincipalStorylines.Add(storyline);
                        break;
                    case SSStoryType.Secondary:
                        SecondaryStorylines.Add(storyline);
                        break;
                    case SSStoryType.Trivial:
                        TrivialStorylines.Add(storyline);
                        break;
                }
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

        [ContextMenu("ResetGroups")]
        private void ResetGroups()
        {
            for (var index = 0; index < Storylines.Count; index++)
            {
                var storyline = Storylines[index];
                if (storyline.StorylineContainer == null) continue;
                FillGroups(storyline);
            }
        }
    }
}