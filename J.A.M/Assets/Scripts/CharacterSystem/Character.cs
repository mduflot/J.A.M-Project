using System;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private string characterName;
    //[SerializeField] private Image characterPortrait;
    
    [SerializeField] private TraitsData.Traits traits;
    
    [Range(0,100)]
    private float mood = 50.0f; 
    [Range(0,100)]
    private float stress = 10.0f;
    
    private bool isIdle;
    
    public TraitsData.Job GetJob() { return traits.GetJob(); }

    public TraitsData.PositiveTraits GetPositiveTraits() { return traits.GetPositiveTraits(); }

    public TraitsData.NegativeTraits GetNegativeTraits() { return traits.GetNegativeTraits(); }

    private void CapMood() { mood = mood > stress ? stress : mood; }
}
