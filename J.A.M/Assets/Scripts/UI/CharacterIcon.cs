using System;
using CharacterSystem;
using Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class CharacterIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private CharacterUI baseParentScript;
        private Transform parentAfterDrag;
        private CharacterUI parentScript;
        [SerializeField] private Image image;
        [SerializeField] private Image characterIcon;
        [SerializeField] private Image currentTaskImage;
        [SerializeField] private TextMeshProUGUI characterName;
        [NonSerialized] public CharacterBehaviour character;
        private Animator animator;
    
        public void Initialize(CharacterBehaviour c, CharacterUI script)
        {
            baseParentScript = script;
            character = c;
            characterIcon.sprite = character.GetCharacterData().characterIcon;
            parentScript = script;
            characterName.text = character.GetCharacterData().firstName;
            animator = GetComponent<Animator>();
        }
        public void ResetTransform()
        {
            transform.SetParent(baseParentScript.transform);
            transform.position = baseParentScript.transform.position;
            transform.localScale = baseParentScript.transform.localScale;
            baseParentScript.icon = this;
        }
    
        public void OnBeginDrag(PointerEventData eventData)
        {
            parentAfterDrag = transform.parent;
            parentScript.ClearCharacter();
            image.raycastTarget = false;
            transform.SetParent(GameManager.Instance.UIManager.canvas.transform);
            animator.SetBool("Selected", true);
        }

        public void OnDrag(PointerEventData eventData)
        {
            //Check si character.IsWorking(), si oui, return ou ResetTransform()
            transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            transform.SetParent(parentAfterDrag);
            transform.localPosition = Vector3.zero;
            animator.SetBool("Selected", false);
            parentScript.icon = this;
            image.raycastTarget = true;
        }

        private void AssignTask(Task t)
        {
            currentTaskImage.sprite = t.Icon;
            currentTaskImage.enabled = true;
        }

        public void RefreshIcon()
        {
            if (character.IsWorking())
            {
                AssignTask(character.GetTask());
            }
            else
            {
                StopTask();
            }
        }

        private void StopTask()
        {
            currentTaskImage.enabled = false;
        }

        public void SetupIcon(Transform parent, CharacterUI script)
        {
            parentAfterDrag = parent;
            transform.localPosition = Vector3.zero;
            parentScript = script;
        }
    }
}
