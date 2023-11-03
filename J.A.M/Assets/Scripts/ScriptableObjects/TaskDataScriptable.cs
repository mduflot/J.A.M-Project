using UnityEngine;

[CreateAssetMenu(menuName = "Task/TaskData", fileName = "TaskData")]
public class TaskDataScriptable : ScriptableObject
{
    public string taskName;
    public string descriptionTask;
    public float timeLeft;
    public float baseDuration;
    public int mandatorySlots;
    public int optionalSlots;
    public SpaceshipManager.System system;
    
    
    [Header("Permanent Task")]
    public bool isPermanent;
    public BaseTaskOutcome[] outcomes;


}
