using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseTaskOutcome : ScriptableObject
{
    public List<CharacterBehaviour> leaderCharacters = new List<CharacterBehaviour>();
    public List<CharacterBehaviour> assistantCharacters = new List<CharacterBehaviour>();
    public virtual void Outcome()
    {
        
    }
}
