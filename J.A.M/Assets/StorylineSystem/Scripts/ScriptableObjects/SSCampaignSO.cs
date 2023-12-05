using System.Collections.Generic;
using System.Linq;
using SS.Enumerations;
using UnityEngine;

namespace SS.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Campaign", menuName = "SSSystem/Campaign")]
    public class SSCampaignSO : ScriptableObject
    {
        [field: SerializeField] public List<Storyline> Storylines { get; set; }
        
        [HideInInspector] public List<Storyline> PrincipalStorylines = new();
        [HideInInspector] public List<Storyline> SecondaryStorylines = new();
        [HideInInspector] public List<Storyline> TrivialStorylines = new();

        private void FillGroups(Storyline storyline)
        {
            storyline.Timelines = new List<SSNodeGroupSO>();
            for (int index = 0; index < storyline.StorylineContainer.NodeGroups.Count; index++)
            {
                var group = storyline.StorylineContainer.NodeGroups.ElementAt(index);
                storyline.Timelines.Add(group.Key);
            }
        }

        [ContextMenu("FillGroups")]
        private void FillGroups()
        {
            for (var index = 0; index < Storylines.Count; index++)
            {
                var storyline = Storylines[index];
                if (storyline.StorylineContainer == null) continue;
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
                FillGroups(storyline);
            }
        }
    }
}