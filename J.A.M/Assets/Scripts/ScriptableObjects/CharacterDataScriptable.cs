using System;
using UnityEngine;

[CreateAssetMenu(menuName = "DataScriptables", fileName = "Character")]
public class CharacterDataScriptable : ScriptableObject
{
    [Header("Statistics")] 
    public int baseVolition;
    public int baseMood;
    public Relationship[] relationships;
    //public Traits[] traits; 
    public Sprite characterIcon;
    public string firstName, lastName;

    [Serializable]
    public struct Relationship
    {
        public CharacterDataScriptable character;
        public int friendshipValue;
        public int loveValue;
    }
    
}
