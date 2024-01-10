using System.Collections.Generic;

namespace SS
{
    [System.Serializable]
    public class TaskLog
    {
        public string NodeTaskName;
        public float Duration;
        public string LeaderCharacter;
        public List<string> AssistantCharacters;
        
        public TaskLog(string nodeTaskName, float duration, string leaderCharacter, List<string> assistantCharacters)
        {
            this.NodeTaskName = nodeTaskName;
            this.Duration = duration;
            this.LeaderCharacter = leaderCharacter;
            this.AssistantCharacters = assistantCharacters;
        }
    }
}