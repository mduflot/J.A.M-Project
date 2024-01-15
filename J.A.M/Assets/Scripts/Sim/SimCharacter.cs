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
        ticksToEat = GameManager.Instance.SpaceshipManager.simHungerBaseThreshold + Random.Range((int) -TimeTickSystem.ticksPerHour * 2, (int) TimeTickSystem.ticksPerHour * 2);
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
        //if (roomType == currentRoom.roomType) return;
        //cancel last path
        doorPath.Clear();
        isIdle = false;
        SimPathing.CreatePath(this, SimPathing.FindRoomByRoomType(roomType));
        simStatus = SimStatus.GoToRoom;
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
