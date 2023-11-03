using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public CharacterUI baseParentScript;
    public Transform parentAfterDrag;
    public CharacterUI parentScript;
    [SerializeField] private Image image;
    public CharacterDataScriptable character;
    
    public void Initialize(CharacterDataScriptable c, CharacterUI script)
    {
        baseParentScript = script;
        character = c;
        image.sprite = c.characterIcon;
        parentScript = script;
    }

    public void ResetTransform()
    {
        transform.SetParent(baseParentScript.transform);
        transform.position = baseParentScript.transform.position;
        baseParentScript.icon = this;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        parentScript.ClearCharacter();
        image.raycastTarget = false;
        transform.SetParent(GameManager.Instance.UIManager.canvas.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        //transform.position = parentAfterDrag.position;
        parentScript.icon = this;
        image.raycastTarget = true;
    }
}
