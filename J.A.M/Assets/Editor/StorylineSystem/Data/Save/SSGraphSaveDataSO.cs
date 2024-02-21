using System.Collections.Generic;
using UnityEngine;

namespace SS.Data.Save
{
    using Enumerations;

    public class SSGraphSaveDataSO : ScriptableObject
    {
        [field: SerializeField] public string FileName { get; set; }
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public SSStoryStatus StoryStatus { get; set; }
        [field: SerializeField] public SSStoryType StoryType { get; set; }
        [field: SerializeField] public SSSpontaneousType SpontaneousType { get; set; }
        [field: SerializeField] public bool IsTutorialToPlay { get; set; }
        [field: SerializeField] public bool IsFirstToPlay { get; set; }
        [field: SerializeField] public bool IsReplayable { get; set; }
        [field: SerializeField] public ConditionSO Condition { get; set; }
        [field: SerializeField] public List<SSGroupSaveData> Groups { get; set; }
        [field: SerializeReference] public List<SSNodeSaveData> Nodes { get; set; }
        [field: SerializeField] public List<string> OldGroupNames { get; set; }
        [field: SerializeField] public List<string> OldUngroupedNodeNames { get; set; }
        [field: SerializeField] public SerializableDictionary<string, List<string>> OldGroupedNodeNames { get; set; }

        public void Initialize(string fileName, SSStoryStatus storyStatus, SSStoryType storyType, SSSpontaneousType spontaneousType, bool isTutorialToPlay, bool isFirstToPlay, bool isReplayable, ConditionSO condition)
        {
            ID = System.Guid.NewGuid().ToString();
            FileName = fileName;
            StoryStatus = storyStatus;
            StoryType = storyType;
            SpontaneousType = spontaneousType;
            IsTutorialToPlay = isTutorialToPlay;
            IsFirstToPlay = isFirstToPlay;
            IsReplayable = isReplayable;
            Condition = condition;
            Groups = new List<SSGroupSaveData>();
            Nodes = new List<SSNodeSaveData>();
        }
    }
}