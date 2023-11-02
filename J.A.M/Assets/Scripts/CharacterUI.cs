using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterUI : MonoBehaviour, IDropHandler
{
    public CharacterDataScriptable character;
    public CharacterIcon icon;

    public void Initialize(CharacterDataScriptable characterData)
    {
        character = characterData;
        icon.Initialize(character, this);
    }
    
    public void ClearCharacter()
    {
        icon = null;
    }

    public void SetCharacter(CharacterIcon icon)
    {
        this.icon = icon;
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        
        Debug.Log(transform.name + " récupère");
        GameObject dropped = eventData.pointerDrag;
        icon = dropped.GetComponent<CharacterIcon>();
        icon.parentAfterDrag = transform;
        icon.parentScript = this;
    }
}
