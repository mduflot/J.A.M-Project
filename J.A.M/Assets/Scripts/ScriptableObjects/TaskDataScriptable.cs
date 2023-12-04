using System.Collections.Generic;
using Unity.VisualScripting;
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

    public ConditionSO[] conditions;
    
    public bool isPermanent;
    public BaseTaskOutcome[] outcomes;
    [TextArea(3, 5)] public string previewOutcome;
}