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
        public GaugeUI[] gauges;
        private Dictionary<SystemType, GaugeUI> gaugeReferences = new();
        public Transform charactersUIParent;
        public List<CharacterUI> charactersUI;
        public List<CharacterIcon> characterIcons = new();
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

        public void UpdateGauges(SystemType systemType, float value, float previewValue)
        {
            gaugeReferences[systemType].UpdateGauge(value, previewValue);
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
                var valueToAdd = 0.0f;
                foreach (var outcome in gaugesOutcomes)
                {
                    if (outcome.gauge == gauge.systemType) valueToAdd += outcome.value;
                }
                
                gauge.PreviewOutcomeGauge(valueToAdd);
            }
        }

        public void ResetPreviewGauges()
        {
            foreach (var gauge in gauges)
            {
                gauge.ResetPreviewGauge();
            }
        }

        public void UpdateCharacterGauges()
        {
            foreach (var charUi in charactersUI)
            {
                charUi.UpdateIconDisplay();
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
                c.PreviewMoodGauge(value);
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
