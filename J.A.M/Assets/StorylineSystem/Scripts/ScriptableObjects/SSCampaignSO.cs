using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SS.ScriptableObjects
{
    using Enumerations;

    [CreateAssetMenu(fileName = "New Campaign", menuName = "SSSystem/Campaign")]
    public class SSCampaignSO : ScriptableObject
    {
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public List<Storyline> Storylines { get; set; }
        [field: HideInInspector] public List<Storyline> PrincipalStorylines = new();
        [field: HideInInspector] public List<Storyline> SecondaryStorylines = new();
        [field: HideInInspector] public List<Storyline> TrivialStorylines = new();

        [ContextMenu("Initialize GUID")]
        private void Initialize()
        {
            ID = System.Guid.NewGuid().ToString();
            foreach (Storyline storyline in Storylines)
            {
                storyline.ID = System.Guid.NewGuid().ToString();
                foreach (Timeline timeline in storyline.Timelines)
                {
                    timeline.ID = System.Guid.NewGuid().ToString();
                }
            }
        }

        private void FillGroups(Storyline storyline)
        {
            storyline.Timelines = new List<Timeline>();
            for (int index = 0; index < storyline.StorylineContainer.NodeGroups.Count; index++)
            {
                var group = storyline.StorylineContainer.NodeGroups.ElementAt(index);
                storyline.Timelines.Add(new Timeline(group.Key));
            }
        }

        [ContextMenu("FillGroups")]
        private void FillGroups()
        {
            PrincipalStorylines.Clear();
            SecondaryStorylines.Clear();
            TrivialStorylines.Clear();
            for (var index = 0; index < Storylines.Count; index++)
            {
                var storyline = Storylines[index];
                Storylines[index].Status = Storylines[index].StorylineContainer.StoryStatus;
                storyline.Status = storyline.StorylineContainer.StoryStatus;
                if (storyline.StorylineContainer == null) continue;
                switch (storyline.StorylineContainer.StoryType)
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

                FillGroups(storyline);
            }
        }
    }
}