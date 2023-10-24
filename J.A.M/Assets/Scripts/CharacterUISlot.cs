using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterUISlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        CharacterIcon icon = dropped.GetComponent<CharacterIcon>();
        icon.parentAfterDrag = transform;
    }
}
