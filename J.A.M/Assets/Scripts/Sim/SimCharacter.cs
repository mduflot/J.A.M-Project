using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimCharacter : MonoBehaviour
{
    [HideInInspector] public bool isIdle = true;
    [HideInInspector] public uint currentRoomDoor = 0;
    public Stack<uint> doorPath = new();

    [HideInInspector] public SimRoom currentRoom;
    [HideInInspector] public SimRoom taskRoom;
    public SimRoom[] idleRooms;

    public SimStatus simStatus;
    public int tick;
    public int ticksToEat;
    public uint ticksToNextIdle;
    
    private GameObject taskFurniture;

    [SerializeField] private SpriteRenderer spriteRendererCharacter;
    [SerializeField] private Animator animatorCharacter;
    
    private void Start()
    {
        currentRoom = idleRooms[0];
        currentRoom.presentCharacters.Add(this);
        currentRoomDoor = currentRoom.roomDoors[0].doorID;
        ticksToNextIdle = (uint) Random.Range(1, 7) * TimeTickSystem.ticksPerHour;
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
            
            spriteRendererCharacter.enabled = true;
            return;
        }
        
        uint nextDoor = doorPath.Pop();

        spriteRendererCharacter.enabled = SimPathing.FindRoomByDoorID(nextDoor).roomType == currentRoom.roomType;
        
        currentRoomDoor = nextDoor;
        currentRoom = SimPathing.FindRoomByDoorID(currentRoomDoor);
    }

    private void MoveToCurrentDoor()
    {
        if(doorPath.Count == 1)
            switch (simStatus)
            {
                case SimStatus.GoToIdle:
                    if (idleRooms.Contains(currentRoom))
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
        animatorCharacter.SetBool("IsMoving", false);
        SetSpeedAnimator(TimeTickSystem.timeScale);
        
        if(taskRoom == null && simStatus != SimStatus.IdleEat && currentRoom != idleRooms[0])
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
        if (roomType == currentRoom.roomType)
        {
            simStatus = SimStatus.Idle;
            return;
        }
        animatorCharacter.SetBool("IsMoving", true);
        SetSpeedAnimator(TimeTickSystem.timeScale);
        
        //cancel last path
        doorPath.Clear();

        SimRoom nextRoom = SimPathing.FindRoomByRoomType(roomType);
        
        isIdle = false;
        SimPathing.CreatePath(this, nextRoom);
        if (doorPath.Count < 1) nextRoom = idleRooms[0]; //If CreatePath fails to find a path, return to idle room
        
        simStatus = SimStatus.GoToRoom;
        currentRoom.presentCharacters.Remove(this);
        nextRoom.presentCharacters.Add(this);
    }

    public void SendToIdleRoom()
    {
        animatorCharacter.SetBool("IsMoving", true);
        SetSpeedAnimator(TimeTickSystem.timeScale);
        simStatus = SimStatus.GoToIdle;
        var randRoom = Random.Range(0f, 1f);
        
        if (idleRooms.Length < 2)
            randRoom = 0f;
        
        if(randRoom <= .5f)
            SendToRoom(idleRooms[0].roomType);
        else
            SendToRoom(idleRooms[Random.Range(1, idleRooms.Length)].roomType);
    }

    public bool IsBusy()
    {
        return simStatus == SimStatus.GoToEat 
               || simStatus == SimStatus.GoToIdle 
               || simStatus == SimStatus.GoToRoom 
               || simStatus == SimStatus.IdleEat;
    }

    public void SetSpeedAnimator(float speed) {
        animatorCharacter.speed = speed;
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
