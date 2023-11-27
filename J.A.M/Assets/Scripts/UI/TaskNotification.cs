using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TaskNotification : MonoBehaviour
{
    public bool IsCompleted;
    public List<Tuple<Sprite, string, string>> Dialogues;
    public Task Task;
    public List<CharacterBehaviour> LeaderCharacters = new();
    public List<CharacterBehaviour> AssistantCharacters = new();

    [SerializeField] private SpriteRenderer icon;
    [SerializeField] private TextMeshPro time;

    private bool isStarted;
    private Camera camera;
    private SpaceshipManager spaceshipManager;

    private void Start()
    {
        camera = Camera.main;
    }
    
    private void Update()
    {
        if (!isStarted) return;
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit; 
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform)
                {
                    Display();
                }
            }
        }
    }

    public void Initialize(Task task, SpaceshipManager spaceshipManager, List<Tuple<Sprite, string, string>> dialogues = null)
    {
        Task = task;
        icon.sprite = task.Icon;
        Dialogues = dialogues;
        this.spaceshipManager = spaceshipManager;
    }

    public void Display()
    {
        GameManager.Instance.UIManager.SpawnTaskUI(this);
    }

    public void OnStart(List<CharacterUISlot> characters)
    {
        foreach (var character in characters)
        {
            if (character.isMandatory)
            {
                LeaderCharacters.Add(character.icon.character);
                character.icon.character.AssignTask(this, true);
            }
            else
            {
                if (character.icon != null)
                {
                    AssistantCharacters.Add(character.icon.character);
                    character.icon.character.AssignTask(this);
                }
            }
        }

        Task.Duration = AssistantCharacters.Count > 0
            ? Task.Duration / Mathf.Pow(AssistantCharacters.Count + LeaderCharacters.Count, Task.HelpFactor)
            : Task.Duration;
        Task.Duration *= TimeTickSystem.ticksPerHour;
        isStarted = true;
    }

    public void OnUpdate()
    {
        if (!isStarted) return;
        if (Task.Duration > 0)
        {
            Task.Duration -= TimeTickSystem.timePerTick;
            // var completionValue = 1 - Task.Duration / (Task.Duration * TimeTickSystem.ticksPerHour);
            // completionGauge.fillAmount = completionValue;
            time.text = Task.Duration.ToString("F2") + " hours";
        }
        else
        {
            OnComplete();
        }
    }

    private void OnComplete()
    {
        IsCompleted = true;
        ResetCharacters();
        GameManager.Instance.RefreshCharacterIcons();
        spaceshipManager.taskNotificationPool.AddToPool(gameObject);
    }

    public void OnCancel()
    {
        ResetCharacters();
        spaceshipManager.taskNotificationPool.AddToPool(gameObject);
    }

    private void ResetCharacters()
    {
        foreach (var character in LeaderCharacters)
        {
            character.StopTask();
        }

        foreach (var character in AssistantCharacters)
        {
            character.StopTask();
        }
    }
}