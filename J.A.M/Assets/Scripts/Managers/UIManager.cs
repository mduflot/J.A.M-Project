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
    
        [Serializable] 
        public struct Gauges
        {
            [FormerlySerializedAs("system")] public SystemType systemType;
            public Image gauge;
            public Image previewGauge;
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
            gaugeReferences[systemType].gauge.fillAmount = value/20;
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
                    valueToAdd += gauge.gauge.fillAmount * 20;
                    gauge.previewGauge.fillAmount = valueToAdd / 20;
                }
                else
                {
                    gauge.previewGauge.fillAmount = gauge.gauge.fillAmount;
                    gauge.gauge.fillAmount += valueToAdd / 20;
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
