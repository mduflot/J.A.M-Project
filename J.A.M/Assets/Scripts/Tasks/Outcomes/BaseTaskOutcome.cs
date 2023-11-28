using System.Collections.Generic;
using UnityEngine;

public class BaseTaskOutcome : ScriptableObject
{
    public List<CharacterBehaviour> leaderCharacters = new();
    public List<CharacterBehaviour> assistantCharacters = new();

    public virtual void Outcome()
    {
    }
}