using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterUISlot : CharacterUI
{
    public bool isMandatory;
    public UnityEngine.UI.Image image;

    private void Start()
    {
        image = GetComponent<UnityEngine.UI.Image>();
    }
    
}
