using System;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private GameObject startButton;
        [SerializeField] private GameObject cancelButton;
        [SerializeField] private DialogueLog dialogueLog;

        [Header("Dialogues")]
        [SerializeField] private GameObject dialogueContainer;

        [Header("Values")]
        [SerializeField] private float timeLeft;
        [SerializeField] private float duration;
        
        private Notification notification;
        private List<CharacterUISlot> characterSlots = new();
        private bool taskStarted;
        private Animator animator;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void Initialize(Notification n, bool needToDisplay = true)
        {
            notification = n;
            titleText.text = notification.Task.Name;
            DisplayText(descriptionText, notification.Task.Description, 20);
            timeLeft = notification.Task.TimeLeft;
            duration = notification.Task.Duration;
            taskStarted = false;

            startButton.SetActive(true);
            cancelButton.SetActive(false);
            for (int i = 0; i < notification.Task.MandatorySlots; i++)
            {
                var slot = inactiveSlots[i];
                slot.SetupSlot(true);
                slot.gameObject.SetActive(true);
                characterSlots.Add(slot);
            }

            for (int i = 3; i < notification.Task.OptionalSlots + 3; i++)
            {
                var slot = inactiveSlots[i];
                slot.SetupSlot(false);
                slot.gameObject.SetActive(true);
                characterSlots.Add(slot);
            }
            dialogueLog.DisplayDialogueLog(notification.Dialogues);

            if (needToDisplay)
            {
                DisplayText(timeLeftText, timeLeft.ToString(), 100);
                timeLeftText.SetText(timeLeft.ToString());
                Appear(true);
            }
        }

        public void DisplayTaskInfo(Notification n)
        {
            notification = n;
            titleText.text = notification.Task.Name;
            DisplayText(descriptionText, notification.Task.Description, 20);
            duration = notification.Task.Duration;
            durationText.SetText(TimeTickSystem.GetTicksAsTime((uint)duration));

            for (int i = 0; i < notification.Task.leaderCharacters.Count; i++)
            {
                var charUI = GameManager.Instance.UIManager.GetCharacterUI(notification.Task.leaderCharacters[i]);
                var slot = inactiveSlots[i];
                slot.SetupSlot(true);
                slot.gameObject.SetActive(true);
                characterSlots.Add(slot);
                slot.icon = charUI.icon;
                charUI.icon.transform.SetParent(slot.transform);
            }

            for (int i = 0; i < notification.Task.assistantCharacters.Count; i++)
            {
                var charUI = GameManager.Instance.UIManager.GetCharacterUI(notification.Task.assistantCharacters[i]);
                if (charUI == null) continue;
                var slot = inactiveSlots[i];
                slot.SetupSlot(false);
                slot.gameObject.SetActive(true);
                characterSlots.Add(slot);
                slot.icon = charUI.icon;
                charUI.icon.transform.SetParent(slot.transform);
            }
            dialogueLog.DisplayDialogueLog(notification.Dialogues);
            startButton.SetActive(false);
            cancelButton.SetActive(true);
            Appear(true);
        }

        public void Update()
        {
            if(!animator.GetBool("Appear")) return;
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
                                        character.icon.character,
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
                                        character.icon.character,
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
            Appear(false);
            foreach (var slot in characterSlots)
            {
                if (slot.icon != null) slot.icon.ResetTransform();
                slot.ClearCharacter();
                slot.gameObject.SetActive(false);
            }
            if (notification.Task.TaskType.Equals(SSTaskType.Permanent) && !taskStarted) CloseNotification();
            previewOutcomeText.text = null;
            characterSlots.Clear();
            dialogueLog.ClearDialogueLog();
            GameManager.Instance.RefreshCharacterIcons();
        }

        /// <summary>
        /// Close the notification
        /// </summary>
        public void CloseNotification()
        {
            TimeTickSystem.ModifyTimeScale(1.0f);
            //Debug.Log("Je close la notification");
            notification.OnCancel();
        }

        public void CancelTask()
        {
            foreach (var slot in characterSlots)
            {
                if (slot.icon != null) slot.icon.ResetTransform();
                slot.icon.character.StopTask();
                slot.ClearCharacter();
                slot.gameObject.SetActive(false);
            }

            notification.IsCancelled = true;
            previewOutcomeText.text = null;
            characterSlots.Clear();
            GameManager.Instance.RefreshCharacterIcons();
            Appear(false);
            CloseNotification();
        }
        
        private bool CharactersWorking()
        {
            foreach (var character in characterSlots)
            {
                if (character.icon != null && character.icon.character.IsWorking())
                {
                    warningUI.CharacterWarning(character.icon.character);
                    return true;
                }
            }

            return false;
        }
        
        private void Appear(bool state)
        {
            animator.SetBool("Appear", state);
        }

        private async System.Threading.Tasks.Task DisplayText(TextMeshProUGUI text, string textToDisplay, int speed)
        {
            int letterIndex = 0;
            string tempText = "";
            text.text = tempText;
            while (letterIndex < textToDisplay.Length)
            {
                await System.Threading.Tasks.Task.Delay(speed);
                tempText += textToDisplay[letterIndex];
                text.text = tempText;
                letterIndex++;
            }
        }
        
    }

}