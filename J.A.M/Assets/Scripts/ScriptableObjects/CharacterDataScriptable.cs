using System;
using UnityEngine;

[CreateAssetMenu(menuName = "DataScriptables", fileName = "Character")]
public class CharacterDataScriptable : ScriptableObject
{
    [Header("Statistics")] public string ID;
    public int baseVolition;
    public int baseMood;
    public Relationship[] relationships;
    public TraitsData.Traits traits;
    public Sprite characterIcon;
    public Sprite characterBody;
    public string firstName, lastName;
    [TextArea(5, 10)] public string description;

    [ContextMenu("Initialize GUID")]
    public void Initialize()
    {
        ID = Guid.NewGuid().ToString();
    }

    [Serializable]
    public struct Relationship
    {
        public CharacterDataScriptable character;
        public int friendshipValue;
        public int loveValue;
    }
}