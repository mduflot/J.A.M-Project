using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SpaceshipManager : MonoBehaviour
{
    public Furniture[] rooms;
    public System[] shipSystems;
    public CharacterBehaviour[] characters;
    public Pool<GameObject> notificationPool;

    [SerializeField] private List<Notification> activeTasks = new();
    [SerializeField] private GameObject taskNotificationPrefab;

    private Dictionary<SystemType, System> systemsDictionary = new();
    private Dictionary<FurnitureType, Furniture> roomsDictionary = new();

    [Serializable]
    public struct Furniture
    {
        [FormerlySerializedAs("roomName")] public FurnitureType name;
        public Transform transform;
        public Transform doorPosition;
        public GameObject[] roomObjects;
    }

    [Serializable]
    public class System
    {
        [FormerlySerializedAs("systemName")] public SystemType systemTypeName;
        public GameObject systemObject;
        public float decreaseSpeed;
        [Range(0, 20)] public float gaugeValue;
        public TaskDataScriptable task;
    }

    public enum SystemType
    {
        Power = 1,
        Airflow = 2,
        Food = 3,
        Hull = 4,
    }

    public enum RoomType
    {
        Electrical = 1,
        Airflow = 2,
        Food = 3,
        Hull = 4,
        Bedrooms = 5,
        Cafeteria = 6,
        Control = 7,
        Warehouse = 8
    }

    public enum FurnitureType
    {
        Electrical = 1,
        Airflow = 2,
        Food = 3,
        Hull = 4,
        Bedrooms = 5,
        Cafeteria = 6,
        Control = 7,
        Warehouse = 8
    }

    private void Start()
    {
        Initialize();
        InitializeSystems();
        notificationPool = new Pool<GameObject>(taskNotificationPrefab, 5);
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
            systemsDictionary.Add(system.systemTypeName, system);
        }

        systemsDictionary[SystemType.Hull].gaugeValue = 10;

        foreach (var room in rooms)
        {
            roomsDictionary.Add(room.name, room);
        }
    }

    private void UpdateSystems(object sender, TimeTickSystem.OnTickEventArgs e)
    {
        foreach (var system in shipSystems)
        {
            if (system.gaugeValue < 0) system.gaugeValue = 0;
            else
                system.gaugeValue -= system.decreaseSpeed / TimeTickSystem.ticksPerHour;
            GameManager.Instance.UIManager.UpdateGauges(system.systemTypeName, system.gaugeValue);
        }

        GameManager.Instance.UIManager.UpdateInGameDate(TimeTickSystem.GetTimeAsInGameDate(e));
    }

    public float GetGaugeValue(SystemType systemType)
    {
        return systemsDictionary[systemType].gaugeValue;
    }

    public void GaugeValueOperation(SystemType systemType, float value)
    {
        var gaugeValue = systemsDictionary[systemType].gaugeValue;
        gaugeValue += value;
        if (gaugeValue > 20)
        {
            gaugeValue = 20;
        }
        else if (gaugeValue < 0)
        {
            gaugeValue = 0;
        }

        systemsDictionary[systemType].gaugeValue = gaugeValue;
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

    /// <summary>
    /// Returns the position of the task in the room
    /// </summary>
    /// <param name="room"> Room of the task </param>
    /// <returns> Position of the room in the ship </returns>
    public Transform GetTaskPosition(FurnitureType room)
    {
        return roomsDictionary[room].transform;
    }

    public void AddTask(Notification task)
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

    public Notification GetTaskNotification(Task task)
    {
        foreach (var activeTask in activeTasks)
        {
            if (activeTask.Task == task) return activeTask;
        }

        return null;
    }

    public void CancelTask(Task task)
    {
        Notification toRemove = null;
        foreach (var activeTask in activeTasks)
        {
            if (activeTask.Task == task)
            {
                activeTask.OnCancel();
                toRemove = activeTask;
            }
        }

        if (toRemove != null) RemoveTask(toRemove);
    }

    public void RemoveTask(Notification task)
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