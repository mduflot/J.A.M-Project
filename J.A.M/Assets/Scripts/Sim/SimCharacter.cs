using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimCharacter : MonoBehaviour
{
    [HideInInspector] public bool moveToTarget;
    [HideInInspector] public bool isIdle = true;
    [HideInInspector] public uint currentRoomDoor = 3;
    public Stack<uint> doorPath = new();

    [SerializeField] private SimRoom currentRoom;

    private void Update()
    {
        Simulate();
        
        if (Input.GetKey(KeyCode.Space) && doorPath.Count == 0)
        {
            moveToTarget = true;
            isIdle = false;
            uint targetID = (uint)Random.Range(1, 13);
            SimPathing.CreatePath(SimPathing.FindDoorByID(currentRoomDoor), SimPathing.FindDoorByID(targetID), this);
        }
    }

    //todo : add movment towards furniture
    //todo : scale w/ time tick
    //todo : add fade between doors -> improve with anim
    //todo : implem vrai utilisation
    //todo : assurer que tout marche avec plusieurs persos
    //todo : stress test
    public void Simulate()
    {
        if (isIdle)
        {
            IdleInRoom();
            return;
        }
        
        //Add scaling based on TimeTickSystem

        SimDoor door = SimPathing.FindDoorByID(currentRoomDoor);
        if ((door.transform.position - transform.position).magnitude < .1f)
        {
            FadeToNextDoor();
        }
        else
        {
            MoveToCurrentDoor();
        }
    }

    private void FadeToNextDoor()
    {
        //GetComponent<SpriteRenderer>().enabled = (doorPath.Count + 1) % 2 == 0;
        if (doorPath.Count == 0)
        {
            moveToTarget = false;
            isIdle = true;
            GetComponent<SpriteRenderer>().enabled = true;
            return;
        }
        currentRoomDoor = doorPath.Pop();
        currentRoom = SimPathing.FindRoomByDoorID(currentRoomDoor);
    }

    private void MoveToCurrentDoor()
    {
        SimDoor door = SimPathing.FindDoorByID(currentRoomDoor);
        Vector3 pos = transform.position;
        pos = Vector3.Lerp(pos, door.transform.position, .0025f);
        transform.position = pos;
    }

    private void IdleInRoom()
    {
        if (currentRoom == null) return;
        
        Vector3 pos = transform.position;
        pos = Vector3.Lerp(pos, currentRoom.transform.position, .25f);
        transform.position = pos;
    }
}
