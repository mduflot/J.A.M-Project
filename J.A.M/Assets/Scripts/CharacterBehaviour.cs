using System;
using UnityEngine;

public class CharacterBehaviour : MonoBehaviour
{
    [SerializeField] private CharacterDataScriptable data;
    [SerializeField] private float moveSpeed;
    
    /*
     * task assign -> mood - 3
     * 1H w/o assign -> mood +1
     * 1H w/ gauge = 0 -> mood -1
     * gauge = 0 -> mood + param
     */
    
    [Range(0,100)]
    private float mood = 50.0f;

    [Range(0, 100)] private const float baseVolition = 70.0f;
    
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
        Debug.Log("new mood is : " + mood);
        CapVolition();
    }

    public void IncreaseVolition(float value)
    {
        volition += value;
        Debug.Log("new volition is : " + volition);
        CapVolition();
    }
    private void CapVolition() { volition = mood < volition ? mood :  baseVolition; }

    private void UpdateStats()
    {
        
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

    public float GetVolition()
    {
        return volition;
    }
    
    public void StopTask()
    {
        isWorking = false;
        currentTask = null;
    }
}
