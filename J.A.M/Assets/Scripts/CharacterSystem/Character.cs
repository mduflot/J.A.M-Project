using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private string characterName;
    //[SerializeField] private Image characterPortrait;

    [SerializeField] private TraitSystem.Job job; //Rename to archetype to fit documentation

    [SerializeField] private TraitSystem.PositiveTraits positiveTraits;

    [SerializeField] private TraitSystem.NegativeTraits negativeTraits;

    [Range(0,100)]
    private float mood = 50.0f; 
    [Range(0,100)]
    private float stress = 10.0f;
    
    private bool isIdle;

    void Start()
    {
        TraitSystem.ApplyBonuses(this, TraitSystem.Job.Mechanic, TraitSystem.PositiveTraits.Crafty | TraitSystem.PositiveTraits.GreenHanded, TraitSystem.NegativeTraits.Dull | TraitSystem.NegativeTraits.Slow);
    }

    public TraitSystem.Job GetJob() { return job; }

    public TraitSystem.PositiveTraits GetPositiveTraits() { return positiveTraits; }

    public TraitSystem.NegativeTraits GetNegativeTraits() { return negativeTraits; }

    private void CapMood() { mood = mood > stress ? stress : mood; }
}
