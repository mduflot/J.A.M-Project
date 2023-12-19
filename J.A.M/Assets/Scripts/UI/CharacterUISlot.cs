using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class CharacterUISlot : CharacterUI
    {
        public bool isMandatory;
        [SerializeField] private UnityEngine.UI.Image image;
        [SerializeField] private Transform iconParent;
        [SerializeField] private Sprite mandatorySprite;
        [SerializeField] private Sprite assistantSprite;

        public void SetupSlot(bool mandatory)
        {
            image.sprite = mandatory ? mandatorySprite : assistantSprite;
            transform.localScale = mandatory ? Vector3.one : Vector3.one / 1.15f;
            isMandatory = mandatory;
        }
        
        public override void OnDrop(PointerEventData eventData)
        {
            if (icon != null) return;
            GameObject dropped = eventData.pointerDrag;
            icon = dropped.GetComponent<CharacterIcon>();
            icon.SetupIcon(iconParent, this);
            icon.transform.localScale = transform.localScale;
        }
    }
}
