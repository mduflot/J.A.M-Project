using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterUISlot : MonoBehaviour, IDropHandler
{
    public CharacterIcon character;
    public bool isMandatory;
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        CharacterIcon icon = dropped.GetComponent<CharacterIcon>();
        character = icon;
        icon.parentAfterDrag = transform;
    }
}
