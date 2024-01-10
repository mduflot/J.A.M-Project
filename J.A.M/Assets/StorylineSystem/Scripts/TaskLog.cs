using System.Collections.Generic;

namespace SS
{
    [System.Serializable]
    public class TaskLog
    {
        public float TimeLeft;
        public string leaderCharacter;
        public List<string> assistantCharacters;
        
        public TaskLog(float timeLeft, string leaderCharacter, List<string> assistantCharacters)
        {
            TimeLeft = timeLeft;
            this.leaderCharacter = leaderCharacter;
            this.assistantCharacters = assistantCharacters;
        }
    }
}