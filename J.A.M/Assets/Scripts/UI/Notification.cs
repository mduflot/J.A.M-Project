using System;
using System.Collections.Generic;
using SS.Enumerations;
using CharacterSystem;
using Managers;
using Tasks;
using TMPro;
using UnityEngine;

namespace UI
{
    public class Notification : MonoBehaviour
    {
        [HideInInspector] public bool IsCompleted;
        [HideInInspector] public bool IsStarted;
        [HideInInspector] public List<Tuple<Sprite, string, string>> Dialogues;
        [HideInInspector] public Task Task;
        [HideInInspector] public List<CharacterBehaviour> LeaderCharacters = new();
        [HideInInspector] public List<CharacterBehaviour> AssistantCharacters = new();

        [SerializeField] private SpriteRenderer icon;
        [SerializeField] private TextMeshPro time;

        private Camera camera;
        private SpaceshipManager spaceshipManager;
        private ConditionSO taskCondition;
        private OutcomeSystem.OutcomeEvent[] outcomeEvents;
        private OutcomeSystem.OutcomeEventArgs[] outcomeEventArgs;

        private void Start()
        {
            camera = Camera.main;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform == transform)
                    {
                        Display();
                    }
                }
            }
        }

        public void Initialize(Task task, SpaceshipManager spaceshipManager,
            List<Tuple<Sprite, string, string>> dialogues = null)
        {
            Task = task;
            Task.TimeLeft *= TimeTickSystem.ticksPerHour;
            icon.sprite = task.Icon;
            Dialogues = dialogues;
            this.spaceshipManager = spaceshipManager;
            TimeTickSystem.ModifyTimeScale(TimeTickSystem.lastActiveTimeScale);
        }

        public void Display()
        {
            TimeTickSystem.ModifyTimeScale(0.0f);
            if (IsStarted)
            {
                GameManager.Instance.UIManager.taskUI.DisplayTaskInfo(this);
            }
            else
            {
                GameManager.Instance.UIManager.taskUI.Initialize(this);
            }
        }

        public void OnStart(List<CharacterUISlot> characters)
        {
            TimeTickSystem.ModifyTimeScale(TimeTickSystem.lastActiveTimeScale);
            foreach (var character in characters)
            {
                if (character.isMandatory)
                {
                    if (character.icon != null)
                    {
                        Task.leaderCharacters.Add(character.icon.character);
                        LeaderCharacters.Add(character.icon.character);
                        character.icon.character.AssignTask(this, true);
                    }
                }
                else
                {
                    if (character.icon != null)
                    {
                        Task.assistantCharacters.Add(character.icon.character);
                        AssistantCharacters.Add(character.icon.character);
                        character.icon.character.AssignTask(this);
                    }
                }
            }
            
            //checkCondition & reference
            var validatedCondition = false;

            for (int i = 0; i < Task.Conditions.Count; i++)
            {
                taskCondition = Task.Conditions[i].Item1;
                validatedCondition = RouteCondition(taskCondition.target);
                if (validatedCondition)
                {
                    Task.conditionIndex = i;
                    break;
                }
            }

            if (LeaderCharacters.Count == 0)
            {
                Debug.Log("No leader assigned to task");
                taskCondition = Task.Conditions[^1].Item1;
                Task.conditionIndex = Task.Conditions.Count - 1;
                validatedCondition = true;
            }
            
            CheckingCondition(validatedCondition);
        }

        private void CheckingCondition(bool validatedCondition)
        {
            if (validatedCondition)
            {
                //Check additional conditions
                var additionalConditionOutcomes = new List<Outcome>();
                for (uint i = 0; i < taskCondition.additionnalConditions.Length; i++)
                {
                    var cond = taskCondition.additionnalConditions[i];
                    switch (cond.target)
                    {
                        case OutcomeData.OutcomeTarget.Leader:
                            if (!ConditionSystem.CheckCharacterCondition(LeaderCharacters[0].GetTraits(), cond))
                                continue;
                            break;

                        case OutcomeData.OutcomeTarget.Assistant:
                            bool condition = false;
                            for (int j = 0; j < AssistantCharacters.Count; j++)
                            {
                                if (AssistantCharacters[j] == null)
                                    if (!ConditionSystem.CheckCharacterCondition(AssistantCharacters[j].GetTraits(),
                                            cond))
                                        condition = true;
                                    else
                                    {
                                        condition = false;
                                        break;
                                    }
                            }
                            if (condition)
                                continue;
                            break;

                        case OutcomeData.OutcomeTarget.Crew:
                            if (!ConditionSystem.CheckCrewCondition(cond))
                                continue;
                            break;

                        case OutcomeData.OutcomeTarget.Ship:
                            if (!ConditionSystem.CheckSpaceshipCondition(cond))
                                continue;
                            break;

                        case OutcomeData.OutcomeTarget.Gauge:
                            if (!ConditionSystem.CheckGaugeCondition(cond))
                                continue;
                            break;

                        case OutcomeData.OutcomeTarget.None:
                            break;
                    }

                    additionalConditionOutcomes.AddRange(cond.outcomes.Outcomes);
                }

                //Generate event args
                outcomeEventArgs =
                    new OutcomeSystem.OutcomeEventArgs[taskCondition.outcomes.Outcomes.Length +
                                                       additionalConditionOutcomes.Count];
                
                Debug.Log(outcomeEventArgs.Length);
                
                for (int i = 0; i < taskCondition.outcomes.Outcomes.Length; i++)
                {
                    //Generate event args
                    var outcome = taskCondition.outcomes.Outcomes[i];
                    switch (outcome.OutcomeTarget)
                    {
                        case OutcomeData.OutcomeTarget.Leader:
                            outcomeEventArgs[i] = OutcomeSystem.GenerateEventArgs(outcome, LeaderCharacters[0]);
                            break;

                        case OutcomeData.OutcomeTarget.Assistant:
                            outcomeEventArgs[i] = OutcomeSystem.GenerateEventArgs(outcome, AssistantCharacters[0]);
                            break;

                        case OutcomeData.OutcomeTarget.Crew:
                            outcomeEventArgs[i] = OutcomeSystem.GenerateEventArgs(outcome,
                                GameManager.Instance.SpaceshipManager.characters);
                            break;

                        case OutcomeData.OutcomeTarget.Gauge:
                            outcomeEventArgs[i] = OutcomeSystem.GenerateEventArgs(outcome, outcome.OutcomeTargetGauge);
                            break;
                    }
                }

                var numberOfBaseOutcomes = taskCondition.outcomes.Outcomes.Length;
                for (int i = 0; i < additionalConditionOutcomes.Count; i++)
                {
                    var outcome = additionalConditionOutcomes[i];
                    switch (outcome.OutcomeTarget)
                    {
                        case OutcomeData.OutcomeTarget.Leader:
                            outcomeEventArgs[numberOfBaseOutcomes + i] =
                                OutcomeSystem.GenerateEventArgs(outcome, LeaderCharacters[0]);
                            break;

                        case OutcomeData.OutcomeTarget.Assistant:
                            outcomeEventArgs[numberOfBaseOutcomes + i] =
                                OutcomeSystem.GenerateEventArgs(outcome, AssistantCharacters[0]);
                            break;

                        case OutcomeData.OutcomeTarget.Crew:
                            outcomeEventArgs[numberOfBaseOutcomes + i] = OutcomeSystem.GenerateEventArgs(outcome,
                                GameManager.Instance.SpaceshipManager.characters);
                            break;

                        case OutcomeData.OutcomeTarget.Gauge:
                            outcomeEventArgs[numberOfBaseOutcomes + i] =
                                OutcomeSystem.GenerateEventArgs(outcome, outcome.OutcomeTargetGauge);
                            break;
                    }
                }

                //Generate events
                outcomeEvents = new OutcomeSystem.OutcomeEvent[outcomeEventArgs.Length];
                for (int i = 0; i < outcomeEventArgs.Length; i++)
                {
                    outcomeEvents[i] = OutcomeSystem.GenerateOutcomeEvent(outcomeEventArgs[i]);
                }

                Task.Duration = AssistantCharacters.Count > 0
                    ? Task.Duration / Mathf.Pow(AssistantCharacters.Count + LeaderCharacters.Count, Task.HelpFactor)
                    : Task.Duration;
                Task.Duration *= TimeTickSystem.ticksPerHour;
                Task.BaseDuration = Task.Duration;
                IsStarted = true;
            }
            else
            {
                outcomeEventArgs = Array.Empty<OutcomeSystem.OutcomeEventArgs>();
                outcomeEvents = Array.Empty<OutcomeSystem.OutcomeEvent>();
            }
        }

        private bool RouteCondition(OutcomeData.OutcomeTarget target)
        {
            bool validateCondition = false;
            switch (target)
            {
                case OutcomeData.OutcomeTarget.Leader:
                    validateCondition =
                        ConditionSystem.CheckCharacterCondition(LeaderCharacters[0].GetTraits(), taskCondition);
                    break;
                case OutcomeData.OutcomeTarget.Assistant:
                    if (AssistantCharacters.Count >= 1)
                        validateCondition =
                            ConditionSystem.CheckCharacterCondition(AssistantCharacters[0].GetTraits(), taskCondition);
                    break;
                case OutcomeData.OutcomeTarget.Gauge:
                    validateCondition = ConditionSystem.CheckGaugeCondition(taskCondition);
                    break;
                case OutcomeData.OutcomeTarget.Crew:
                    validateCondition = ConditionSystem.CheckCrewCondition(taskCondition);
                    break;
                case OutcomeData.OutcomeTarget.Ship:
                    validateCondition = ConditionSystem.CheckSpaceshipCondition(taskCondition);
                    break;
                case OutcomeData.OutcomeTarget.None:
                    validateCondition = true;
                    break;
            }

            return validateCondition;
        }

        public void OnUpdate()
        {
            // TODO : CHECK CANCEL
            if (IsStarted)
            {
                if (Task.Duration > 0)
                {
                    time.text = TimeTickSystem.GetTicksAsTime((uint)Task.Duration);
                    Task.Duration -= TimeTickSystem.timePerTick;
                }
                else
                {
                    OnComplete();
                }
            }
            else if (Task.TaskType.Equals(SSTaskType.Timed))
            {
                if (Task.TimeLeft > 0)
                {
                    time.text = TimeTickSystem.GetTicksAsTime((uint)Task.TimeLeft);
                    Task.TimeLeft -= TimeTickSystem.timePerTick;
                }
                else
                {
                    GameManager.Instance.UIManager.taskUI.Initialize(this, false);
                    GameManager.Instance.UIManager.taskUI.StartTask();
                }
            }
        }

        private void OnComplete()
        {
            for (uint i = 0; i < outcomeEvents.Length; i++) outcomeEvents[i].Invoke(outcomeEventArgs[i]);
            IsCompleted = true;
            ResetCharacters();
            GameManager.Instance.RefreshCharacterIcons();
            transform.parent = null;
            spaceshipManager.notificationPool.AddToPool(gameObject);
        }

        public void OnCancel()
        {
            ResetCharacters();
            if (Task.TaskType.Equals(SSTaskType.Permanent))
            {
                transform.parent = null;
                spaceshipManager.notificationPool.AddToPool(gameObject);
            }
            else
            {
                taskCondition = Task.Conditions[^1].Item1;
                Task.conditionIndex = Task.Conditions.Count - 1;
                CheckingCondition(true);
            }
        }

        private void ResetCharacters()
        {
            foreach (var character in LeaderCharacters)
            {
                character.StopTask();
            }

            foreach (var character in AssistantCharacters)
            {
                character.StopTask();
            }
        }
    }
}