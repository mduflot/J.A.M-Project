using UnityEngine;

public class Task
{
    public string Name;
    public string Description;
    public Sprite Icon;
    public float TimeLeft;
    public float Duration;
    public float BaseDuration;
    public int MandatorySlots;
    public int OptionalSlots;
    public float HelpFactor;
    public RoomType Room;
    public bool IsPermanent;
    public string PreviewOutcome;

    public Task(string name, string description, Sprite icon, float timeLeft, float duration, int mandatorySlots,
        int optionalSlots, float helpFactor, RoomType room, bool isPermanent, string previewOutcome)
    {
        Name = name;
        Description = description;
        Icon = icon;
        TimeLeft = timeLeft;
        Duration = duration;
        BaseDuration = duration;
        MandatorySlots = mandatorySlots;
        OptionalSlots = optionalSlots;
        HelpFactor = helpFactor;
        Room = room;
        IsPermanent = isPermanent;
        PreviewOutcome = previewOutcome;
    }
}