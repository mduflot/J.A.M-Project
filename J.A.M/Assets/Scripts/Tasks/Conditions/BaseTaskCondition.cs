using System;
using UnityEngine;

public class BaseTaskCondition : ScriptableObject
{
    public virtual bool Condition()
    {
        return false;
    }
}
