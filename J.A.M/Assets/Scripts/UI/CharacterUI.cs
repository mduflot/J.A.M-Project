using CharacterSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class CharacterUI : MonoBehaviour, IDropHandler
    {
        public CharacterBehaviour character;
        public CharacterIcon icon;
        public CharacterIcon defaultIcon;
        public Image moodGauge;
        public Image volitionGauge;
        public Image previewMoodGauge;

        [SerializeField] private Speaker speaker;
        
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
            Debug.Log("On me drop dessus");
        }
        
       
    }
}