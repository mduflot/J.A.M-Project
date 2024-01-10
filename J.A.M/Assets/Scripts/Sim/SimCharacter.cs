using System;
using System.Collections.Generic;
using Managers;
using UnityEditor.AddressableAssets.Build.BuildPipelineTasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimCharacter : MonoBehaviour
{
    [HideInInspector] public bool isIdle = true;
    [HideInInspector] public uint currentRoomDoor = 0;
    public Stack<uint> doorPath = new();

    [HideInInspector] public SimRoom currentRoom;
    public SimRoom idleRoom;

    private GameObject taskFurniture;
    
    private void Start()
    {
        currentRoom = idleRoom;
        currentRoomDoor = currentRoom.roomDoors[0].doorID;
    }

    private void Update()
    {
        Simulate();
    }

    public void Simulate()
    {
        currentRoom = SimPathing.FindRoomByDoorID(currentRoomDoor);
        if (isIdle)
        {
            IdleInRoom();
            return;
        }
        
        //Add scaling based on TimeTickSystem

        SimDoor door = SimPathing.FindDoorByID(currentRoomDoor);
        if ((door.transform.position - transform.position).magnitude < 1.5f)
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
        float lerpT = (1 - Mathf.Exp(-GameManager.Instance.SpaceshipManager.simMoveSpeed * Time.deltaTime)) * TimeTickSystem.timeScale;
        SimDoor door = SimPathing.FindDoorByID(currentRoomDoor);
        Vector3 pos = transform.position;
        pos = Vector3.Lerp(pos, door.transform.position, lerpT);
        transform.position = pos;
    }

    private void IdleInRoom()
    {
        if (currentRoom == null) return;
        
        float lerpT = (1 - Mathf.Exp(-GameManager.Instance.SpaceshipManager.simMoveSpeed * Time.deltaTime)) * TimeTickSystem.timeScale;
        Vector3 pos = transform.position;
        pos = Vector3.Lerp(pos,
            taskFurniture == null ? currentRoom.transform.position : taskFurniture.transform.position,
            lerpT);
        transform.position = pos;
    }

    public void SendToRoom(RoomType roomType)
    {
        //cancel last path
        doorPath.Clear();
        isIdle = false;
        SimPathing.CreatePath(this, SimPathing.FindRoomByRoomType(roomType));
    }

    public void SendToIdleRoom()
    {
        SendToRoom(idleRoom.roomType);
    }
}
