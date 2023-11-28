using System;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipManager : MonoBehaviour
{
    public Room[] rooms;
    public ShipSystem[] shipSystems;
    public CharacterBehaviour[] characters;
    public Pool<GameObject> taskNotificationPool;

    [SerializeField] private List<TaskNotification> activeTasks = new();
    [SerializeField] private GameObject taskNotificationPrefab;
    
    private Dictionary<System, ShipSystem> systemsDictionary = new();
    private Dictionary<ShipRooms, Room> roomsDictionary = new();

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
        [Range(0, 20)] public float gaugeValue;
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

    private void Start()
    {
        Initialize();
        InitializeSystems();
        taskNotificationPool = new Pool<GameObject>(taskNotificationPrefab, 5);
    }

    private void Initialize()
    {
        TimeTickSystem.OnTick += UpdateSystems;
        TimeTickSystem.OnTick += UpdateTasks;
        TimeTickSystem.OnTick += UpdateCharacters;
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
            roomsDictionary.Add(room.roomName, room);
        }
    }

    private void UpdateSystems(object sender, TimeTickSystem.OnTickEventArgs e)
    {
        foreach (var system in shipSystems)
        {
            if (system.gaugeValue < 0) system.gaugeValue = 0;
            else
                system.gaugeValue -= system.decreaseSpeed / TimeTickSystem.ticksPerHour;
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
        else if (gaugeValue < 0)
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
            if (activeTask.IsCompleted)
            {
                RemoveTask(activeTask);
                break;
            }

            activeTask.OnUpdate();
        }
    }

    public void SpawnPermanentTask(TaskDataScriptable taskDataScriptable)
    {
        // if (!IsTaskActive(taskDataScriptable))
        // {
        //     var position = GetTaskPosition(taskDataScriptable.room).position;
        //     position = GameManager.Instance.mainCamera.WorldToScreenPoint(position);
        //     TaskNotification taskNote = Instantiate(taskNotificationPrefab, position, Quaternion.identity,
        //         GameManager.Instance.UIManager.taskNotificationParent);
        //     // taskNote.Initialize(taskDataScriptable);
        //     OpenTaskUI(taskNote);
        //     AddTask(taskNote);
        // }
    }

    /// <summary>
    /// Returns the position of the task in the room
    /// </summary>
    /// <param name="room"> Room of the task </param>
    /// <returns> Position of the room in the ship </returns>
    public Transform GetTaskPosition(ShipRooms room)
    {
        return roomsDictionary[room].transform;
    }

    public void OpenTaskUI(TaskNotification tn)
    {
        GameManager.Instance.UIManager.SpawnTaskUI(tn);
    }

    public void AddTask(TaskNotification task)
    {
        activeTasks.Add(task);
    }

    public bool IsTaskActive(Task task)
    {
        foreach (var activeTask in activeTasks)
        {
            return activeTask.Task == task;
        }

        return false;
    }

    public TaskNotification GetTaskNotification(Task task)
    {
        foreach (var activeTask in activeTasks)
        {
            if (activeTask.Task == task) return activeTask;
        }

        return null;
    }

    public void CancelTask(Task task)
    {
        TaskNotification taskToRemove = null;
        foreach (var activeTask in activeTasks)
        {
            if (activeTask.Task == task)
            {
                activeTask.OnCancel();
                taskToRemove = activeTask;
            }
        }

        if (taskToRemove != null) RemoveTask(taskToRemove);
    }

    public void RemoveTask(TaskNotification task)
    {
        activeTasks.Remove(task);
    }

    #endregion

    #region Characters

    private void UpdateCharacters(object sender, TimeTickSystem.OnTickEventArgs e)
    {
        foreach (var character in characters)
        {
            if (!character.IsWorking())
            {
                float moodIncrease = 3.0f / TimeTickSystem.ticksPerHour;
                foreach (var system in shipSystems)
                {
                    if (system.gaugeValue <= 0)
                    {
                        moodIncrease -= 2.5f / TimeTickSystem.ticksPerHour;
                    }
                }

                character.IncreaseMood(moodIncrease);
            }
        }

        GameManager.Instance.UIManager.UpdateCharacterGauges();
    }

    #endregion
}