using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount > 0) return;
        GameObject dropped = eventData.pointerDrag;
        DraggableCharacter draggableCharacter = dropped.GetComponent<DraggableCharacter>();
        draggableCharacter.parentAfterDrag = transform;
    }
}
