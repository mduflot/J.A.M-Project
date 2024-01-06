using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;
using System.Linq;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;

public class SimPathing : MonoBehaviour
{
    public static SimPathing instance;
    
    [SerializeField] private SimDoor[] doors;

    private void Awake()
    {
        if (instance == null) instance = this;
        InitializeDoorIDs();
    }

    //Call once at game start;
    public static void InitializeDoorIDs()
    {
        for (uint i = 0; i < instance.doors.Length; i++)
            instance.doors[i].doorID = i+1; 
    }
    
    public static void CreatePath(SimDoor characterDoor, SimDoor targetDoor, SimCharacter targetCharacter)
    {
        if (characterDoor.doorID == targetDoor.doorID) return;
        
        //Door hierarchy in current search
        Stack<SimDoor> doorStack = new Stack<SimDoor>();
        
        //Already visited doors
        List<uint> visitedIDs = new List<uint>();

        if (!FindPath(characterDoor, targetDoor, visitedIDs, doorStack)) 
        {
            Debug.LogError("No Path To Target Door");
            return;
        }
        CreateIDStack(doorStack, targetCharacter);
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
    
    //experimental code, reimplement if broken :)
    public static SimDoor FindDoorByID(uint doorID)
    {
        int index = 0;
        for (; (index != instance.doors.Length - 1) && (instance.doors[index].doorID != doorID); index++) ;
        return index == instance.doors.Length ? null : instance.doors[index];
    }
}
