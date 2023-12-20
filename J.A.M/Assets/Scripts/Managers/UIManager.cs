using System;
using System.Collections.Generic;
using CharacterSystem;
using Tasks;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        public Gauges[] gauges;
        private Dictionary<SystemType, Gauges> gaugeReferences = new();
        public Transform charactersUIParent;
        public List<CharacterUI> charactersUI;
        private List<CharacterIcon> characterIcons = new();
        public CharacterUI characterUIPrefab;
        public Transform taskNotificationParent;
        public Transform taskParent;
        public TaskUI taskUI;
        public Canvas canvas;
        public TextMeshProUGUI date;
        [SerializeField] private Sprite redArrow;
        [SerializeField] private Sprite greenArrow;
        public CharacterInfoUI characterInfoUI;
    
        [Serializable] 
        public struct Gauges
        {
            [FormerlySerializedAs("system")] public SystemType systemType;
            public Image gauge;
            public Image previewGauge;
            public Image arrow;
        }
    
        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            for (int i = 0; i < gauges.Length; i++)
            {
                gaugeReferences.Add(gauges[i].systemType, gauges[i]);
            }

            foreach (var character in GameManager.Instance.SpaceshipManager.characters)
            {
                var ui = Instantiate(characterUIPrefab, charactersUIParent);
                ui.Initialize(character);
                charactersUI.Add(ui);
                characterIcons.Add(ui.icon);
            }
        }

        public void UpdateGauges(SystemType systemType, float value)
        {
            gaugeReferences[systemType].gauge.fillAmount = value/50;
            gaugeReferences[systemType].arrow.sprite = redArrow;
        }

        public void UpdateInGameDate(string newDate)
        {
            date.text = newDate;
        }

        public void RefreshCharacterIcons()
        {
            foreach (var icon in characterIcons)
            {
                icon.RefreshIcon();
            }
        }

        public void PreviewOutcomeGauges(List<TaskUI.GaugesOutcome> gaugesOutcomes)
        {
            foreach (var gauge in gauges)
            {
                var valueToAdd = 0f;
                foreach (var outcome in gaugesOutcomes)
                {
                    if (outcome.gauge == gauge.systemType) valueToAdd += outcome.value;
                }

                if (valueToAdd > 0)
                {
                    valueToAdd += gauge.gauge.fillAmount * 50;
                    gauge.previewGauge.fillAmount = valueToAdd / 50;
                    gauge.arrow.sprite = greenArrow;
                }
                else
                {
                    gauge.previewGauge.fillAmount = gauge.gauge.fillAmount;
                    gauge.gauge.fillAmount += valueToAdd / 50;
                    gauge.arrow.sprite = redArrow;
                }
            }
        }

        public void ResetPreviewGauges()
        {
            foreach (var gauge in gauges)
            {
                gauge.previewGauge.fillAmount = 0;
            }
        }

        public void UpdateCharacterGauges()
        {
            foreach (var charUi in charactersUI)
            {
                charUi.moodGauge.fillAmount = charUi.character.GetMood() / charUi.character.GetMaxMood();
                charUi.volitionGauge.fillAmount = charUi.character.GetVolition() / charUi.character.GetMaxMood();
            }
        }

        
        //TODO : Modifier une fois qu'on aura les traits et des bonus de Mood
        public void CharacterPreviewGauges(List<TaskUI.CharacterOutcome> characters)
        {
            foreach (var c in charactersUI)
            {
                var value = 0f;
                foreach (var character in characters)
                {
                    if (c.character == character.character) value += character.value;
                }
                if(value < 0)
                {
                    c.previewMoodGauge.fillAmount = c.character.GetMood() / c.character.GetMaxMood();
                    c.moodGauge.fillAmount = (c.character.GetMood() - GameManager.Instance.SpaceshipManager.moodLossOnTaskStart) /
                                             c.character.GetMaxMood();
                }
                else
                {
                    value += c.moodGauge.fillAmount * c.character.GetMaxMood();
                    c.previewMoodGauge.fillAmount = value / c.character.GetMaxMood();
                    c.moodGauge.fillAmount = c.character.GetMood() / c.character.GetMaxMood();
                }
            }
        }

        public void ResetCharactersPreviewGauges()
        {
            foreach (var charUI in charactersUI)
            {
                charUI.previewMoodGauge.fillAmount = 0;
            }
        }
        
        public CharacterUI GetCharacterUI(CharacterBehaviour c)
        {
            foreach (var charUI in charactersUI)
            {
                if (charUI.character == c) return charUI;
            }

            return null;
        }
        
    }
}
