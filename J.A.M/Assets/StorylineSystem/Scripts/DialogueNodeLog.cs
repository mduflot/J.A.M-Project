namespace SS
{
    [System.Serializable]
    public class DialogueNodeLog : NodeLog
    {
        public string DialogueText;
        public string SpeakerName;

        public DialogueNodeLog(string nodeName, string dialogueText, string speakerName)
        {
            NodeName = nodeName;
            DialogueText = dialogueText;
            SpeakerName = speakerName;
        }
    }
}