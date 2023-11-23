using System;
using UnityEngine;

public class CharacterBehaviour : MonoBehaviour
{
    [SerializeField] private CharacterDataScriptable data;
    [SerializeField] private float moveSpeed;
    
    /*
     * gauge = 0 -> mood + param
     */
    
    private const float MaxMood = 20.0f;
    
    [Range(0,100)]
    private float mood = 50.0f;

    
    [Range(0,100)]
    private float volition = 10.0f;
    
    [SerializeField] private TraitsData.Traits traits;
    
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

    public void IncreaseMood(float value)
    {
        mood += value;
        CapStats();
    }

    public void IncreaseVolition(float value)
    {
        volition += value;
    }
    
    private void CapStats()
    {
        mood = Mathf.Clamp(mood, 0, MaxMood);
    }
    
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
        mood -= 3.0f;
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

    public float GetMood()
    {
        return mood;
    }

    public float GetMaxMood()
    {
        return MaxMood;
    }
    
    public float GetVolition()
    {
        return mood < volition ? mood : volition;
    }

    public float GetBaseVolition()
    {
        return volition;
    }
    
    public void StopTask()
    {
        isWorking = false;
        currentTask = null;
    }
}
