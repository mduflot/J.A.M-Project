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
        [SerializeField] private GameObject separator;

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
                separator.SetActive(true);
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
                var slot = inactiveSlots[i + 3];
                slot.SetupSlot(false);
                slot.gameObject.SetActive(true);
                characterSlots.Add(slot);
                slot.icon = charUI.icon;
                charUI.icon.transform.SetParent(slot.transform);
            }

            dialogueLog.DisplayDialogueLog(notification.Dialogues);
            startButton.SetActive(false);
            cancelButton.SetActive(true);
            separator.SetActive(true);
            Appear(true);
        }

        // TODO : Refactor this, don't want to use Update for this
        public void Update()
        {
            if (!animator.GetBool("Appear")) return;
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
                            case OutcomeData.OutcomeTarget.None:
                                condition = true;
                                break;
                        }

                        if (condition)
                        {
                            previewOutcomeText.text =
                                $"{characterSlots[0].icon.character.GetCharacterData().name} {notification.Task.Conditions[index].Item2}\n";
                            if (notification.Task.Conditions[index].Item1.BaseCondition.Traits.GetJob() !=
                                TraitsData.Job.None)
                                previewOutcomeText.text +=
                                    $"{notification.Task.Conditions[index].Item1.BaseCondition.Traits.GetJob()}";
                            if (notification.Task.Conditions[index].Item1.BaseCondition.Traits.GetPositiveTraits() !=
                                TraitsData.PositiveTraits.None)
                                previewOutcomeText.text +=
                                    $"{notification.Task.Conditions[index].Item1.BaseCondition.Traits.GetPositiveTraits()}";
                            if (notification.Task.Conditions[index].Item1.BaseCondition.Traits.GetNegativeTraits() !=
                                TraitsData.NegativeTraits.None)
                                previewOutcomeText.text +=
                                    $"{notification.Task.Conditions[index].Item1.BaseCondition.Traits.GetNegativeTraits()}";
                            
                            for (int j = 0; j < notification.Task.Conditions[index].Item1.outcomes.Outcomes.Length; j++)
                            {
                                var outcome = notification.Task.Conditions[index].Item1.outcomes.Outcomes[j];
                                switch (outcome.OutcomeType)
                                {
                                    case OutcomeData.OutcomeType.Gauge:
                                        switch (outcome.OutcomeTargetGauge)
                                        {
                                            case SystemType.Airflow:
                                                previewOutcomeText.text +=
                                                    $"<color=blue>{outcome.OutcomeOperation} {outcome.value} {outcome.OutcomeTargetGauge}</color>\n";
                                                break;
                                            case SystemType.Food:
                                                previewOutcomeText.text +=
                                                    $"<color=green>{outcome.OutcomeOperation} {outcome.value} {outcome.OutcomeTargetGauge}</color>\n";
                                                break;
                                            case SystemType.Hull:
                                                previewOutcomeText.text +=
                                                    $"<color=red>{outcome.OutcomeOperation} {outcome.value} {outcome.OutcomeTargetGauge}</color>\n";
                                                break;
                                            case SystemType.Power:
                                                previewOutcomeText.text +=
                                                    $"<color=yellow>{outcome.OutcomeOperation} {outcome.value} {outcome.OutcomeTargetGauge}</color>\n";
                                                break;
                                        }

                                        break;
                                    case OutcomeData.OutcomeType.GaugeVolition:
                                        switch (outcome.OutcomeTargetGauge)
                                        {
                                            case SystemType.Airflow:
                                                previewOutcomeText.text +=
                                                    $"<color=blue>{outcome.OutcomeOperation} {characterSlots[0].icon.character.GetVolition()} {outcome.OutcomeTargetGauge}</color>\n";
                                                break;
                                            case SystemType.Food:
                                                previewOutcomeText.text +=
                                                    $"<color=green>{outcome.OutcomeOperation} {characterSlots[0].icon.character.GetVolition()} {outcome.OutcomeTargetGauge}</color>\n";
                                                break;
                                            case SystemType.Hull:
                                                previewOutcomeText.text +=
                                                    $"<color=red>{outcome.OutcomeOperation} {characterSlots[0].icon.character.GetVolition()} {outcome.OutcomeTargetGauge}</color>\n";
                                                break;
                                            case SystemType.Power:
                                                previewOutcomeText.text +=
                                                    $"<color=yellow>{outcome.OutcomeOperation} {characterSlots[0].icon.character.GetVolition()} {outcome.OutcomeTargetGauge}</color>\n";
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
                                                        $"{outcome.OutcomeOperation} Crew {outcome.OutcomeTargetTrait.GetJob()}\n";
                                                    break;
                                                default:
                                                    for (int i = 0; i < characterSlots.Count; i++)
                                                    {
                                                        var character = characterSlots[i];
                                                        if (character.icon == null) continue;
                                                        previewOutcomeText.text +=
                                                            $"{outcome.OutcomeOperation} {character.icon.character.GetCharacterData().firstName} {outcome.OutcomeTargetTrait.GetJob()}\n";
                                                    }

                                                    break;
                                            }
                                        }

                                        if (outcome.OutcomeTargetTrait.GetPositiveTraits() !=
                                            TraitsData.PositiveTraits.None)
                                            previewOutcomeText.text +=
                                                $"{outcome.OutcomeOperation} {outcome.OutcomeTargetTrait.GetPositiveTraits()}\n";
                                        if (outcome.OutcomeTargetTrait.GetNegativeTraits() !=
                                            TraitsData.NegativeTraits.None)
                                            previewOutcomeText.text +=
                                                $"{outcome.OutcomeOperation} {outcome.OutcomeTargetTrait.GetNegativeTraits()}\n";
                                        if (outcome.OutcomeShipTrait != TraitsData.SpaceshipTraits.None)
                                        {
                                            foreach (TraitsData.SpaceshipTraits spaceshipTraits in Enum.GetValues(
                                                         typeof(TraitsData.SpaceshipTraits)))
                                            {
                                                if (outcome.OutcomeShipTrait.HasFlag(spaceshipTraits) &&
                                                    spaceshipTraits != TraitsData.SpaceshipTraits.None)
                                                {
                                                    previewOutcomeText.text +=
                                                        $"{outcome.OutcomeOperation} Ship {spaceshipTraits}\n";
                                                }
                                            }
                                        }

                                        break;
                                    case OutcomeData.OutcomeType.CharacterStat:
                                        previewOutcomeText.text +=
                                            $"{outcome.OutcomeOperation} {outcome.value} {outcome.OutcomeTargetStat}\n";
                                        break;
                                }
                            }

                            notification.Task.conditionIndex = index;

                            for (int jindex = 0;
                                 jindex < notification.Task.Conditions[index].Item1.additionnalConditions.Length;
                                 jindex++)
                            {
                                condition = false;
                                switch (notification.Task.Conditions[index].Item1.additionnalConditions[jindex].target)
                                {
                                    case OutcomeData.OutcomeTarget.Leader:
                                        for (int j = 0; j < characterSlots.Count; j++)
                                        {
                                            if (!characterSlots[j].isMandatory) continue;
                                            var character = characterSlots[j];
                                            if (character.icon == null) continue;
                                            condition = ConditionSystem.CheckCharacterCondition(
                                                character.icon.character,
                                                notification.Task.Conditions[index].Item1
                                                    .additionnalConditions[jindex]);
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
                                                notification.Task.Conditions[index].Item1
                                                    .additionnalConditions[jindex]);
                                        }

                                        break;

                                    case OutcomeData.OutcomeTarget.Crew:
                                        condition = ConditionSystem.CheckCrewCondition(
                                            notification.Task.Conditions[index].Item1.additionnalConditions[jindex]);
                                        break;

                                    case OutcomeData.OutcomeTarget.Gauge:
                                        condition = ConditionSystem.CheckGaugeCondition(notification.Task
                                            .Conditions[index]
                                            .Item1.additionnalConditions[jindex]);
                                        break;

                                    case OutcomeData.OutcomeTarget.Ship:
                                        condition = ConditionSystem.CheckSpaceshipCondition(notification.Task
                                            .Conditions[index].Item1.additionnalConditions[jindex]);
                                        break;
                                    case OutcomeData.OutcomeTarget.None:
                                        condition = true;
                                        break;
                                }

                                if (condition)
                                {
                                    if (notification.Task.Conditions[index].Item1.additionnalConditions[jindex].BaseCondition.Traits.GetJob() != TraitsData.Job.None)
                                        previewOutcomeText.text +=
                                            $"{notification.Task.Conditions[index].Item1.additionnalConditions[jindex].BaseCondition.Traits.GetJob()} ";
                                    if (notification.Task.Conditions[index].Item1.additionnalConditions[jindex].BaseCondition.Traits
                                            .GetPositiveTraits() !=
                                        TraitsData.PositiveTraits.None)
                                        previewOutcomeText.text +=
                                            $"{notification.Task.Conditions[index].Item1.additionnalConditions[jindex].BaseCondition.Traits.GetPositiveTraits()} ";
                                    if (notification.Task.Conditions[index].Item1.additionnalConditions[jindex].BaseCondition.Traits
                                            .GetNegativeTraits() !=
                                        TraitsData.NegativeTraits.None)
                                        previewOutcomeText.text +=
                                            $"{notification.Task.Conditions[index].Item1.additionnalConditions[jindex].BaseCondition.Traits.GetNegativeTraits()} ";
                                    for (int j = 0;
                                         j < notification.Task.Conditions[index].Item1.additionnalConditions[jindex]
                                             .outcomes.Outcomes.Length;
                                         j++)
                                    {
                                        var outcome = notification.Task.Conditions[index].Item1
                                            .additionnalConditions[jindex].outcomes.Outcomes[j];
                                        switch (outcome.OutcomeType)
                                        {
                                            case OutcomeData.OutcomeType.Gauge:
                                                switch (outcome.OutcomeTargetGauge)
                                                {
                                                    case SystemType.Airflow:
                                                        previewOutcomeText.text +=
                                                            $"<color=blue>{outcome.OutcomeOperation} {outcome.value} {outcome.OutcomeTargetGauge}</color>\n";
                                                        break;
                                                    case SystemType.Food:
                                                        previewOutcomeText.text +=
                                                            $"<color=green>{outcome.OutcomeOperation} {outcome.value} {outcome.OutcomeTargetGauge}</color>\n";
                                                        break;
                                                    case SystemType.Hull:
                                                        previewOutcomeText.text +=
                                                            $"<color=red>{outcome.OutcomeOperation} {outcome.value} {outcome.OutcomeTargetGauge}</color>\n";
                                                        break;
                                                    case SystemType.Power:
                                                        previewOutcomeText.text +=
                                                            $"<color=yellow>{outcome.OutcomeOperation} {outcome.value} {outcome.OutcomeTargetGauge}</color>\n";
                                                        break;
                                                }

                                                break;
                                            case OutcomeData.OutcomeType.GaugeVolition:
                                                switch (outcome.OutcomeTargetGauge)
                                                {
                                                    case SystemType.Airflow:
                                                        previewOutcomeText.text +=
                                                            $"<color=blue>{outcome.OutcomeOperation} {characterSlots[0].icon.character.GetVolition()} {outcome.OutcomeTargetGauge}</color>\n";
                                                        break;
                                                    case SystemType.Food:
                                                        previewOutcomeText.text +=
                                                            $"<color=green>{outcome.OutcomeOperation} {characterSlots[0].icon.character.GetVolition()} {outcome.OutcomeTargetGauge}</color>\n";
                                                        break;
                                                    case SystemType.Hull:
                                                        previewOutcomeText.text +=
                                                            $"<color=red>{outcome.OutcomeOperation} {characterSlots[0].icon.character.GetVolition()} {outcome.OutcomeTargetGauge}</color>\n";
                                                        break;
                                                    case SystemType.Power:
                                                        previewOutcomeText.text +=
                                                            $"<color=yellow>{outcome.OutcomeOperation} {characterSlots[0].icon.character.GetVolition()} {outcome.OutcomeTargetGauge}</color>\n";
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
                                                                $"{outcome.OutcomeOperation} Crew {outcome.OutcomeTargetTrait.GetJob()}\n";
                                                            break;
                                                        default:
                                                            for (int i = 0; i < characterSlots.Count; i++)
                                                            {
                                                                var character = characterSlots[i];
                                                                if (character.icon == null) continue;
                                                                previewOutcomeText.text +=
                                                                    $"{outcome.OutcomeOperation} {character.icon.character.GetCharacterData().firstName} {outcome.OutcomeTargetTrait.GetJob()}\n";
                                                            }

                                                            break;
                                                    }
                                                }

                                                if (outcome.OutcomeTargetTrait.GetPositiveTraits() !=
                                                    TraitsData.PositiveTraits.None)
                                                    previewOutcomeText.text +=
                                                        $"{outcome.OutcomeOperation} {outcome.OutcomeTargetTrait.GetPositiveTraits()}\n";
                                                if (outcome.OutcomeTargetTrait.GetNegativeTraits() !=
                                                    TraitsData.NegativeTraits.None)
                                                    previewOutcomeText.text +=
                                                        $"{outcome.OutcomeOperation} {outcome.OutcomeTargetTrait.GetNegativeTraits()}\n";
                                                if (outcome.OutcomeShipTrait != TraitsData.SpaceshipTraits.None)
                                                {
                                                    foreach (TraitsData.SpaceshipTraits spaceshipTraits in Enum
                                                                 .GetValues(
                                                                     typeof(TraitsData.SpaceshipTraits)))
                                                    {
                                                        if (outcome.OutcomeShipTrait.HasFlag(spaceshipTraits) &&
                                                            spaceshipTraits != TraitsData.SpaceshipTraits.None)
                                                        {
                                                            previewOutcomeText.text +=
                                                                $"{outcome.OutcomeOperation} Ship {spaceshipTraits}\n";
                                                        }
                                                    }
                                                }

                                                break;
                                            case OutcomeData.OutcomeType.CharacterStat:
                                                previewOutcomeText.text +=
                                                    $"{outcome.OutcomeOperation} {outcome.value} {outcome.OutcomeTargetStat}\n";
                                                break;
                                        }
                                    }

                                    break;
                                }
                            }

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
            TimeTickSystem.ModifyTimeScale(1.0f);
            previewOutcomeText.text = null;
            characterSlots.Clear();
            dialogueLog.ClearDialogueLog();
            separator.SetActive(false);
            GameManager.Instance.RefreshCharacterIcons();
        }

        /// <summary>
        /// Close the notification
        /// </summary>
        public void CloseNotification()
        {
            TimeTickSystem.ModifyTimeScale(1.0f);
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
            separator.SetActive(false);
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