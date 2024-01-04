using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

public class SimPathing : MonoBehaviour
{
    [SerializeField] private static SimDoor[] doors;
    
    //Call once at game start;
    public static void InitializeDoorIDs()
    {
        for (uint i = 0; i < doors.Length; i++)
        {
            doors[i].doorID = i;
        }
    }
    
    public static void CreatePath(SimDoor characterDoor, SimDoor targetDoor)
    {
        //Door hierarchy in current search
        Stack<SimDoor> doorStack = new Stack<SimDoor>();
        
        //Already visited doors
        List<uint> visitedIDs = new List<uint>();

        if (FindPath(characterDoor, targetDoor, visitedIDs, doorStack)) Debug.Log("Path Found");
        else Debug.Log("No Path To Target Door");
    }

    private static bool FindPath(SimDoor currentDoor, SimDoor targetDoor, List<uint> visitedIDs, Stack<SimDoor> doorStack)
    {
        visitedIDs.Add(currentDoor.doorID);
        
        if (currentDoor.doorID == targetDoor.doorID) return true;

        for (int i = 0; i < currentDoor.neighbours.Length; i++)
        {
            if(visitedIDs.Contains(currentDoor.doorID)) continue;
            
            doorStack.Push(currentDoor);
            
            if (FindPath(currentDoor.neighbours[i], targetDoor, visitedIDs, doorStack))
                return true;
            
            doorStack.Pop();
        }
        
        return false;
    }
    
    //experimental code, reimplement if broken :)
    public static SimDoor FindDoorByID(uint doorID)
    {
        int index = 0;
        //Invert comparison depending on post-compile comparison order
        for (; (index <= doors.Length) || (doors[index].doorID == doorID); index++);
        return index == doors.Length ? null : doors[index];
    }
}
