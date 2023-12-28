using System.Collections.Generic;
using CharacterSystem;
using Tasks;
using UI;
using UnityEngine;

namespace Managers
{
    public class SpaceshipManager : MonoBehaviour, IDataPersistence
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

        [Header("Character Value")] public float moodLossOnTaskStart = 7.0f;

        [SerializeField] private float hourlyMoodGain = 1.0f;

        [SerializeField] private float hourlyMoodLossGaugeEmpty = 0.75f;
        public float moodLossOnCancelTask = 10.0f;

        private Dictionary<SystemType, ShipSystem> systemsDictionary = new();
        private Dictionary<RoomType, Room> roomsDictionary = new();

        private void Start()
        {
            Initialize();
            InitializeSystems();
            notificationPool = new Pool<GameObject>(taskNotificationPrefab, 5);
            DataPersistenceManager.Instance.InitializeGame();
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
                system.gaugeValue = 35;
                systemsDictionary.Add(system.type, system);
            }

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
                GameManager.Instance.UIManager.UpdateGauges(system.type, system.gaugeValue, system.previewGaugeValue);
            }

            GameManager.Instance.UIManager.UpdateInGameDate(TimeTickSystem.GetTimeAsInGameDate(e));
        }

        public void ApplyGaugeOutcomes(List<TaskUI.GaugesOutcome> outcomes)
        {
            foreach (var system in systems)
            {
                var valueToAdd = 0f;
                foreach (var outcome in outcomes)
                {
                    if (outcome.gauge == system.type) valueToAdd += outcome.value;
                }

                system.previewGaugeValue += valueToAdd;
            }
        }

        public void RemoveGaugeOutcomes(List<TaskUI.GaugesOutcome> outcomes)
        {
            foreach (var system in systems)
            {
                var valueToAdd = 0f;
                foreach (var outcome in outcomes)
                {
                    if (outcome.gauge == system.type) valueToAdd -= outcome.value;
                }

                system.previewGaugeValue += valueToAdd;
            }
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
            checker.GenerateRandomEvent();
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
                    float moodIncrease = hourlyMoodGain / TimeTickSystem.ticksPerHour;
                    foreach (var system in systems)
                    {
                        if (system.gaugeValue <= 0)
                        {
                            moodIncrease -= hourlyMoodLossGaugeEmpty / TimeTickSystem.ticksPerHour;
                        }
                    }

                    character.IncreaseMood(moodIncrease);
                }
            }

            GameManager.Instance.UIManager.UpdateCharacterGauges();
        }

        #endregion

        #region Save&Load

        public void LoadData(GameData gameData)
        {
            foreach (var system in systems)
            {
                if (gameData.gaugeValues.TryGetValue(system.type, out float value))
                {
                    system.gaugeValue = value;
                }
            }

            this.SpaceshipTraits = gameData.spaceshipTraits;
            this.HiddenSpaceshipTraits = gameData.hiddenSpaceshipTraits;
        }

        public void SaveData(ref GameData gameData)
        {
            foreach (var system in systems)
            {
                if (gameData.gaugeValues.TryAdd(system.type, system.gaugeValue)) continue;
                gameData.gaugeValues.Remove(system.type);
                gameData.gaugeValues.Add(system.type, system.gaugeValue);
            }

            gameData.spaceshipTraits = this.SpaceshipTraits;
            gameData.hiddenSpaceshipTraits = this.HiddenSpaceshipTraits;
        }

        #endregion
    }
}