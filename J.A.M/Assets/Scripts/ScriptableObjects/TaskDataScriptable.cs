using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TaskData", fileName = "TaskData")]
public class TaskDataScriptable : ScriptableObject
{
    public string taskName;
    public int mandatorySlots;
    public int optionalSlots;
}
