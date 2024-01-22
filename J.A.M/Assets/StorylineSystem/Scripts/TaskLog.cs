using System.Collections.Generic;

namespace SS
{
    [System.Serializable]
    public class TaskLog
    {
        public string NodeTaskName;
        public float Duration;
        public float TimeLeft;
        public bool IsStarted;
        public List<string> LeaderCharacter;
        public List<string> AssistantCharacters;
        
        public TaskLog(string nodeTaskName, float duration, float timeLeft, bool isStarted, List<string> leaderCharacter, List<string> assistantCharacters)
        {
            this.NodeTaskName = nodeTaskName;
            this.Duration = duration;
            this.TimeLeft = timeLeft;
            this.IsStarted = isStarted;
            this.LeaderCharacter = leaderCharacter;
            this.AssistantCharacters = assistantCharacters;
        }
    }
}