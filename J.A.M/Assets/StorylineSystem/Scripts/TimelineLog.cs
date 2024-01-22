namespace SS
{
    [System.Serializable]
    public class TimelineLog
    {
        public string timelineID;
        public string timelineName;
        public string timelineDate;
        
        public TimelineLog(string timelineID, string timelineName, string timelineDate)
        {
            this.timelineID = timelineID;
            this.timelineName = timelineName;
            this.timelineDate = timelineDate;
        }
    }
}