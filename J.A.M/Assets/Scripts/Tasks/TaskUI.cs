using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CharacterSystem;
using Managers;
using SS;
using SS.Enumerations;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Tasks
{
    public class TaskUI : MonoBehaviour
    {
        [Header("Task")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI timeLeftText;
        [SerializeField] private GameObject timeLeftObject;
        [SerializeField] private Transform startButtonObject;
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
        [SerializeField] private GameObject popupHelp;

        [Header("Values")]
        [SerializeField] private float timeLeft;
        [SerializeField] private float duration;

        private Notification notification;
        private List<CharacterUISlot> characterSlots = new();
        private bool taskStarted;
        [SerializeField] private Animator animator;
        private List<GaugesOutcome> gaugesOutcomes = new();
        private List<CharacterOutcome> charOutcome = new();

        private bool isConditionMet;

        public struct GaugesOutcome
        {
            public SystemType gauge;
            public float value;

            public GaugesOutcome(SystemType system, float value)
            {
                gauge = system;
                this.value = value;
            }
        }

        public struct CharacterOutcome
        {
            public CharacterBehaviour character;
            public float value;

            public CharacterOutcome(CharacterBehaviour c, float value)
            {
                character = c;
                this.value = value;
            }
        }

        public void Initialize(Notification n, CharacterIcon icon = null, bool needToDisplay = true,
            TaskLog taskLog = null)
        {
            notification = n;
            if (notification.Task.IsTaskTutorial) popupHelp.SetActive(true);
            titleText.text = $"{notification.Task.Name} / {notification.Task.NameStoryline}";
            StartCoroutine(DisplayText(descriptionText, notification.Task.Description, 0.02f));
            timeLeft = notification.Task.TimeLeft;
            if (taskLog != null) timeLeft = taskLog.Duration;
            else duration = notification.Task.Duration;
            taskStarted = false;

            startButton.SetActive(true);
            cancelButton.SetActive(false);
            if (notification.Task.TaskType != SSTaskType.Compute)
            {
                characterSlots.Clear();
                for (int i = 0; i < notification.Task.MandatorySlots; i++)
                {
                    var slot = inactiveSlots[i];
                    slot.SetupSlot(true, this);
                    slot.gameObject.SetActive(true);
                    characterSlots.Add(slot);
                }

                for (int i = 3; i < notification.Task.OptionalSlots + 3; i++)
                {
                    var slot = inactiveSlots[i];
                    slot.SetupSlot(false, this);
                    slot.gameObject.SetActive(true);
                    characterSlots.Add(slot);
                }
            }

            dialogueLog.DisplayDialogueLog(notification.Dialogues);

            if (icon != null) SetLeader(icon);
            if (taskLog != null)
            {
                if (GameManager.Instance.UIManager.characterIcons.First(characterIcon =>
                        characterIcon.character.GetCharacterData().ID == taskLog.LeaderCharacter[0]))
                {
                    SetLeader(GameManager.Instance.UIManager.characterIcons.First(characterIcon =>
                        characterIcon.character.GetCharacterData().ID == taskLog.LeaderCharacter[0]));
                }

                for (int indexAssistant = 0; indexAssistant < taskLog.AssistantCharacters.Count; indexAssistant++)
                {
                    if (GameManager.Instance.UIManager.characterIcons.First(characterIcon =>
                            characterIcon.character.GetCharacterData().ID ==
                            taskLog.AssistantCharacters[indexAssistant]))
                    {
                        SetAssistant(GameManager.Instance.UIManager.characterIcons.First(characterIcon =>
                            characterIcon.character.GetCharacterData().ID ==
                            taskLog.AssistantCharacters[indexAssistant]));
                    }
                }
            }

            if (notification.Task.TaskType != SSTaskType.Timed)
            {
                startButtonObject.localPosition =
                    new Vector3(50, startButtonObject.localPosition.y, startButtonObject.localPosition.z);
                timeLeftObject.SetActive(false);
            }
            else
            {
                startButtonObject.localPosition =
                    new Vector3(0, startButtonObject.localPosition.y, startButtonObject.localPosition.z);
                timeLeftObject.SetActive(true);
            }

            if (needToDisplay)
            {
                StartCoroutine(DisplayText(timeLeftText,
                    "Ends in : " + (timeLeft / TimeTickSystem.ticksPerHour).ToString("F2"), 0.02f));
                separator.SetActive(true);
                Appear(true);
                GameManager.Instance.taskOpened = true;
            }

            UpdatePreview();
        }

        public void DisplayTaskInfo(Notification n)
        {
            notification = n;
            if (notification.Task.IsTaskTutorial) popupHelp.SetActive(true);
            titleText.text = notification.Task.Name;
            StartCoroutine(DisplayText(descriptionText, notification.Task.Description, 0.02f));
            StartCoroutine(DisplayText(previewOutcomeText, notification.Task.previewText, 0.02f));
            duration = notification.Task.Duration;
            durationText.SetText(TimeTickSystem.GetTicksAsTime((uint) duration));

            for (int i = 0; i < notification.Task.leaderCharacters.Count; i++)
            {
                var charUI = GameManager.Instance.UIManager.GetCharacterUI(notification.Task.leaderCharacters[i]);
                var slot = inactiveSlots[i];
                slot.SetupSlot(true, this);
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
                slot.SetupSlot(false, this);
                slot.gameObject.SetActive(true);
                characterSlots.Add(slot);
                slot.icon = charUI.icon;
                charUI.icon.transform.SetParent(slot.transform);
            }

            dialogueLog.DisplayDialogueLog(notification.Dialogues);
            startButton.SetActive(false);
            cancelButton.SetActive(true);
            cancelButton.GetComponentInChildren<Button>().interactable = !notification.Task.IsTaskTutorial;
            separator.SetActive(true);
            GameManager.Instance.taskOpened = true;
            Appear(true);
            UpdatePreview();
        }

        public void UpdatePreview()
        {
            if (!animator.GetBool("Appear")) return;
            if (taskStarted) return;
            isConditionMet = false;
            List<GaugesOutcome> gaugeOutcomes = new List<GaugesOutcome>();
            List<CharacterOutcome> characterOutcomes = new List<CharacterOutcome>();

            var assistantCharacters = characterSlots.Count(slot => !slot.isMandatory && slot.icon != null);
            foreach (var c in characterSlots)
            {
                if (c.icon != null)
                    characterOutcomes.Add(new CharacterOutcome(c.icon.character,
                        -GameManager.Instance.SpaceshipManager.moodLossOnTaskStart));
            }

            charOutcome = characterOutcomes;
            GameManager.Instance.UIManager.CharacterPreviewGauges(charOutcome);
            duration = assistantCharacters > 0
                ? notification.Task.Duration /
                  Mathf.Pow(assistantCharacters + 1, notification.Task.HelpFactor)
                : notification.Task.Duration;

            durationText.text = TimeTickSystem.GetTicksAsTime((uint) (duration * TimeTickSystem.ticksPerHour));
            var button = startButton.GetComponentInChildren<Button>();
            var text = button.GetComponentInChildren<TextMeshProUGUI>();
            var image = button.GetComponent<Image>();
            button.Select();

            for (int index = 0; index < notification.Task.Conditions.Count; index++)
            {
                bool condition = CheckTarget(notification.Task.Conditions[index].Item1);
                if (notification.Task.TaskType != SSTaskType.Compute) {
                    if (characterSlots[0]?.icon == null && notification.Task.TaskType != SSTaskType.Untimed) {
                        index = notification.Task.Conditions.Count - 1;
                        condition = true;
                    }
                }

                if (!condition && notification.Task.TaskType == SSTaskType.Compute)
                {
                    previewOutcomeText.text = "Condition not met";
                    button.interactable = false;
                    text.text = "Bad combination";
                    image.color = Color.black;
                }
                else
                {
                    previewOutcomeText.text = null;
                    button.interactable = true;
                    text.text = "Start Task";
                    image.color = Color.white;
                }

                if (notification.Task.TaskType != SSTaskType.Compute)
                {
                    if ((!condition && notification.Task.TaskType == SSTaskType.Untimed) ||
                        (characterSlots[0].icon == null && notification.Task.TaskType == SSTaskType.Permanent))
                    {
                        previewOutcomeText.text = "Condition not met";
                        button.interactable = false;
                        text.text = "Bad combination";
                        image.color = Color.black;
                    }
                    else
                    {
                        previewOutcomeText.text = null;
                        button.interactable = true;
                        text.text = "Start Task";
                        image.color = Color.white;
                    }
                }

                if (condition)
                {
                    isConditionMet = condition;
                    previewOutcomeText.text = null;
                    if (notification.Task.TaskType != SSTaskType.Compute)
                    {
                        if (characterSlots[0].icon != null)
                            previewOutcomeText.text =
                                $"{characterSlots[0].icon.character.GetCharacterData().firstName} {notification.Task.Conditions[index].Item2}\n";
                    }
                    else previewOutcomeText.text = $"{notification.Task.Conditions[index].Item2}\n";

                    var traits = DisplayTraits(notification.Task.Conditions[index].Item1);

                    for (int j = 0; j < notification.Task.Conditions[index].Item1.outcomes.Outcomes.Length; j++)
                    {
                        var outcome = notification.Task.Conditions[index].Item1.outcomes.Outcomes[j];
                        DisplayPreview(outcome, traits, gaugeOutcomes);
                    }

                    notification.Task.conditionIndex = index;

                    for (int jindex = 0;
                         jindex < notification.Task.Conditions[index].Item1.additionnalConditions.Length;
                         jindex++)
                    {
                        condition = CheckTarget(notification.Task.Conditions[index].Item1
                            .additionnalConditions[jindex]);

                        if (condition)
                        {
                            traits = DisplayTraits(notification.Task.Conditions[index].Item1
                                .additionnalConditions[jindex]);
                            for (int j = 0;
                                 j < notification.Task.Conditions[index].Item1.additionnalConditions[jindex]
                                     .outcomes.Outcomes.Length;
                                 j++)
                            {
                                var outcome = notification.Task.Conditions[index].Item1
                                    .additionnalConditions[jindex].outcomes.Outcomes[j];
                                DisplayPreview(outcome, traits, gaugeOutcomes);
                                foreach (var trait in GameManager.Instance.UIManager.characterInfoUI.characterTraits)
                                {
                                    if (traits.ToLower().Contains(trait.GetName().ToLower()))
                                    {
                                        trait.ChangeColor(notification.Task.Conditions[index].Item1
                                            .additionnalConditions[jindex].outcomes.Outcomes[j]
                                            .OutcomeOperation == OutcomeData.OutcomeOperation.Add);
                                    }
                                }
                            }
                        }
                    }

                    notification.Task.previewText = previewOutcomeText.text;
                    break;
                }
            }

            gaugesOutcomes = gaugeOutcomes;
            GameManager.Instance.UIManager.ResetPreviewGauges();
            GameManager.Instance.UIManager.PreviewOutcomeGauges(gaugesOutcomes);
        }

        private string DisplayTraits(ConditionSO conditionSO)
        {
            var traits = "";
            if (conditionSO.BaseCondition.Traits.GetJob() !=
                TraitsData.Job.None)
                traits +=
                    $"{conditionSO.BaseCondition.Traits.GetJob()}: ";
            if (conditionSO.BaseCondition.Traits.GetPositiveTraits() !=
                TraitsData.PositiveTraits.None)
                traits +=
                    $"{conditionSO.BaseCondition.Traits.GetPositiveTraits()}: ";
            if (conditionSO.BaseCondition.Traits.GetNegativeTraits() !=
                TraitsData.NegativeTraits.None)
                traits +=
                    $"{conditionSO.BaseCondition.Traits.GetNegativeTraits()}: ";
            return traits;
        }

        private void DisplayPreview(Outcome outcome, string traits, List<GaugesOutcome> gaugeOutcomes)
        {
            var operation = "+";
            var operationString = "green";
            switch (outcome.OutcomeType)
            {
                case OutcomeData.OutcomeType.Gauge:
                    if (outcome.OutcomeOperation == OutcomeData.OutcomeOperation.Sub)
                    {
                        operation = "-";
                        operationString = "red";
                    }

                    switch (outcome.OutcomeTargetGauge)
                    {
                        case SystemType.Trajectory:
                            previewOutcomeText.text +=
                                $"{traits} <color={operationString}>{operation} {outcome.value}</color> <sprite=19>\n";
                            break;
                        case SystemType.Food:
                            previewOutcomeText.text +=
                                $"{traits} <color={operationString}>{operation} {outcome.value}</color> <sprite=14>\n";
                            break;
                        case SystemType.Hull:
                            previewOutcomeText.text +=
                                $"{traits} <color={operationString}>{operation} {outcome.value}</color> <sprite=9>\n";
                            break;
                        case SystemType.Power:
                            previewOutcomeText.text +=
                                $"{traits} <color={operationString}>{operation} {outcome.value}</color> <sprite=4>\n";
                            break;
                    }

                    var value = outcome.OutcomeOperation == OutcomeData.OutcomeOperation.Add
                        ? outcome.value
                        : -outcome.value;

                    gaugeOutcomes.Add(new GaugesOutcome(outcome.OutcomeTargetGauge,
                        value));

                    break;
                case OutcomeData.OutcomeType.GaugeVolition:
                    if (characterSlots[0].icon == null) break;
                    if (outcome.OutcomeOperation == OutcomeData.OutcomeOperation.Sub)
                    {
                        operation = "-";
                        operationString = "red";
                    }

                    var valueVolition = characterSlots[0].icon.character.GetBaseVolition();
                    switch (outcome.OutcomeTargetGauge)
                    {
                        case SystemType.Trajectory:
                            previewOutcomeText.text +=
                                $"{traits} <sprite=20> : <color={operationString}>{operation} {valueVolition.ToString("F2")} <sprite=19></color>\n";
                            break;
                        case SystemType.Food:
                            previewOutcomeText.text +=
                                $"{traits} <sprite=20> : <color={operationString}>{operation} {valueVolition.ToString("F2")} <sprite=14></color>\n";
                            break;
                        case SystemType.Hull:
                            previewOutcomeText.text +=
                                $"{traits} <sprite=20> : <color={operationString}>{operation} {valueVolition.ToString("F2")} <sprite=9></color>\n";
                            break;
                        case SystemType.Power:
                            previewOutcomeText.text +=
                                $"{traits} <sprite=20> : <color={operationString}>{operation} {valueVolition.ToString("F2")} <sprite=4></color>\n";
                            break;
                    }

                    if (characterSlots[0].icon.character.GetMood() < characterSlots[0].icon.character.GetBaseVolition())
                    {
                        valueVolition /= 2;
                        previewOutcomeText.text +=
                            $"<color=#ff00ffff><sprite=21> - {valueVolition.ToString("F2")}</color>\n";
                    }

                    var volition =
                        outcome.OutcomeOperation == OutcomeData.OutcomeOperation.Add ? valueVolition : -valueVolition;

                    if (notification.Task.TaskType == SSTaskType.Permanent)
                    {
                        for (int index = 0; index < GameManager.Instance.SpaceshipManager.systems.Length; index++)
                        {
                            var system = GameManager.Instance.SpaceshipManager.systems[index];
                            if (system.type == outcome.OutcomeTargetGauge)
                            {
                                volition -= system.decreaseValues[0] * Mathf.FloorToInt(duration);
                                break;
                            }
                        }
                    }

                    gaugeOutcomes.Add(new GaugesOutcome(outcome.OutcomeTargetGauge, volition));
                    break;
                case OutcomeData.OutcomeType.Trait:
                    previewOutcomeText.text += traits;
                    if (outcome.OutcomeTargetTrait.GetJob() != TraitsData.Job.None)
                    {
                        switch (outcome.OutcomeTarget)
                        {
                            case OutcomeData.OutcomeTarget.Crew:
                                previewOutcomeText.text +=
                                    $"{outcome.OutcomeOperation} {outcome.OutcomeTargetTrait.GetJob()} to Crew\n";
                                break;
                            case OutcomeData.OutcomeTarget.Leader:
                                previewOutcomeText.text +=
                                    $"{outcome.OutcomeOperation} {outcome.OutcomeTargetTrait.GetJob()} to {characterSlots[0].icon.character.GetCharacterData().firstName}\n";
                                break;
                            case OutcomeData.OutcomeTarget.Assistant:
                                for (int i = 0; i < characterSlots.Count; i++)
                                {
                                    var character = characterSlots[i];
                                    if (character.isMandatory) continue;
                                    if (character.icon == null) continue;
                                    previewOutcomeText.text +=
                                        $"{outcome.OutcomeOperation} {outcome.OutcomeTargetTrait.GetJob()} to {character.icon.character.GetCharacterData().firstName}\n";
                                }

                                break;
                        }
                    }

                    if (outcome.OutcomeTargetTrait.GetPositiveTraits() !=
                        TraitsData.PositiveTraits.None)
                        switch (outcome.OutcomeTarget)
                        {
                            case OutcomeData.OutcomeTarget.Crew:
                                previewOutcomeText.text +=
                                    $"{outcome.OutcomeOperation} {outcome.OutcomeTargetTrait.GetPositiveTraits()} to Crew\n";
                                break;
                            case OutcomeData.OutcomeTarget.Leader:
                                previewOutcomeText.text +=
                                    $"{outcome.OutcomeOperation} {outcome.OutcomeTargetTrait.GetPositiveTraits()} to {characterSlots[0].icon.character.GetCharacterData().firstName}\n";
                                break;
                            case OutcomeData.OutcomeTarget.Assistant:
                                for (int i = 0; i < characterSlots.Count; i++)
                                {
                                    var character = characterSlots[i];
                                    if (character.isMandatory) continue;
                                    if (character.icon == null) continue;
                                    previewOutcomeText.text +=
                                        $"{outcome.OutcomeOperation} {outcome.OutcomeTargetTrait.GetPositiveTraits()} to {character.icon.character.GetCharacterData().firstName}\n";
                                }

                                break;
                        }

                    if (outcome.OutcomeTargetTrait.GetNegativeTraits() !=
                        TraitsData.NegativeTraits.None)
                        switch (outcome.OutcomeTarget)
                        {
                            case OutcomeData.OutcomeTarget.Crew:
                                previewOutcomeText.text +=
                                    $"{outcome.OutcomeOperation} {outcome.OutcomeTargetTrait.GetNegativeTraits()} to Crew\n";
                                break;
                            case OutcomeData.OutcomeTarget.Leader:
                                previewOutcomeText.text +=
                                    $"{outcome.OutcomeOperation} {outcome.OutcomeTargetTrait.GetNegativeTraits()} to {characterSlots[0].icon.character.GetCharacterData().firstName}\n";
                                break;
                            case OutcomeData.OutcomeTarget.Assistant:
                                for (int i = 0; i < characterSlots.Count; i++)
                                {
                                    var character = characterSlots[i];
                                    if (character.isMandatory) continue;
                                    if (character.icon == null) continue;
                                    previewOutcomeText.text +=
                                        $"{outcome.OutcomeOperation} {outcome.OutcomeTargetTrait.GetNegativeTraits()} to {character.icon.character.GetCharacterData().firstName}\n";
                                }

                                break;
                        }

                    if (outcome.OutcomeShipTrait != TraitsData.SpaceshipTraits.None)
                    {
                        foreach (TraitsData.SpaceshipTraits spaceshipTraits in Enum.GetValues(
                                     typeof(TraitsData.SpaceshipTraits)))
                        {
                            if (outcome.OutcomeShipTrait.HasFlag(spaceshipTraits) &&
                                spaceshipTraits != TraitsData.SpaceshipTraits.None)
                            {
                                previewOutcomeText.text +=
                                    $"{outcome.OutcomeOperation} {spaceshipTraits} to Ship\n";
                            }
                        }
                    }

                    break;
                case OutcomeData.OutcomeType.CharacterStat:
                    previewOutcomeText.text += traits;
                    string operationStat = "+";
                    string targetStat = "<sprite=20>";
                    string operationStringStat = "green";
                    if (outcome.OutcomeOperation == OutcomeData.OutcomeOperation.Sub)
                    {
                        operationStat = "-";
                        operationStringStat = "red";
                    }

                    if (outcome.OutcomeTargetStat == OutcomeData.OutcomeTargetStat.Mood)
                    {
                        targetStat = "<sprite=22>";
                        if (outcome.OutcomeOperation == OutcomeData.OutcomeOperation.Sub)
                        {
                            operationStat = "-";
                            targetStat = "<sprite=21>";
                            operationStringStat = "red";
                        }
                    }

                    switch (outcome.OutcomeTarget)
                    {
                        case OutcomeData.OutcomeTarget.Crew:
                            previewOutcomeText.text +=
                                $"<color={operationStringStat}>{operationStat} {outcome.value}</color> {targetStat} to Crew\n";
                            break;
                        case OutcomeData.OutcomeTarget.Leader:
                            previewOutcomeText.text +=
                                $"<color={operationStringStat}>{operationStat} {outcome.value}</color> {targetStat} to {characterSlots[0].icon.character.GetCharacterData().firstName}\n";
                            break;
                        case OutcomeData.OutcomeTarget.Assistant:
                            for (int i = 0; i < characterSlots.Count; i++)
                            {
                                var character = characterSlots[i];
                                if (character.isMandatory) continue;
                                if (character.icon == null) continue;
                                previewOutcomeText.text +=
                                    $"<color={operationStringStat}>{operationStat} {outcome.value}</color> {targetStat} to {character.icon.character.GetCharacterData().firstName}\n";
                            }

                            break;
                    }

                    break;
            }
        }

        private bool CheckTarget(ConditionSO conditionSO)
        {
            CharacterBehaviour leader = null;
            List<CharacterBehaviour> assistants = new List<CharacterBehaviour>();
            for (int j = 0; j < characterSlots.Count; j++)
            {
                if (characterSlots[j].isMandatory && characterSlots[j].icon != null)
                    leader = characterSlots[j].icon.character;
                else if (characterSlots[j].icon != null)
                    assistants.Add(characterSlots[j].icon.character);
            }

            var condition = false;
            switch (conditionSO.BaseCondition.target)
            {
                case OutcomeData.OutcomeTarget.Leader:
                    condition = ConditionSystem.CheckCharacterCondition(leader, conditionSO);
                    break;

                case OutcomeData.OutcomeTarget.Assistant:
                    condition = ConditionSystem.CheckCharacterCondition(leader, conditionSO);

                    break;

                case OutcomeData.OutcomeTarget.Crew:
                    condition = ConditionSystem.CheckCrewCondition(conditionSO);
                    break;

                case OutcomeData.OutcomeTarget.Gauge:
                    condition = ConditionSystem.CheckGaugeCondition(conditionSO);
                    break;

                case OutcomeData.OutcomeTarget.GaugeValue:
                    condition = ConditionSystem.CheckGaugeValueCondition(conditionSO);
                    break;

                case OutcomeData.OutcomeTarget.Ship:
                    condition = ConditionSystem.CheckSpaceshipCondition(conditionSO);
                    break;

                case OutcomeData.OutcomeTarget.None:
                    condition = true;
                    break;
            }

            return condition;
        }

        public void StartTask()
        {
            if (notification.IsStarted) return;
            if (!notification.Task.TaskType.Equals(SSTaskType.Timed))
            {
                if (!CanStartTask()) return;
                if (notification.Task.TaskType.Equals(SSTaskType.Compute))
                {
                    bool condition = CheckTarget(notification.Task.Conditions[0].Item1);
                    if (!condition) return;
                }
            }

            if (CharactersWorking()) return;
            notification.OnStart(characterSlots, gaugesOutcomes);
            taskStarted = true;
            GameManager.Instance.UIManager.ResetCharactersPreviewGauges();
            GameManager.Instance.SpaceshipManager.ApplyGaugeOutcomes(gaugesOutcomes);
            gaugesOutcomes.Clear();
            charOutcome.Clear();
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

            if ((notification.Task.TaskType.Equals(SSTaskType.Permanent) ||
                 notification.Task.TaskType.Equals(SSTaskType.Compute)) && !taskStarted) CloseNotification();
            TimeTickSystem.ModifyTimeScale(1);
            previewOutcomeText.text = null;
            characterSlots.Clear();
            dialogueLog.ClearDialogueLog();
            popupHelp.SetActive(false);
            separator.SetActive(false);
            GameManager.Instance.RefreshCharacterIcons();
            GameManager.Instance.taskOpened = false;
        }

        /// <summary>
        /// Close the notification
        /// </summary>
        public void CloseNotification()
        {
            TimeTickSystem.ModifyTimeScale(1);
            GameManager.Instance.RefreshCharacterIcons();
            notification.OnCancel();
        }

        /// <summary>
        /// Cancel the task
        /// </summary>
        public void CancelTask()
        {
            // Unassigned characters from the task and reset their mood
            foreach (var slot in characterSlots)
            {
                if (slot.icon != null) slot.icon.ResetTransform();
                slot.icon.character.StopTask();
                slot.icon.character.IncreaseMood(-10);
                slot.ClearCharacter();
                slot.gameObject.SetActive(false);
            }

            notification.IsCancelled = true;
            previewOutcomeText.text = null;
            characterSlots.Clear();
            GameManager.Instance.RefreshCharacterIcons();
            GameManager.Instance.taskOpened = false;
            separator.SetActive(false);
            Appear(false);
            CloseNotification();
        }

        private bool CharactersWorking()
        {
            List<CharacterBehaviour> characters = new();
            foreach (var character in characterSlots)
            {
                if (character.icon != null && character.icon.character.IsWorking())
                {
                    characters.Add(character.icon.character);
                }
            }

            if (characters.Count > 0)
            {
                warningUI.CharacterWarning(characters);
                return true;
            }

            return false;
        }

        public void Appear(bool state)
        {
            animator.SetBool("Appear", state);
        }

        public void SetLeader(CharacterIcon leader)
        {
            characterSlots[0].SetupIcon(leader);
            leader.SetupIconValues();
        }

        public void SetAssistant(CharacterIcon assistant)
        {
            for (int i = 0; i < characterSlots.Count; i++)
            {
                var slot = characterSlots[i];
                if (slot.isMandatory) continue;
                if (slot.icon != null) continue;
                slot.SetupIcon(assistant);
                assistant.SetupIconValues();
                break;
            }
        }

        private IEnumerator DisplayText(TextMeshProUGUI text, string textToDisplay, float speed)
        {
            int letterIndex = 0;
            string tempText = "";
            string tagBuffer = "";
            bool bufferTag = false;
            text.text = tempText;
            text.enableAutoSizing = false;
            text.fontSize = 20;

            while (letterIndex < textToDisplay.Length)
            {
                if (text.isTextOverflowing) text.enableAutoSizing = true;

                //If tag-beginning character is parsed, start buffering the tag
                if (textToDisplay[letterIndex] == '<')
                    bufferTag = true;
                //If tag-ending character is parsed, buffer the tag ending character and concatenate with text
                if (textToDisplay[letterIndex] == '>')
                {
                    bufferTag = false;
                    tagBuffer += textToDisplay[letterIndex];
                    tempText = string.Concat(tempText, tagBuffer);
                    letterIndex++;
                    tagBuffer = ""; //Reset buffer in case of multiple tags.
                    continue;
                }

                //If buffering tag, write to buffer instead of tempText and skip waiting
                if (bufferTag)
                {
                    tagBuffer += textToDisplay[letterIndex];
                    letterIndex++;
                    continue;
                }

                yield return new WaitForSeconds(speed);
                tempText += textToDisplay[letterIndex];
                text.text = tempText;
                letterIndex++;
            }

            text.text += "\n\n";
        }
    }
}