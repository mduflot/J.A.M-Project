using System;
using System.Collections.Generic;
using Managers;
using Tasks;
using UI;
using UnityEngine;

namespace CharacterSystem
{
    public class CharacterBehaviour : MonoBehaviour, IDataPersistence
    {
        public Speaker speaker;

        [SerializeField] private CharacterDataScriptable data;
        [SerializeField] private float moveSpeed;

        /*
         * gauge = 0 -> mood + param
         */

        private const float MaxMood = 20.0f;

        [Range(0, 100)] private float mood = 50.0f;

        [Range(0, 100)] private float volition = 10.0f;

        [SerializeField] private TraitsData.Traits traits;

        private bool isWorking;
        private bool isTaskLeader;

        private bool isMoodIncreasing;
        
        private Notification currentNotification;

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            mood = data.baseMood;
            volition = data.baseVolition;
            traits = data.traits;
        }

        public bool CheckStat(float threshold, OutcomeData.OutcomeTargetStat ts, ConditionSystem.ComparisonOperator co)
        {
            var targetValue = (ts == OutcomeData.OutcomeTargetStat.Mood) ? mood : volition;
            var compValue = (threshold > MaxMood) ? MaxMood : threshold;
            switch (co)
            {
                case ConditionSystem.ComparisonOperator.Equal:
                    return (int)targetValue == (int)compValue; //Cheating to avoid floating point comparison error

                case ConditionSystem.ComparisonOperator.Higher:
                    return targetValue > compValue;

                case ConditionSystem.ComparisonOperator.Less:
                    return targetValue < compValue;

                case ConditionSystem.ComparisonOperator.HigherOrEqual:
                    return targetValue >= compValue;

                case ConditionSystem.ComparisonOperator.LessOrEqual:
                    return targetValue <= compValue;
            }

            return true;
        }

        public TraitsData.Job GetJob()
        {
            return traits.GetJob();
        }

        public TraitsData.PositiveTraits GetPositiveTraits()
        {
            return traits.GetPositiveTraits();
        }

        public TraitsData.NegativeTraits GetNegativeTraits()
        {
            return traits.GetNegativeTraits();
        }

        public void AddTrait(TraitsData.Traits argTraits)
        {
            traits.AddTraits(argTraits);
        }

        public void SubTrait(TraitsData.Traits argTraits)
        {
            traits.RemoveTraits(argTraits);
        }

        public void IncreaseMood(float value)
        {
            isMoodIncreasing = value > 0;
            mood += value;
            CapStats();
        }

        public void IncreaseVolition(float value)
        {
            volition += value;
        }

        private void CapStats()
        {
            mood = Mathf.Clamp(mood, 0, MaxMood);
        }

        public void MoveTo(Transform destination)
        {
            float timeToTravel = Vector2.Distance(transform.position, destination.position) * moveSpeed;
            //Ajouter une étape avec la porte de la pièce active
        }

        public void AssignTask(Notification t, bool leader = false)
        {
            isWorking = true;
            currentNotification = t;
            isTaskLeader = leader;
            mood -= GameManager.Instance.SpaceshipManager.moodLossOnTaskStart;
        }

        public bool IsWorking()
        {
            return isWorking;
        }

        public bool IsTaskLeader()
        {
            return isTaskLeader;
        }

        public Task GetTask()
        {
            return currentNotification.Task;
        }

        public CharacterDataScriptable GetCharacterData()
        {
            return data;
        }

        public float GetMood()
        {
            float finalMood = mood;
            return finalMood;
        }

        public float GetMaxMood()
        {
            float finalMaxMood = MaxMood;
            finalMaxMood -=
                GameManager.Instance.SpaceshipManager.SpaceshipTraits.HasFlag(TraitsData.SpaceshipTraits
                    .ObstructedVentilation)
                    ? 4
                    : 0;
            finalMaxMood -=
                GameManager.Instance.SpaceshipManager.SpaceshipTraits.HasFlag(TraitsData.SpaceshipTraits
                    .Malfunction)
                    ? 4
                    : 0;
            return finalMaxMood;
        }

        public float GetVolition()
        {
            float finalVolition = volition;
            finalVolition -= traits.GetNegativeTraits().HasFlag(TraitsData.NegativeTraits.Crippled) ? 3 : 0;
            finalVolition -= traits.GetNegativeTraits().HasFlag(TraitsData.NegativeTraits.Scarred) ? 1 : 0;
            return mood < finalVolition ? mood : finalVolition;
        }

        public float GetBaseVolition()
        {
            return volition;
        }

        public TraitsData.Traits GetTraits()
        {
            return traits;
        }

        public void StopTask()
        {
            isWorking = false;
            currentNotification = null;
        }

        public bool IsMoodIncreasing()
        {
            return isMoodIncreasing;
        }

        public void LoadData(GameData gameData)
        {
            if (gameData.characterTraits.TryGetValue(data.ID, out var t))
            {
                traits = t;
            }
            if (gameData.characterMoods.TryGetValue(data.ID, out var m))
            {
                mood = m;
            }
            if (gameData.characterVolitions.TryGetValue(data.ID, out var v))
            {
                volition = v;
            }
        }

        public void SaveData(ref GameData gameData)
        {
            if (!gameData.characterTraits.TryAdd(data.ID, traits))
            {
                gameData.characterTraits.Remove(data.ID);
                gameData.characterTraits.Add(data.ID, traits);
            }
            if (!gameData.characterMoods.TryAdd(data.ID, mood))
            {
                gameData.characterMoods.Remove(data.ID);
                gameData.characterMoods.Add(data.ID, mood);return;
            }
            if (!gameData.characterVolitions.TryAdd(data.ID, volition))
            {
                gameData.characterVolitions.Remove(data.ID);
                gameData.characterVolitions.Add(data.ID, volition);
            }
        }
    }
}