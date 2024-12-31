using System.Collections.Generic;
using System.Linq;
using CharacterSystem;
using SS.Enumerations;
using Tasks;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

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
        [SerializeField] private GameObject roomsDisplay;
        [SerializeField] private GameObject shipOutside;

        [SerializeField] private List<Notification> activeTasks = new();
        [SerializeField] private GameObject taskNotificationPrefab;

        [Header("Character Value")] public float moodLossOnTaskStart = 7.0f;

        [SerializeField] private float hourlyMoodGain = 1.0f;

        [SerializeField] private float hourlyMoodLossGaugeEmpty = 0.75f;
        public float moodLossOnCancelTask = 10.0f;

        [Header("Sim Values")] public float simMoveSpeed = .9f;
        public int simHungerBaseThreshold = 12;
        public int simHungerNoise = 6;
        [SerializeField] private int simEatThreshold = 3;
        [SerializeField] private float simEatAmount;

        private Dictionary<SystemType, ShipSystem> systemsDictionary = new();
        private Dictionary<RoomType, Room> roomsDictionary = new();

        public bool IsInTutorial;

        [Header("Leak")] [SerializeField] private uint baseTimeToWaitLeak = 12;
        private uint timeToWaitLeak;
        private uint waitingTimeLeak;
        private bool isLeaked;

        [Header("Traits description")] public SerializableDictionary<TraitsData.Job, string> jobDescription;
        public SerializableDictionary<TraitsData.PositiveTraits, string> positiveTraitsDescription;
        public SerializableDictionary<TraitsData.NegativeTraits, string> negativeTraitsDescription;
        public SerializableDictionary<TraitsData.SpaceshipTraits, string> spaceshipTraitsDescription;

        private void Start()
        {
            if (DataPersistenceManager.Instance.IsNewGame) IsInTutorial = true;
            Initialize();
            InitializeSystems();
            notificationPool = new Pool<GameObject>(taskNotificationPrefab, 5);
            DataPersistenceManager.Instance.InitializeGame();
        }

        private void OnDisable()
        {
            TimeTickSystem.OnTick -= UpdateSystems;
            TimeTickSystem.OnTick -= UpdateTasks;
            TimeTickSystem.OnTick -= UpdateCharacters;
            TimeTickSystem.OnTick -= GenerateSecondaryEventOnFirstDay;
            activeTasks.Clear();
        }

        private void Initialize()
        {
            timeToWaitLeak = baseTimeToWaitLeak * TimeTickSystem.ticksPerHour;
            TimeTickSystem.OnTick += UpdateSystems;
            TimeTickSystem.OnTick += UpdateTasks;
            TimeTickSystem.OnTick += UpdateCharacters;
            TimeTickSystem.OnTick += GenerateSecondaryEventOnFirstDay;
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

        private void GenerateLeakEvent()
        {
            isLeaked = true;
            Checker.Instance.ChooseNewStoryline(SSStoryType.Spontaneous, SSSpontaneousType.Leak);
            TimeTickSystem.OnTick += WaitingLeakEvent;
        }

        private void WaitingLeakEvent(object sender, TimeTickSystem.OnTickEventArgs e)
        {
            if (waitingTimeLeak < timeToWaitLeak)
            {
                waitingTimeLeak += TimeTickSystem.timePerTick;
                return;
            }

            isLeaked = false;
            waitingTimeLeak = 0;
            TimeTickSystem.OnTick -= WaitingLeakEvent;
        }

        private void UpdateSystems(object sender, TimeTickSystem.OnTickEventArgs e)
        {
            if (systems.All(system => system.gaugeValue <= 0))
            {
                TimeTickSystem.ModifyTimeScale(0);
                GameManager.Instance.UIManager.PopupEndGame.InitializeEndGame(
                    "You might be forgiven, it was a long way to earth...", "Defeat !");
                TimeTickSystem.OnTick -= UpdateSystems;
                TimeTickSystem.OnTick -= UpdateTasks;
                TimeTickSystem.OnTick -= UpdateCharacters;
                TimeTickSystem.OnTick -= GenerateSecondaryEventOnFirstDay;
                return;
            }

            foreach (var system in systems)
            {
                if (system.type == SystemType.Hull)
                {
                    if (!isLeaked)
                    {
                        if (system.gaugeValue < 10)
                        {
                            GenerateLeakEvent();
                        }
                    }
                }

                if (system.gaugeValue <= 0) system.gaugeValue = 0.0f;
                else {
                    float decreaseValue = 0.0f;
                    for (int index = 0; index < system.decreaseValues.Count; index++) {
                        decreaseValue += system.decreaseValues[index];
                    }
                    system.gaugeValue -= decreaseValue / TimeTickSystem.ticksPerHour;
                }

                if (!IsInTutorial)
                    GameManager.Instance.UIManager.UpdateGauges(system.type, system.gaugeValue,
                        system.previewGaugeValue);
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

                if (system.type == SystemType.Trajectory) system.previewGaugeValue -= valueToAdd;
                else system.previewGaugeValue += valueToAdd;
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

            if (systemType == SystemType.Trajectory) systemsDictionary[systemType].previewGaugeValue += value;
            else systemsDictionary[systemType].previewGaugeValue -= value;
            systemsDictionary[systemType].gaugeValue = gaugeValue;
        }

        private void GenerateSecondaryEventOnFirstDay(object sender, TimeTickSystem.OnTickEventArgs e)
        {
            if (e.tick % (TimeTickSystem.ticksPerHour * 24) != 0) return;
            if (IsInTutorial) return;

            Checker.Instance.ChooseNewStoryline(SSStoryType.Spontaneous);
            TimeTickSystem.OnTick -= GenerateSecondaryEventOnFirstDay;
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
            //CharacterBehaviour
            foreach (var character in characters)
            {
                SimCharacter simCharacter = character.GetSimCharacter();
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

                //SimCharacter
                if (character.GetSimCharacter() != null)
                {
                    simCharacter.tick++;
                    switch (simCharacter.simStatus)
                    {
                        case SimCharacter.SimStatus.IdleEat:
                            if (simCharacter.tick >= simEatThreshold * TimeTickSystem.ticksPerHour)
                            {
                                if (character.IsWorking() && simCharacter.taskRoom != null)
                                {
                                    simCharacter.SendToRoom(simCharacter.taskRoom.roomType);
                                }
                                else
                                    simCharacter.SendToIdleRoom();

                                if (!IsInTutorial)
                                {
                                    float eatAmount = simEatAmount;
                                    eatAmount += SpaceshipTraits.HasFlag(TraitsData.SpaceshipTraits.Rot)
                                        ? (simEatAmount * 3f) / 10f
                                        : 0f;
                                    eatAmount += SpaceshipTraits.HasFlag(TraitsData.SpaceshipTraits.DamagedRations)
                                        ? (simEatAmount * 3f) / 10f
                                        : 0f;
                                    eatAmount -= SpaceshipTraits.HasFlag(TraitsData.SpaceshipTraits.Restriction)
                                        ? (simEatAmount) / 10f
                                        : 0f;
                                    eatAmount -= SpaceshipTraits.HasFlag(TraitsData.SpaceshipTraits.Fertilization)
                                        ? (simEatAmount) / 10f
                                        : 0f;

                                    systems[2].gaugeValue -= eatAmount;
                                }

                                simCharacter.tick = 0;
                                simCharacter.ticksToEat = (simHungerBaseThreshold
                                                           * (int) TimeTickSystem.ticksPerHour)
                                                          + Random.Range(
                                                              (int) -TimeTickSystem.ticksPerHour * simHungerNoise,
                                                              (int) TimeTickSystem.ticksPerHour * simHungerNoise);
                            }

                            break;

                        case SimCharacter.SimStatus.GoToEat:
                            simCharacter.tick = 0;
                            break;

                        case SimCharacter.SimStatus.GoToIdle:
                            simCharacter.tick--;
                            break;

                        case SimCharacter.SimStatus.Idle:
                            if (simCharacter.tick >= simCharacter.ticksToEat)
                            {
                                simCharacter.SendToRoom(RoomType.Kitchen);
                                simCharacter.simStatus = SimCharacter.SimStatus.GoToEat;
                                simCharacter.ticksToNextIdle = (uint) Random.Range(1, 7) * TimeTickSystem.ticksPerHour;
                                simCharacter.tick = 0;
                                break;
                            }

                            if (!character.IsWorking() && simCharacter.tick >= simCharacter.ticksToNextIdle)
                            {
                                simCharacter.ticksToNextIdle = (uint) Random.Range(1, 7) * TimeTickSystem.ticksPerHour +
                                                               (uint) simCharacter.tick;
                                simCharacter.SendToIdleRoom();
                            }

                            break;
                    }
                }
            }

            GameManager.Instance.UIManager.UpdateCharacterGauges();
        }

        #endregion

        public void DisplayRooms(bool state)
        {
            roomsDisplay.SetActive(state);
            if (Camera.main.transform.position.z < -2000)
            {
                DisplayShipOutside(!state);
            }
        }

        public void DisplayShipOutside(bool state)
        {
            shipOutside.SetActive(state);
        }

        #region Save&Load

        public void LoadData(GameData gameData)
        {
            IsInTutorial = gameData.isInTutorial;

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
            gameData.isInTutorial = this.IsInTutorial;

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