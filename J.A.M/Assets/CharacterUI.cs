using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUI : MonoBehaviour
{
    public CharacterDataScriptable character;
    public CharacterIcon icon;

    public void Initialize(CharacterDataScriptable characterData)
    {
        character = characterData;
        icon.image.sprite = character.characterIcon;
    }
}
