using CharacterSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class CharacterUI : MonoBehaviour, IDropHandler
    {
        [Header("Static Data")]
        public CharacterBehaviour character;
        public CharacterIcon icon;
        public CharacterIcon defaultIcon;
        
        [Header("Data")]
        [SerializeField] private Image moodGauge;
        [SerializeField] private Slider volitionGauge;
        public Image previewMoodGauge;
        
        [Space]
        
        public Image moodArrow;
        [SerializeField] private Sprite redArrow;
        [SerializeField] private Sprite greenArrow;
        
        [Space]
        
        [SerializeField] private Image moodIcon;
        [SerializeField] private Sprite happyIcon;
        [SerializeField] private Sprite sadIcon;

        public void Initialize(CharacterBehaviour c)
        {
            character = c;
            icon.Initialize(character, this);
        }

        public void ClearCharacter()
        {
            icon = null;
        }

        public void SetCharacter(CharacterIcon i)
        {
            icon = i;
        }

        public void UpdateIconDisplay()
        {
            moodGauge.fillAmount = character.GetMood() / character.GetMaxMood();
            volitionGauge.value = character.GetBaseVolition() / character.GetMaxMood();
            
            moodIcon.sprite = character.GetMood() < character.GetBaseVolition() ? sadIcon : happyIcon;
            moodArrow.sprite = character.IsMoodIncreasing() ? greenArrow : redArrow;
        }

        public void PreviewMoodGauge(float value)
        {
            if (value < 0)
            {
                previewMoodGauge.fillAmount = character.GetMood() / character.GetMaxMood();
                moodGauge.fillAmount =
                    (character.GetMood() - GameManager.Instance.SpaceshipManager.moodLossOnTaskStart) /
                    character.GetMaxMood();
            }
            else
            {
                value += moodGauge.fillAmount * character.GetMaxMood();
                previewMoodGauge.fillAmount = value / character.GetMaxMood();
                moodGauge.fillAmount = character.GetMood() / character.GetMaxMood();
            }
        }

        public virtual void OnDrop(PointerEventData eventData)
        {
            if (defaultIcon != eventData.pointerDrag.GetComponent<CharacterIcon>()) return;
            GameObject dropped = eventData.pointerDrag;
            icon = dropped.GetComponent<CharacterIcon>();
            icon.SetupIcon(transform, this);
            icon.transform.localScale = transform.localScale;
        }
    }
}