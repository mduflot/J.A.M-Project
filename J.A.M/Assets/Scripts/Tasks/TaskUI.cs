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
        [Header("Task")] [SerializeField] private TextMeshProUGUI titleText;
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

        [Header("Dialogues")] [SerializeField] private GameObject dialogueContainer;

        [Header("Values")] [SerializeField] private float timeLeft;
        [SerializeField] private float duration;

        private Notification notification;
        private List<CharacterUISlot> characterSlots = new();
        private bool taskStarted;
        private Animator animator;
        private List<GaugesOutcome> gaugesOutcomes = new();
        private List<CharacterOutcome> charOutcome = new();

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

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void Initialize(Notification n, CharacterIcon icon = null, bool needToDisplay = true,
            TaskLog taskLog = null)
        {
            notification = n;
            titleText.text = notification.Task.Name;
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
        }

        public void DisplayTaskInfo(Notification n)
        {
            notification = n;
            titleText.text = notification.Task.Name;
            StartCoroutine(DisplayText(descriptionText, notification.Task.Description, 0.02f));
            StartCoroutine(DisplayText(previewOutcomeText, notification.Task.previewText, 0.02f));
            duration = notification.Task.Duration;
            durationText.SetText(TimeTickSystem.GetTicksAsTime((uint)duration));

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
            separator.SetActive(true);
            GameManager.Instance.taskOpened = true;
            Appear(true);
        }

        public void Update()
        {
            if (!animator.GetBool("Appear")) return;
            if (!taskStarted)
            {
                List<GaugesOutcome> gaugeOutcomes = new List<GaugesOutcome>();
                List<CharacterOutcome> characterOutcomes = new List<CharacterOutcome>();

                for (int index = 0; index < notification.Task.Conditions.Count; index++)
                {
                    bool condition = CheckTarget(notification.Task.Conditions[index].Item1);

                    if ((!condition && notification.Task.TaskType == SSTaskType.Compute) || (characterSlots[0].icon == null && notification.Task.TaskType == SSTaskType.Untimed))
                    {
                        previewOutcomeText.text = "Condition not met";
                        startButton.GetComponentInChildren<Button>().interactable = false;
                    }
                    else
                    {
                        previewOutcomeText.text = null;
                        startButton.GetComponentInChildren<Button>().interactable = true;
                    }

                    if (condition)
                    {
                        previewOutcomeText.text = null;
                        if (notification.Task.TaskType != SSTaskType.Compute)
                        {
                            if (characterSlots[0].icon != null)
                                previewOutcomeText.text =
                                    $"{characterSlots[0].icon.character.GetCharacterData().name} {notification.Task.Conditions[index].Item2}\n";
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
                                }

                                //break;
                            }
                        }

                        notification.Task.previewText = previewOutcomeText.text;
                        break;
                    }
                }

                var assistantCharacters = characterSlots.Count(slot => !slot.isMandatory && slot.icon != null);
                GameManager.Instance.UIManager.PreviewOutcomeGauges(gaugesOutcomes);
                foreach (var c in characterSlots)
                {
                    if (c.icon != null)
                        characterOutcomes.Add(new CharacterOutcome(c.icon.character,
                            -GameManager.Instance.SpaceshipManager.moodLossOnTaskStart));
                }

                GameManager.Instance.UIManager.CharacterPreviewGauges(charOutcome);
                charOutcome = characterOutcomes;
                gaugesOutcomes = gaugeOutcomes;
                duration = assistantCharacters > 0
                    ? notification.Task.Duration /
                      (Mathf.Pow(assistantCharacters + 1, notification.Task.HelpFactor))
                    : notification.Task.Duration;

                durationText.text = TimeTickSystem.GetTicksAsTime((uint)(duration * TimeTickSystem.ticksPerHour));
                var button = startButton.GetComponentInChildren<Button>();
                button.Select();
            }
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
            switch (outcome.OutcomeType)
            {
                case OutcomeData.OutcomeType.Gauge:
                    if (outcome.OutcomeOperation == OutcomeData.OutcomeOperation.Sub)
                        operation = "-";
                    switch (outcome.OutcomeTargetGauge)
                    {
                        case SystemType.Trajectory:
                            previewOutcomeText.text +=
                                $"<color=lightblue>{traits} {operation} {outcome.value} {outcome.OutcomeTargetGauge}</color>\n";
                            break;
                        case SystemType.Food:
                            previewOutcomeText.text +=
                                $"<color=green>{traits} {operation} {outcome.value} {outcome.OutcomeTargetGauge}</color>\n";
                            break;
                        case SystemType.Hull:
                            previewOutcomeText.text +=
                                $"<color=red>{traits} {operation} {outcome.value} {outcome.OutcomeTargetGauge}</color>\n";
                            break;
                        case SystemType.Power:
                            previewOutcomeText.text +=
                                $"<color=yellow>{traits} {operation} {outcome.value} {outcome.OutcomeTargetGauge}</color>\n";
                            break;
                    }

                    var value = outcome.OutcomeOperation == OutcomeData.OutcomeOperation.Add
                        ? outcome.value
                        : -outcome.value;
                    
                    if (notification.Task.TaskType == SSTaskType.Permanent)
                    {
                        for (int index = 0; index < GameManager.Instance.SpaceshipManager.systems.Length; index++)
                        {
                            var system = GameManager.Instance.SpaceshipManager.systems[index];
                            if (system.type == outcome.OutcomeTargetGauge)
                            {
                                value -= system.decreaseSpeed * notification.Task.Duration;
                                break;
                            }
                        }
                    }
                    
                    gaugeOutcomes.Add(new GaugesOutcome(outcome.OutcomeTargetGauge,
                        value));

                    break;
                case OutcomeData.OutcomeType.GaugeVolition:
                    if (characterSlots[0].icon == null) break;
                    if (outcome.OutcomeOperation == OutcomeData.OutcomeOperation.Sub)
                        operation = "-";
                    var valueVolition = characterSlots[0].icon.character.GetVolition();
                    switch (outcome.OutcomeTargetGauge)
                    {
                        case SystemType.Trajectory:
                            previewOutcomeText.text +=
                                $"<color=lightblue>{traits} Volition: {operation} {valueVolition.ToString("F2")} {outcome.OutcomeTargetGauge}</color>\n";
                            break;
                        case SystemType.Food:
                            previewOutcomeText.text +=
                                $"<color=green>{traits} Volition: {operation} {valueVolition.ToString("F2")} {outcome.OutcomeTargetGauge}</color>\n";
                            break;
                        case SystemType.Hull:
                            previewOutcomeText.text +=
                                $"<color=red>{traits} Volition: {operation} {valueVolition.ToString("F2")} {outcome.OutcomeTargetGauge}</color>\n";
                            break;
                        case SystemType.Power:
                            previewOutcomeText.text +=
                                $"<color=yellow>{traits} Volition: {operation} {valueVolition.ToString("F2")} {outcome.OutcomeTargetGauge}</color>\n";
                            break;
                    }

                    if (characterSlots[0].icon.character.GetMood() < characterSlots[0].icon.character.GetVolition())
                    {
                        valueVolition /= 2;
                        previewOutcomeText.text +=
                            $"<color=#ff00ffff>Bad Mood: - {valueVolition.ToString("F2")}</color>\n";
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
                                volition -= system.decreaseSpeed * notification.Task.Duration;
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
                    switch (outcome.OutcomeTarget)
                    {
                        case OutcomeData.OutcomeTarget.Crew:
                            previewOutcomeText.text +=
                                $"{outcome.OutcomeOperation} {outcome.value} {outcome.OutcomeTargetStat} to Crew\n";
                            break;
                        case OutcomeData.OutcomeTarget.Leader:
                            previewOutcomeText.text +=
                                $"{outcome.OutcomeOperation} {outcome.value} {outcome.OutcomeTargetStat} to {characterSlots[0].icon.character.GetCharacterData().firstName}\n";
                            break;
                        case OutcomeData.OutcomeTarget.Assistant:
                            for (int i = 0; i < characterSlots.Count; i++)
                            {
                                var character = characterSlots[i];
                                if (character.isMandatory) continue;
                                if (character.icon == null) continue;
                                previewOutcomeText.text +=
                                    $"{outcome.OutcomeOperation} {outcome.value} {outcome.OutcomeTargetStat} to {character.icon.character.GetCharacterData().firstName}\n";
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
                    condition = ConditionSystem.CheckCharacterCondition(leader, assistants.ToArray(), conditionSO);
                    break;

                case OutcomeData.OutcomeTarget.Assistant:
                    condition = ConditionSystem.CheckCharacterCondition(leader, assistants.ToArray(), conditionSO);

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
            GameManager.Instance.UIManager.ResetPreviewGauges();
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
            separator.SetActive(false);
            GameManager.Instance.UIManager.ResetPreviewGauges();
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

        public void CancelTask()
        {
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

        private void Appear(bool state)
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

            while (letterIndex < textToDisplay.Length)
            {
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
        }
    }
}