using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimRoom : MonoBehaviour
{
    public float roomXOffset = 15f;
    public float roomYOffset = 5f;
    public SimDoor[] roomDoors;
    public RoomType roomType;
    public List<SimCharacter> presentCharacters;
}
