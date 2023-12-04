using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableTuple<T1, T2, T3>
{
    [SerializeField] public T1 Job;
    [SerializeField] public T2 Positive;
    [SerializeField] public T3 Negative;
}
