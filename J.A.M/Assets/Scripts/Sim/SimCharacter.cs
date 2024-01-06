using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimCharacter : MonoBehaviour
{
    [HideInInspector] public bool moveToTarget;
    
    [HideInInspector] public uint currentRoomDoor = 6;
    public Stack<uint> doorPath = new Stack<uint>();

    private void Update()
    {
        Simulate();
        
        if (Input.GetKey(KeyCode.Space) && doorPath.Count == 0)
        {
            moveToTarget = true;
            uint targetID = (uint)Random.Range(1, 4);
            SimPathing.CreatePath(SimPathing.FindDoorByID(currentRoomDoor), SimPathing.FindDoorByID(4), this);
        }
    }

    //todo : add movment towards furniture
    //todo : scale w/ time tick
    //todo : add fade between doors
    //todo : implem vrai utilisation
    //todo : assurer que tout marche avec plusieurs persos
    //todo : stress test
    public void Simulate()
    {
        if (!moveToTarget) return;
        
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
        if (doorPath.Count == 0)
        {
            moveToTarget = false;
            return;
        }
        currentRoomDoor = doorPath.Pop();
    }

    private void MoveToCurrentDoor()
    {
        SimDoor door = SimPathing.FindDoorByID(currentRoomDoor);
        Vector3 pos = transform.position;
        pos = Vector3.Lerp(pos, door.transform.position, .0025f);
        transform.position = pos;
    }

    public void SetDoorPath(Stack<uint> path)
    {
        doorPath = path;
    }
}
