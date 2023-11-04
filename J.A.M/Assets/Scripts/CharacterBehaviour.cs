using UnityEngine;

public class CharacterBehaviour : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    private bool isWorking;
    private bool isTaskLeader;
    private TaskNotification currentTask;
    [SerializeField] private CharacterDataScriptable data;


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
