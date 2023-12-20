using CharacterSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class CharacterUI : MonoBehaviour, IDropHandler, IPointerDownHandler
    {
        public CharacterBehaviour character;
        public CharacterIcon icon;
        public CharacterIcon defaultIcon;
        public Image moodGauge;
        public Image volitionGauge;
        public Image previewMoodGauge;

        [SerializeField] private Speaker speaker;
        private float clickTime;
        private uint clicked;
        private float clickDelay = 0.5f;
        public void Initialize(CharacterBehaviour c)
        {
            character = c;
            c.speaker = speaker;
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

        public virtual void OnDrop(PointerEventData eventData)
        {
            if (defaultIcon != eventData.pointerDrag.GetComponent<CharacterIcon>()) return;
            GameObject dropped = eventData.pointerDrag;
            icon = dropped.GetComponent<CharacterIcon>();
            icon.SetupIcon(transform, this);
            icon.transform.localScale = transform.localScale;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if(!GameManager.Instance.taskOpened) return;
            clicked++;
            if (clicked == 1) clickTime = Time.time;
            if (clicked > 1 && Time.time - clickTime < clickDelay)
            {
                clicked = 0;
                clickTime = 0;
                
            }
            else if (clicked >= 2 && Time.time - clickTime > clickDelay) clicked = 0;
        }
    }
}