using UnityEngine;

[CreateAssetMenu(menuName = "Task/TaskData", fileName = "TaskData")]
public class TaskDataScriptable : ScriptableObject
{
    public string taskName;
    
    [TextArea(5, 10)]
    public string descriptionTask;
    public Sprite taskIcon;
    public float timeLeft;
    public float baseDuration;
    public int mandatorySlots;
    public int optionalSlots;
    public float taskHelpFactor = .75f;
    public SpaceshipManager.ShipRooms room;
    
    
    [Header("Permanent Task")]
    public bool isPermanent;
    public BaseTaskOutcome[] outcomes;


}
