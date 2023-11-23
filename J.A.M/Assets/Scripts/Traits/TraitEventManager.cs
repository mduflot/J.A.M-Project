using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraitEventManager : MonoBehaviour
{
    //text event -> char gains x (text overridable)
    //outcome none mais garde text
    
    //jauge + 
    
    //jauge -
    
    //ajouter trait a personnage (si trait non possédé)
    
    //retirer un trait a personnage (si trait possédé)
    
    //moral +
    public static void IncreaseMood(CharacterTraitEventArgs e)
    {
        if (e.characterRole != CharacterRole.Leader) return;
        var tempCharBehaviour = new CharacterBehaviour();
        
        tempCharBehaviour.IncreaseMood(e.modifierValue);
    }
    
    //moral -
    public static void DecreaseMood(CharacterTraitEventArgs e)
    {
        if (e.characterRole != CharacterRole.Leader) return;
        var tempCharBehaviour = new CharacterBehaviour();
        
        tempCharBehaviour.IncreaseMood(-e.modifierValue);
    }
    
    //volition +
    public static void IncreaseVolition(CharacterTraitEventArgs e)
    {
        if (e.characterRole != CharacterRole.Leader) return;
        
        var tempCharBehaviour = new CharacterBehaviour();
        
        tempCharBehaviour.IncreaseVolition(e.modifierValue);
    }
    
    //volition -
    public static void DecreaseVolition(CharacterTraitEventArgs e)
    {
        if (e.characterRole != CharacterRole.Leader) return;
        var tempCharBehaviour = new CharacterBehaviour();
        
        tempCharBehaviour.IncreaseVolition(-e.modifierValue);
    }

    [System.Serializable]
    public struct CharacterTraitEventArgs
    {
        public CharacterRole characterRole;
        public float modifierValue;
    }
    
    public enum CharacterRole
    {
        Leader,
        Assistant
    }
}
