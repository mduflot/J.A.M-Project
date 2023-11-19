using UnityEngine;

[System.Serializable]
public class SerializableTuple<T1, T2>
{
    [SerializeField] public T1 Item1;
    [SerializeField] public T2 Item2;
}

[System.Serializable]
public class SerializableTuple<T1, T2, T3>
{
    [SerializeField] public T1 Item1;
    [SerializeField] public T2 Item2;
    [SerializeField] public T3 Item3;
}