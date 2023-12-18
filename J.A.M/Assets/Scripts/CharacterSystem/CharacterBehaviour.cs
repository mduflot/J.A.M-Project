using Tasks;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SubsystemsImplementation;
using UnityEngine.UIElements;

namespace CharacterSystem
{
    public class CharacterBehaviour : MonoBehaviour
    {
        public Speaker speaker;
    
        [SerializeField] private CharacterDataScriptable data;
        [SerializeField] private float moveSpeed;
    
        /*
         * gauge = 0 -> mood + param
         */
    
        private const float MaxMood = 20.0f;
    
        [Range(0,100)]
        private float mood = 50.0f;
    
        [Range(0,100)]
        private float volition = 10.0f;
    
        [SerializeField] private TraitsData.Traits traits;
    
        private bool isWorking;
        private bool isTaskLeader;
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
                    return (int) targetValue == (int) compValue; //Cheating to avoid floating point comparison error
                
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
        
        public TraitsData.Job GetJob() { return traits.GetJob(); }

        public TraitsData.PositiveTraits GetPositiveTraits() { return traits.GetPositiveTraits(); }

        public TraitsData.NegativeTraits GetNegativeTraits() { return traits.GetNegativeTraits(); }

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
            mood -= 3.0f;
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
            return mood;
        }

        public float GetMaxMood()
        {
            return MaxMood;
        }
    
        public float GetVolition()
        {
            return mood < volition ? mood : volition;
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
    }
}