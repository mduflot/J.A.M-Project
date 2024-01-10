using System.Collections.Generic;

namespace SS
{
    [System.Serializable]
    public class TaskLog
    {
        public float Duration;
        public string LeaderCharacter;
        public List<string> AssistantCharacters;
        
        public TaskLog(float duration, string leaderCharacter, List<string> assistantCharacters)
        {
            Duration = duration;
            this.LeaderCharacter = leaderCharacter;
            this.AssistantCharacters = assistantCharacters;
        }
    }
}