using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
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
    [HideInInspector] public SimRoom taskRoom;
    public SimRoom idleRoom;

    public SimStatus simStatus;
    public int tick;
    public int ticksToEat;
    
    private GameObject taskFurniture;
    
    private void Start()
    {
        currentRoom = idleRoom;
        currentRoomDoor = currentRoom.roomDoors[0].doorID;
        ticksToEat = (GameManager.Instance.SpaceshipManager.simHungerBaseThreshold 
                      * (int) TimeTickSystem.ticksPerHour) 
                     + Random.Range(
                         (int) -TimeTickSystem.ticksPerHour * GameManager.Instance.SpaceshipManager.simHungerNoise, 
                         (int) TimeTickSystem.ticksPerHour * GameManager.Instance.SpaceshipManager.simHungerNoise);
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
        
        SimDoor door = SimPathing.FindDoorByID(currentRoomDoor);
        if ((door.transform.position - transform.position).magnitude < 1.5f * TimeTickSystem.timeScale)
            FadeToNextDoor();
        else
            MoveToCurrentDoor();
    }

    private void FadeToNextDoor()
    {
        if (doorPath.Count == 0)
        {
            isIdle = true;

            if (simStatus == SimStatus.GoToEat)
            {
                simStatus = SimStatus.IdleEat;
                ticksToEat = 0;
            }
            else
                simStatus = SimStatus.Idle;
            
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
        if(doorPath.Count == 1)
            switch (simStatus)
            {
                case SimStatus.GoToIdle:
                    if (currentRoom == idleRoom)
                    {
                        doorPath.Clear();
                        return;
                    }
                    break;
                
                case SimStatus.GoToRoom:
                    if (currentRoom == taskRoom)
                    {
                        doorPath.Clear();
                        return;
                    }
                    break;
            }
        SimDoor door = SimPathing.FindDoorByID(currentRoomDoor);
        Vector3 pos = transform.position;
        pos += TimeTickSystem.timeScale * GameManager.Instance.SpaceshipManager.simMoveSpeed * (door.transform.position - transform.position).normalized;
        transform.position = pos;
    }

    private void IdleInRoom()
    {
        if (currentRoom == null) return;
        
        if(taskRoom == null && simStatus != SimStatus.IdleEat && currentRoom != idleRoom)
            SendToIdleRoom();
        
        
        Vector3 pos = transform.position;

        Vector3 newPos = currentRoom.transform.position;
        int charIndex = currentRoom.presentCharacters.IndexOf(this);
        newPos.x += currentRoom.roomXOffset * (charIndex - 3);
        newPos.y -= currentRoom.roomYOffset;

        if ((newPos - transform.position).magnitude < .5f * TimeTickSystem.timeScale) return;
        
        pos += GameManager.Instance.SpaceshipManager.simMoveSpeed * (newPos - transform.position).normalized;
        
        transform.position = pos;
    }

    public void SendToRoom(RoomType roomType)
    {
        if (roomType == currentRoom.roomType) return;
        
        //cancel last path
        doorPath.Clear();

        SimRoom nextRoom = SimPathing.FindRoomByRoomType(roomType);
        
        isIdle = false;
        SimPathing.CreatePath(this, nextRoom);
        simStatus = SimStatus.GoToRoom;
        currentRoom.presentCharacters.Remove(this);
        nextRoom.presentCharacters.Add(this);
    }

    public void SendToIdleRoom()
    {
        SendToRoom(idleRoom.roomType);
        simStatus = SimStatus.GoToIdle;
    }

    public bool IsBusy()
    {
        return simStatus == SimStatus.GoToEat 
               || simStatus == SimStatus.GoToIdle 
               || simStatus == SimStatus.GoToRoom 
               || simStatus == SimStatus.IdleEat;
    }
    
    public enum SimStatus
    {
        Idle,
        IdleEat,
        GoToRoom,
        GoToIdle,
        GoToEat,
    }
}
