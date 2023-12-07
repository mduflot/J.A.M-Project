using System;
using System.Collections.Generic;
using System.Linq;
using CharacterSystem;
using Managers;
using SS.Enumerations;
using TMPro;
using UI;
using UnityEngine;

namespace Tasks
{
    public class TaskUI : MonoBehaviour
    {
        [Header("Task")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI timeLeftText;
        [SerializeField] private TextMeshProUGUI durationText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI previewOutcomeText;
        [SerializeField] private Transform charactersParent;
        [SerializeField] private CharacterUISlot[] inactiveSlots;
        [SerializeField] private WarningUI warningUI;

        [Header("Dialogues")]
        [SerializeField] private GameObject dialogueContainer;

        [Header("Values")]
        [SerializeField] private float timeLeft;
        [SerializeField] private float duration;

        private Notification notification;
        private List<CharacterUISlot> characterSlots = new();
        private bool taskStarted;

        public void Initialize(Notification notification, bool needToDisplay = true)
        {
            this.notification = notification;
            warningUI.gameObject.SetActive(false);
            titleText.text = this.notification.Task.Name;
            timeLeft = this.notification.Task.TimeLeft;
            duration = this.notification.Task.Duration;
            descriptionText.text = this.notification.Task.Description;
            taskStarted = false;

            for (int i = 0; i < notification.Task.MandatorySlots; i++)
            {
                var slot = inactiveSlots[i];
                slot.SetupSlot(true);
                slot.gameObject.SetActive(true);
                characterSlots.Add(slot);
            }

            for (int i = 3; i < this.notification.Task.OptionalSlots + 3; i++)
            {
                var slot = inactiveSlots[i];
                slot.SetupSlot(false);
                slot.gameObject.SetActive(true);
                characterSlots.Add(slot);
            }

            if (needToDisplay)
            {
                timeLeftText.SetText(timeLeft.ToString());
                gameObject.SetActive(true);
            }
        }

        public void DisplayTaskInfo(Notification n)
        {
            notification = n;
            warningUI.gameObject.SetActive(false);
            titleText.text = notification.Task.Name;
            duration = notification.Task.Duration;
            descriptionText.text = notification.Task.Description;

            for (int i = 0; i < notification.Task.leaderCharacters.Count; i++)
            {
                var charUI = GameManager.Instance.UIManager.GetCharacterUI(notification.Task.leaderCharacters[i]);
                var slot = inactiveSlots[i];
                slot.SetupSlot(true);
                slot.gameObject.SetActive(true);
                characterSlots.Add(slot);
                slot.character = charUI.character;
                charUI.icon.SetupIcon(slot.transform, slot);
            }

            for (int i = 0; i < notification.Task.assistantCharacters.Count; i++)
            {
                var charUI = GameManager.Instance.UIManager.GetCharacterUI(notification.Task.assistantCharacters[i]);
                if (charUI == null) continue;
                var slot = inactiveSlots[i];
                slot.SetupSlot(false);
                slot.gameObject.SetActive(true);
                characterSlots.Add(slot);
                slot.character = charUI.character;
                charUI.icon.SetupIcon(slot.transform, slot);
            }
        }

        public void Update()
        {
            if (!taskStarted)
            {
                bool canCheck = true;
                for (int j = 0; j < characterSlots.Count; j++)
                {
                    if (!characterSlots[j].isMandatory) continue;
                    var character = characterSlots[j];
                    if (character.icon == null)
                    {
                        canCheck = false;
                    }
                }

                if (canCheck)
                {
                    for (int index = 0; index < notification.Task.Conditions.Count; index++)
                    {
                        bool condition = false;
                        switch (notification.Task.Conditions[index].Item1.target)
                        {
                            case OutcomeData.OutcomeTarget.Leader:
                                for (int j = 0; j < characterSlots.Count; j++)
                                {
                                    if (!characterSlots[j].isMandatory) continue;
                                    var character = characterSlots[j];
                                    if (character.icon == null) continue;
                                    condition = ConditionSystem.CheckCharacterCondition(
                                        character.icon.character.GetTraits(),
                                        notification.Task.Conditions[index].Item1);
                                }

                                break;

                            case OutcomeData.OutcomeTarget.Assistant:
                                for (int j = 0; j < characterSlots.Count; j++)
                                {
                                    if (characterSlots[j].isMandatory) continue;
                                    var character = characterSlots[j];
                                    if (character.icon == null) continue;
                                    condition = ConditionSystem.CheckCharacterCondition(
                                        character.icon.character.GetTraits(),
                                        notification.Task.Conditions[index].Item1);
                                }

                                break;

                            case OutcomeData.OutcomeTarget.Crew:
                                condition = ConditionSystem.CheckCrewCondition(
                                    notification.Task.Conditions[index].Item1);
                                break;

                            case OutcomeData.OutcomeTarget.Gauge:
                                condition = ConditionSystem.CheckGaugeCondition(notification.Task.Conditions[index]
                                    .Item1);
                                break;

                            case OutcomeData.OutcomeTarget.Ship:
                                condition = ConditionSystem.CheckSpaceshipCondition(notification.Task.Conditions[index]
                                    .Item1);
                                break;
                        }

                        if (condition)
                        {
                            previewOutcomeText.text = notification.Task.Conditions[index].Item2;
                            for (int j = 0; j < notification.Task.Conditions[index].Item1.outcomes.Outcomes.Length; j++)
                            {
                                var outcome = notification.Task.Conditions[index].Item1.outcomes.Outcomes[j];
                                switch (outcome.OutcomeType)
                                {
                                    case OutcomeData.OutcomeType.Gauge:
                                        switch (outcome.OutcomeTargetGauge)
                                        {
                                            case SystemType.Airflow:
                                                previewOutcomeText.text += "\n" + outcome.OutcomeOperation + " " +
                                                                           outcome.value + " " +
                                                                           outcome.OutcomeTargetGauge;
                                                break;
                                            case SystemType.Hull:
                                                previewOutcomeText.text += "\n" + outcome.OutcomeOperation + " " +
                                                                           outcome.value + " " +
                                                                           outcome.OutcomeTargetGauge;
                                                break;
                                            case SystemType.Power:
                                                previewOutcomeText.text += "\n" + outcome.OutcomeOperation + " " +
                                                                           outcome.value + " " +
                                                                           outcome.OutcomeTargetGauge;
                                                break;
                                            case SystemType.Food:
                                                previewOutcomeText.text += "\n" + outcome.OutcomeOperation + " " +
                                                                           outcome.value + " " +
                                                                           outcome.OutcomeTargetGauge;
                                                break;
                                        }

                                        break;
                                    case OutcomeData.OutcomeType.Trait:
                                        if (outcome.OutcomeTargetTrait.GetJob() != TraitsData.Job.None)
                                        {
                                            switch (outcome.OutcomeTarget)
                                            {
                                                case OutcomeData.OutcomeTarget.Crew:
                                                    previewOutcomeText.text +=
                                                        "\n" + outcome.OutcomeOperation + " Crew " +
                                                        outcome.OutcomeTargetTrait.GetJob();
                                                    break;
                                                default:
                                                    for (int i = 0; i < characterSlots.Count; i++)
                                                    {
                                                        var character = characterSlots[i];
                                                        if (character.icon == null) continue;
                                                        previewOutcomeText.text += "\n" + outcome.OutcomeOperation +
                                                            " " + character.icon.character.GetCharacterData()
                                                                .firstName + " " +
                                                            outcome.OutcomeTargetTrait.GetJob();
                                                    }

                                                    break;
                                            }
                                        }

                                        if (outcome.OutcomeTargetTrait.GetPositiveTraits() !=
                                            TraitsData.PositiveTraits.None)
                                            previewOutcomeText.text += "\n" + outcome.OutcomeOperation + " " +
                                                                       outcome.OutcomeTargetTrait.GetPositiveTraits();
                                        if (outcome.OutcomeTargetTrait.GetNegativeTraits() !=
                                            TraitsData.NegativeTraits.None)
                                            previewOutcomeText.text += "\n" + outcome.OutcomeOperation + " " +
                                                                       outcome.OutcomeTargetTrait.GetNegativeTraits();
                                        if (outcome.OutcomeShipTrait != TraitsData.SpaceshipTraits.None)
                                        {
                                            foreach (TraitsData.SpaceshipTraits spaceshipTraits in Enum.GetValues(typeof(TraitsData.SpaceshipTraits)))
                                            {
                                                if (outcome.OutcomeShipTrait.HasFlag(spaceshipTraits) &&
                                                    spaceshipTraits != TraitsData.SpaceshipTraits.None)
                                                {
                                                    previewOutcomeText.text += "\n" + outcome.OutcomeOperation + " Ship " +
                                                                               spaceshipTraits;
                                                }
                                            }
                                        }
                                        break;
                                    case OutcomeData.OutcomeType.CharacterStat:
                                        previewOutcomeText.text += "\n" + outcome.OutcomeOperation + " " +
                                                                   outcome.value + " " + outcome.OutcomeTargetStat;
                                        break;
                                }
                            }

                            notification.Task.conditionIndex = index;
                            break;
                        }
                    }
                }
                else previewOutcomeText.text = null;

                var assistantCharacters = characterSlots.Count(slot => !slot.isMandatory && slot.icon != null);

                duration = assistantCharacters > 0
                    ? notification.Task.Duration /
                      (Mathf.Pow(assistantCharacters + 1, notification.Task.HelpFactor))
                    : notification.Task.Duration;

                durationText.text = TimeTickSystem.GetTicksAsTime((uint)(duration * TimeTickSystem.ticksPerHour));
            }
        }

        public void StartTask()
        {
            if (notification.Task.TaskType.Equals(SSTaskType.Permanent) ||
                notification.Task.TaskType.Equals(SSTaskType.Untimed))
                if (!CanStartTask())
                    return;
            if (CharactersWorking()) return;
            notification.OnStart(characterSlots);
            taskStarted = true;
            CloseTask();
        }

        private bool CanStartTask()
        {
            foreach (var slot in characterSlots)
            {
                if (slot.isMandatory && slot.icon == null)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Close the task UI
        /// </summary>
        public void CloseTask()
        {
            foreach (var slot in characterSlots)
            {
                if (slot.icon != null) slot.icon.ResetTransform();
                slot.ClearCharacter();
                slot.gameObject.SetActive(false);
            }

            previewOutcomeText.text = null;
            characterSlots.Clear();
            GameManager.Instance.RefreshCharacterIcons();
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Close the notification if it is permanent
        /// </summary>
        public void CloseNotification()
        {
            TimeTickSystem.ModifyTimeScale(1.0f);
            notification.OnCancel();
        }

        private bool CharactersWorking()
        {
            foreach (var character in characterSlots)
            {
                if (character.icon != null && character.icon.character.IsWorking())
                {
                    warningUI.gameObject.SetActive(true);
                    warningUI.Init(character.icon.character);
                    return true;
                }
            }

            return false;
        }
    }
}