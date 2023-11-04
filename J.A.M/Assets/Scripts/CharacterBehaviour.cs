using System;
using UnityEngine;

public class CharacterBehaviour : MonoBehaviour
{
    [SerializeField] private CharacterDataScriptable data;
    [SerializeField] private float moveSpeed;
    
    [Range(0,100)]
    private float mood = 50.0f; 
    
    [Range(0,100)]
    
    [SerializeField] private TraitsData.Traits traits;
    
    private float volition = 10.0f;
    private bool isWorking;
    private bool isTaskLeader;
    private TaskNotification currentTask;


    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        mood = data.baseMood;
        volition = data.baseVolition;
        traits = data.traits;
    }
    
    public TraitsData.Job GetJob() { return traits.GetJob(); }

    public TraitsData.PositiveTraits GetPositiveTraits() { return traits.GetPositiveTraits(); }

    public TraitsData.NegativeTraits GetNegativeTraits() { return traits.GetNegativeTraits(); }

    private void CapMood() { volition = mood < volition ? mood : volition; }
    
    
    public void MoveTo(Transform destination)
    {
        float timeToTravel = Vector2.Distance(transform.position, destination.position) * moveSpeed;
        //Ajouter une étape avec la porte de la pièce active
    }

    public void AssignTask(TaskNotification t, bool leader = false)
    {
        isWorking = true;
        currentTask = t;
        isTaskLeader = leader;
    }

    public bool IsWorking()
    {
        return isWorking;
    }

    public bool IsTaskLeader()
    {
        return isTaskLeader;
    }

    public TaskNotification GetTask()
    {
        return currentTask;
    }

    public CharacterDataScriptable GetCharacterData()
    {
        return data;
    }

    public void StopTask()
    {
        isWorking = false;
        currentTask = null;
    }
}
