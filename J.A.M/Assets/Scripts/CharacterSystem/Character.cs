using System;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private string characterName;
    //[SerializeField] private Image characterPortrait;
    
    [SerializeField] private TraitSystem.Traits traits;
    
    [Range(0,100)]
    private float mood = 50.0f; 
    [Range(0,100)]
    private float stress = 10.0f;
    
    private bool isIdle;

    void Start()
    {
        TraitSystem.ApplyBonuses(this, TraitSystem.Job.Mechanic, TraitSystem.PositiveTraits.Crafty | TraitSystem.PositiveTraits.GreenHanded, TraitSystem.NegativeTraits.Dull | TraitSystem.NegativeTraits.Slow);
    }

    public TraitSystem.Job GetJob() { return traits.GetJob(); }

    public TraitSystem.PositiveTraits GetPositiveTraits() { return traits.GetPositiveTraits(); }

    public TraitSystem.NegativeTraits GetNegativeTraits() { return traits.GetNegativeTraits(); }

    private void CapMood() { mood = mood > stress ? stress : mood; }
}
