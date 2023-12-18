using System.Collections.Generic;
using CharacterSystem;
using Tasks;
using UI;
using UnityEngine;

namespace Managers
{
    public class SpaceshipManager : MonoBehaviour
    {
        public Room[] rooms;
        public ShipSystem[] systems;
        public CharacterBehaviour[] characters;
        public Pool<GameObject> notificationPool;
        public TraitsData.SpaceshipTraits SpaceshipTraits = TraitsData.SpaceshipTraits.None;
        public TraitsData.HiddenSpaceshipTraits HiddenSpaceshipTraits = TraitsData.HiddenSpaceshipTraits.None;

        [SerializeField] private Checker checker;
        [SerializeField] private List<Notification> activeTasks = new();
        [SerializeField] private GameObject taskNotificationPrefab;
    
        private Dictionary<SystemType, ShipSystem> systemsDictionary = new();
        private Dictionary<RoomType, Room> roomsDictionary = new();

        private void Start()
        {
            Initialize();
            InitializeSystems();
            checker.Initialize();
            notificationPool = new Pool<GameObject>(taskNotificationPrefab, 5);
        }

        private void Initialize()
        {
            TimeTickSystem.OnTick += UpdateSystems;
            TimeTickSystem.OnTick += UpdateTasks;
            TimeTickSystem.OnTick += UpdateCharacters;
            TimeTickSystem.OnTick += GenerateRandomEventOnDayStart;
        }

        private void InitializeSystems()
        {
            foreach (var system in systems)
            {
                system.gaugeValue = 20;
                systemsDictionary.Add(system.type, system);
            }

            systemsDictionary[SystemType.Hull].gaugeValue = 10;

            foreach (var room in rooms)
            {
                roomsDictionary.Add(room.type, room);
            }
        }

        private void UpdateSystems(object sender, TimeTickSystem.OnTickEventArgs e)
        {
            foreach (var system in systems)
            {
                if (system.gaugeValue < 0) system.gaugeValue = 0;
                else
                    system.gaugeValue -= system.decreaseSpeed / TimeTickSystem.ticksPerHour;
                GameManager.Instance.UIManager.UpdateGauges(system.type, system.gaugeValue);
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
            var maxGauge = systemsDictionary[systemType].maxGauge;
            gaugeValue += value;
            if (gaugeValue > maxGauge)
            {
                gaugeValue = maxGauge;
            }
            else if (gaugeValue < 0)
            {
                gaugeValue = 0;
            }

            systemsDictionary[systemType].gaugeValue = gaugeValue;
        }

        public void GenerateRandomEventOnDayStart(object sender, TimeTickSystem.OnTickEventArgs e)
        {
            if (e.tick % (TimeTickSystem.ticksPerHour * 24) != 0)
                return;
        
            Debug.Log("Generating random event");
            //Checker.GenerateRandomEvent();
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

        public Room GetRoom(RoomType room)
        {
            return roomsDictionary[room];
        }

        public void AddTask(Notification task)
        {
            activeTasks.Add(task);
        }

        public bool IsTaskActive(string taskName)
        {
            for (var index = 0; index < activeTasks.Count; index++)
            {
                var activeTask = activeTasks[index];
                if (activeTask.Task.Name == taskName) return true;
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
                    float moodIncrease = 1.0f / TimeTickSystem.ticksPerHour;
                    foreach (var system in systems)
                    {
                        if (system.gaugeValue <= 0)
                        {
                            moodIncrease -= 0.75f / TimeTickSystem.ticksPerHour;
                        }
                    }

                    character.IncreaseMood(moodIncrease);
                }
            }

            GameManager.Instance.UIManager.UpdateCharacterGauges();
        }

        #endregion
    }
}