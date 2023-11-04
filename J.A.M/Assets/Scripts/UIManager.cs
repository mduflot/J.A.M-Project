using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public Gauges[] gauges;
    private Dictionary<SpaceshipManager.System, Image> gaugeReferences = new Dictionary<SpaceshipManager.System, Image>();
    public Transform charactersUIParent;
    public List<CharacterUI> charactersUI;
    private List<CharacterIcon> characterIcons = new List<CharacterIcon>();
    public CharacterUI characterUIPrefab;
    public Transform taskNotificationParent;
    public Transform taskParent;
    public Task taskUI;
    public Canvas canvas;
    public TextMeshProUGUI date;
    
    [Serializable] 
    public struct Gauges
    {
        public SpaceshipManager.System system;
        public Image gauge;
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        for (int i = 0; i < gauges.Length; i++)
        {
            gaugeReferences.Add(gauges[i].system, gauges[i].gauge);
        }

        foreach (var character in GameManager.Instance.SpaceshipManager.characters)
        {
            var ui = Instantiate(characterUIPrefab, charactersUIParent);
            ui.Initialize(character);
            charactersUI.Add(ui);
            characterIcons.Add(ui.icon);
        }
    }

    public void SpawnTaskUI(TaskNotification tn)
    {
        taskUI.Initialize(tn);
    }

    public void UpdateGauges(SpaceshipManager.System system, float value)
    {
        gaugeReferences[system].fillAmount = value/100;
    }

    public void UpdateInGameDate(string newDate)
    {
        date.text = newDate;
    }

    public void RefreshCharacterIcons()
    {
        foreach (var icon in characterIcons)
        {
            icon.RefreshIcon();
        }
    }
}
