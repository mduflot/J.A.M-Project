using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;
using System.Linq;
using System.Xml.Linq;
using Cysharp.Threading.Tasks.Triggers;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;

public class SimPathing : MonoBehaviour
{
    public static SimPathing instance;
    
    [SerializeField] private SimDoor[] doors;
    [SerializeField] private SimRoom[] rooms;
    
    private void Awake()
    {
        if (instance == null) instance = this;
        InitializeDoorIDs();
        instance.rooms = rooms;
    }

    //Call once at game start;
    private static void InitializeDoorIDs()
    {
        for (uint i = 0; i < instance.doors.Length; i++)
            instance.doors[i].doorID = i+1; 
    }

    public static void CreatePath(SimCharacter character, SimRoom targetRoom)
    {
        if (character.currentRoom == targetRoom || targetRoom == null) return;
        
        //Door hierarchy in current search
        Stack<SimDoor> doorStack = new Stack<SimDoor>();
        
        //Already visited doors
        List<uint> visitedIDs = new List<uint>();

        if (!FindPath(character.currentRoom, targetRoom, visitedIDs, doorStack)) 
        {
            Debug.LogError("No Path To Target Door");
            return;
        }
        CreateIDStack(doorStack, character);
    }
    
    private static void CreateIDStack(Stack<SimDoor> doorStack, SimCharacter targetCharacter)
    {
        targetCharacter.doorPath.Clear();
        
        Stack<uint> path = new Stack<uint>();
        
        while (doorStack.Count != 0)
            path.Push(doorStack.Pop().doorID);
        
        targetCharacter.doorPath = path;
    }

    private static bool FindPath(SimDoor currentDoor, SimDoor targetDoor, List<uint> visitedIDs, Stack<SimDoor> doorStack)
    {
        visitedIDs.Add(currentDoor.doorID);
        doorStack.Push(currentDoor);
        
        if (currentDoor.doorID == targetDoor.doorID) return true;
        
        for (int i = 0; i < currentDoor.neighbours.Length; i++)
        {
            if (visitedIDs.Contains(currentDoor.neighbours[i].doorID)) continue;
            if (FindPath(currentDoor.neighbours[i], targetDoor, visitedIDs, doorStack)) return true;
        }

        doorStack.Pop();
        return false;
    }
    
    private static bool FindPath(SimRoom currentRoom, SimRoom targetRoom, List<uint> visitedIDs, Stack<SimDoor> doorStack)
    {
        for(int i = 0; i < currentRoom.roomDoors.Length; i++)
            for (int j = 0; j < targetRoom.roomDoors.Length; j++)
            {
                if (FindPath(currentRoom.roomDoors[i], targetRoom.roomDoors[j], visitedIDs, doorStack)) return true;
                visitedIDs.Clear();
                doorStack.Clear();
            }

        return false;
    }
    
    public static SimDoor FindDoorByID(uint doorID)
    {
        int index = 0;
        for (; (index != instance.doors.Length - 1) && (instance.doors[index].doorID != doorID); index++) ;
        return index == instance.doors.Length ? null : instance.doors[index];
    }

    public static SimRoom FindRoomByDoorID(uint doorID)
    {
        for (int i = 0; i < instance.rooms.Length; i++)
        {
            foreach (SimDoor door in instance.rooms[i].roomDoors)
            {
                if (door.doorID == doorID) return instance.rooms[i];
            }
        }

        return null;
    }

    public static SimRoom FindRoomByRoomType(RoomType roomType)
    {
        for (int i = 0; i < instance.rooms.Length; i++)
            if (instance.rooms[i].roomType == roomType) return instance.rooms[i];

        return null;
    }
}
