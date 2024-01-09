using System;
using System.Collections.Generic;
using UnityEditor.AddressableAssets.Build.BuildPipelineTasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimCharacter : MonoBehaviour
{
    [HideInInspector] public bool isIdle = true;
    [HideInInspector] public uint currentRoomDoor = 3;
    public Stack<uint> doorPath = new();

    public SimRoom currentRoom;

    private void Update()
    {
        Simulate();
        if(Input.GetKey(KeyCode.Space))
            SendToRoom(RoomType.Docking);
        if(Input.GetKey(KeyCode.Q))
            SendToRoom(RoomType.Flight);
        if(Input.GetKey(KeyCode.W))
            SendToRoom(RoomType.Common);
    }

    //todo : add movment towards furniture
    //todo : scale w/ time tick
    //todo : assurer que tout marche avec plusieurs persos
    //todo : stress test
    private void Simulate()
    {
        currentRoom = SimPathing.FindRoomByDoorID(currentRoomDoor);
        if (isIdle)
        {
            IdleInRoom();
            return;
        }
        
        //Add scaling based on TimeTickSystem

        SimDoor door = SimPathing.FindDoorByID(currentRoomDoor);
        if ((door.transform.position - transform.position).magnitude < .1f)
            FadeToNextDoor();
        else
            MoveToCurrentDoor();
    }

    private void FadeToNextDoor()
    {
        if (doorPath.Count == 0)
        {
            isIdle = true;
            GetComponent<SpriteRenderer>().enabled = true;
            return;
        }
        
        uint nextDoor = doorPath.Pop();

        GetComponent<SpriteRenderer>().enabled = SimPathing.FindRoomByDoorID(nextDoor).roomType == currentRoom.roomType;
        
        currentRoomDoor = nextDoor;
        currentRoom = SimPathing.FindRoomByDoorID(currentRoomDoor);
    }

    private void MoveToCurrentDoor()
    {
        SimDoor door = SimPathing.FindDoorByID(currentRoomDoor);
        Vector3 pos = transform.position;
        pos = Vector3.Lerp(pos, door.transform.position, .075f);
        transform.position = pos;
    }

    private void IdleInRoom()
    {
        if (currentRoom == null) return;
        
        Vector3 pos = transform.position;
        pos = Vector3.Lerp(pos, currentRoom.transform.position, .075f);
        transform.position = pos;
    }

    public void SendToRoom(RoomType roomType)
    {
        //cancel last path
        doorPath.Clear();
        isIdle = false;
        SimPathing.CreatePath(this, SimPathing.FindRoomByRoomType(roomType));
    }
}
