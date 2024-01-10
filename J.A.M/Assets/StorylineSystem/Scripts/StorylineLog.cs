using System.Collections.Generic;

namespace SS
{
    [System.Serializable]
    public class StorylineLog
    {
        public string storylineID;
        public string storylineName;
        public string storylineDate;
        public List<TimelineLog> timelineLogs;
        
        public StorylineLog(string storylineID, string storylineName, string storylineDate)
        {
            this.storylineID = storylineID;
            this.storylineName = storylineName;
            this.storylineDate = storylineDate;
            timelineLogs = new List<TimelineLog>();
        }
    }
}