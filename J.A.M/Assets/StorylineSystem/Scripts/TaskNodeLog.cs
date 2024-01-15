using System.Collections.Generic;

namespace SS
{
    [System.Serializable]
    public class TaskNodeLog : NodeLog
    {
        public string TaskName;
        public string TaskDescription;
        public List<string> TaskCharacters;

        public TaskNodeLog(string nodeName, string taskName, string taskDescription, List<string> taskCharacters)
        {
            NodeName = nodeName;
            TaskName = taskName;
            TaskDescription = taskDescription;
            TaskCharacters = taskCharacters;
        }
    }
}