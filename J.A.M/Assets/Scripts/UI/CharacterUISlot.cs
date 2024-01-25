using Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class CharacterUISlot : CharacterUI
    {
        public bool isMandatory;

        [SerializeField] private Image image;
        [SerializeField] private Transform iconParent;
        [SerializeField] private Sprite mandatorySprite;
        [SerializeField] private Sprite assistantSprite;
        
        private TaskUI taskUI;

        public void SetupSlot(bool mandatory, TaskUI task)
        {
            image.sprite = mandatory ? mandatorySprite : assistantSprite;
            transform.localScale = mandatory ? Vector3.one : Vector3.one / 1.15f;
            isMandatory = mandatory;
            taskUI = task;
        }

        public override void OnDrop(PointerEventData eventData)
        {
            CharacterIcon dropped = eventData.pointerDrag.GetComponent<CharacterIcon>();
            SetupIcon(dropped);
        }

        public void SetupIcon(CharacterIcon c)
        {
            if (icon != null)
            {
                icon.ResetTransform();
                icon.RefreshIcon();
                ClearCharacter();
            }
            
            icon = c;
            icon.SetupIcon(iconParent, this);
            icon.transform.localScale = transform.localScale;
            if (taskUI.isActiveAndEnabled) taskUI.UpdatePreview();
        }
    }
}