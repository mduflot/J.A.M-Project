using System;
using UnityEngine;

[Serializable]
public struct Room
{
    public RoomType type;
    public Transform transform;
    public Transform doorPosition;
    public Furniture[] roomObjects;
}