using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimDoor : MonoBehaviour
{
    [HideInInspector] public uint doorID;
    public SimDoor[] neighbours;
    public Animator doorSprite;
}
