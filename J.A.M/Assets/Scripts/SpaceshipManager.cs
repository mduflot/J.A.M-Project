using System;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipManager : MonoBehaviour
{
    public Room[] rooms;
    public ShipSystem[] shipSystems;
    [SerializeField] private float gaugesThreshold;
    public CharacterBehaviour[] characters;
    private Dictionary<System, ShipSystem> systemsDictionary = new Dictionary<System, ShipSystem>();
    [SerializeField] private TaskNotification taskNotificationPrefab;
    [SerializeField] private List<TaskNotification> activeTasks = new List<TaskNotification>();

    [Serializable]
    public struct Room
    {
        public string name;
        public Transform transform;
        public Transform doorPosition;
        public GameObject[] roomObjects;
    }

    [Serializable]
    public class ShipSystem
    {
        public System systemName;
        public GameObject systemObject;
        public float decreaseSpeed;
        [Range(0, 100)]
        public float gaugeValue;
        public TaskDataScriptable task;
    }

    public enum System
    {
        Power = 1,
        Oxygen = 2,
        Food = 3,
    }
    private void InitializeSystems()
    {
        foreach (var system in shipSystems)
        {
            system.gaugeValue = 100;
            systemsDictionary.Add(system.systemName, system);
        }
    }

    private void Initialize()
    {
        TimeTickSystem.OnTick += UpdateSystems;
    }

    private void Start()
    {
        Initialize();
        InitializeSystems();
    }

    private void UpdateSystems(object sender, TimeTickSystem.OnTickEventArgs e)
    {
        foreach (var system in shipSystems)
        {
            system.gaugeValue -= system.decreaseSpeed;
            GameManager.Instance.UIManager.UpdateGauges(system.systemName, system.gaugeValue);
        }
    }

    public float GetGaugeValue(System system)
    {
        return systemsDictionary[system].gaugeValue;
    }

    public void GaugeValueOperation(System system, float value)
    {
        var gaugeValue = systemsDictionary[system].gaugeValue;
        gaugeValue += value;
        if (gaugeValue > 100)
        {
            gaugeValue = 100;
        }
        else if(gaugeValue < 0)
        {
            gaugeValue = 0;
        }

        systemsDictionary[system].gaugeValue = gaugeValue;
    }

    public Transform GetTaskPosition(System system)
    {
        return systemsDictionary[system].systemObject.transform;
    }
    
    public void SpawnTask(TaskDataScriptable taskDataScriptable)
    {
        if (!IsTaskActive(taskDataScriptable))
        {
            var position = systemsDictionary[taskDataScriptable.system].systemObject.transform.position;
            Debug.Log(systemsDictionary[taskDataScriptable.system].systemObject);
            var taskNote = Instantiate(taskNotificationPrefab, position, Quaternion.identity, GameManager.Instance.UIManager.taskNotificationParent);
            taskNote.InitTask(taskDataScriptable);
            AddTask(taskNote);
        }
    }

    public void OpenTaskUI(TaskDataScriptable taskDataScriptable, TaskNotification tn)
    {
        GameManager.Instance.UIManager.SpawnTaskUI(taskDataScriptable, tn);
    }

    public void AddTask(TaskNotification task)
    {
        activeTasks.Add(task);
    }

    public bool IsTaskActive(TaskDataScriptable task)
    {
        foreach (var activeTask in activeTasks)
        {
            return activeTask.taskData == task;
        }

        return false;
    }

    public TaskNotification GetTaskNotification(TaskDataScriptable task)
    {
        foreach (var activeTask in activeTasks)
        {
            if(activeTask.taskData == task) return activeTask;
        }

        return null;
    }

    public void CancelTask(TaskDataScriptable task)
    {
        foreach (var activeTask in activeTasks)
        {
            if (activeTask.taskData == task)
            {
                activeTask.CancelTask();
            }
        }
    }
}
