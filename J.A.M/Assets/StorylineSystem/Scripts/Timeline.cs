namespace SS
{
    using Enumerations;
    using ScriptableObjects;

    [System.Serializable]
    public class Timeline
    {
        public string ID;
        public SSNodeGroupSO TimelineContainer;
        public SSStoryStatus Status;

        public Timeline(SSNodeGroupSO timelineContainer)
        {
            ID = timelineContainer.ID;
            TimelineContainer = timelineContainer;
            Status = timelineContainer.StoryStatus;
        }
    }
}