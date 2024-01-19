using System.Collections.Generic;

namespace SS
{
    [System.Serializable]
    public class StorylineLog
    {
        public string storylineID;
        public string storylineName;
        public string storylineDate;
        public string storylineEndLog;
        public List<TimelineLog> timelineLogs;
        
        public StorylineLog(string storylineID, string storylineName, string storylineDate, string storylineEndLog)
        {
            this.storylineID = storylineID;
            this.storylineName = storylineName;
            this.storylineDate = storylineDate;
            this.storylineEndLog = storylineEndLog;
            timelineLogs = new List<TimelineLog>();
        }
    }
}