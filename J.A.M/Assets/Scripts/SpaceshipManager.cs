using System;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipManager : MonoBehaviour
{
    public Room[] rooms;
    public ShipSystem[] shipSystems;
    public CharacterBehaviour[] characters;
    private Dictionary<System, ShipSystem> systemsDictionary = new Dictionary<System, ShipSystem>();
    private Dictionary<ShipRooms, Room> roomsDictionnary = new Dictionary<ShipRooms, Room>();
    [SerializeField] private TaskNotification taskNotificationPrefab;
    [SerializeField] private List<TaskNotification> activeTasks = new List<TaskNotification>();

    [Serializable]
    public struct Room
    {
        public ShipRooms roomName;
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
        [Range(0, 20)]
        public float gaugeValue;
        public TaskDataScriptable task;
    }

    public enum System
    {
        Power = 1,
        Airflow = 2,
        Food = 3,
        Hull = 4,
    }
    
    public enum ShipRooms
    {
        Electrical = 1,
        Airflow = 2,
        Food = 3,
        Hull = 4,
        Bedrooms = 5,
        Cafeteria = 6,
        Control = 7,
        Warehouse = 8,
    }

    private void InitializeSystems()
    {
        foreach (var system in shipSystems)
        {
            system.gaugeValue = 20;
            systemsDictionary.Add(system.systemName, system);
        }

        systemsDictionary[System.Hull].gaugeValue = 10;

        foreach (var room in rooms)
        {
            roomsDictionnary.Add(room.roomName, room);
        }
    }

    private void Initialize()
    {
        TimeTickSystem.OnTick += UpdateSystems;
        TimeTickSystem.OnTick += UpdateTasks;
        TimeTickSystem.OnTick += UpdateCharacters;
    }

    private void Update()
    {
        foreach (var activeTask in activeTasks)
        {
            var position = GetTaskPosition(activeTask.taskData.room).position;
            activeTask.transform.position = GameManager.Instance.mainCamera.WorldToScreenPoint(position);
        }
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
            if (system.gaugeValue < 0) system.gaugeValue = 0;
            else
                system.gaugeValue -= system.decreaseSpeed/TimeTickSystem.ticksPerHour;
            GameManager.Instance.UIManager.UpdateGauges(system.systemName, system.gaugeValue);
        }
        GameManager.Instance.UIManager.UpdateInGameDate(TimeTickSystem.GetTimeAsInGameDate(e));
    }

    public float GetGaugeValue(System system)
    {
        return systemsDictionary[system].gaugeValue;
    }

    public void GaugeValueOperation(System system, float value)
    {
        var gaugeValue = systemsDictionary[system].gaugeValue;
        gaugeValue += value;
        if (gaugeValue > 20)
        {
            gaugeValue = 20;
        }
        else if(gaugeValue < 0)
        {
            gaugeValue = 0;
        }

        systemsDictionary[system].gaugeValue = gaugeValue;
    }

    #region Tasks

    private void UpdateTasks(object sender, TimeTickSystem.OnTickEventArgs e)
    {
        foreach (var activeTask in activeTasks)
        {
            if (activeTask.isCompleted)
            {
                RemoveTask(activeTask);
                break;
            }

            activeTask.UpdateTask();
        }
    }

    public void SpawnTask(TaskDataScriptable taskDataScriptable, List<Tuple<Sprite, string, string>> dialoguesTask)
    {
        if (!IsTaskActive(taskDataScriptable))
        {
            var position = GetTaskPosition(taskDataScriptable.room).position;
            position = GameManager.Instance.mainCamera.WorldToScreenPoint(position);
            var taskNote = Instantiate(taskNotificationPrefab, position, Quaternion.identity, GameManager.Instance.UIManager.taskNotificationParent);
            taskNote.InitTask(taskDataScriptable, dialoguesTask);
            AddTask(taskNote);
        }
    }

    public void SpawnPermanentTask(TaskDataScriptable taskDataScriptable)
    {
        if (!IsTaskActive(taskDataScriptable))
        {
            var position = GetTaskPosition(taskDataScriptable.room).position;
            position = GameManager.Instance.mainCamera.WorldToScreenPoint(position);
            var taskNote = Instantiate(taskNotificationPrefab, position, Quaternion.identity, GameManager.Instance.UIManager.taskNotificationParent);
            taskNote.InitTask(taskDataScriptable);
            OpenTaskUI(taskNote);
            AddTask(taskNote);
        }
    }

    public Transform GetTaskPosition(ShipRooms room)
    {
        return roomsDictionnary[room].transform;
    }

    public void OpenTaskUI(TaskNotification tn)
    {
        GameManager.Instance.UIManager.SpawnTaskUI(tn);
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
        TaskNotification taskToRemove = null;
        foreach (var activeTask in activeTasks)
        {
            if (activeTask.taskData == task)
            {
                activeTask.CancelTask();
                taskToRemove = activeTask;
            }
        }
        if(taskToRemove != null) RemoveTask(taskToRemove);
    }

    public void RemoveTask(TaskNotification task)
    {
        activeTasks.Remove(task);
    }
    #endregion

    #region characters

    private void UpdateCharacters(object sender, TimeTickSystem.OnTickEventArgs e)
    {
        foreach(var character in characters)
        {
            if (!character.IsWorking())
            {
                float moodIncrease = 3.0f / TimeTickSystem.ticksPerHour;
                foreach (var system in shipSystems)
                {
                    if (system.gaugeValue <= 0)
                    {
                        moodIncrease -= 2.5f/TimeTickSystem.ticksPerHour;
                    }
                }
                character.IncreaseMood(moodIncrease);
            }
        }
        GameManager.Instance.UIManager.UpdateCharacterGauges();
    }

    #endregion
}
