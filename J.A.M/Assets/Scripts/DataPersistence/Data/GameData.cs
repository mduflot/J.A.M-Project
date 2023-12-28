using System;
using System.Collections.Generic;
using SS.Enumerations;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public SerializableDictionary<SystemType, float> gaugeValues;
    public SerializableDictionary<string, TraitsData.Traits> characterTraits;
    public SerializableDictionary<string, float> characterMoods;
    public SerializableDictionary<string, float> characterVolitions;
    public TraitsData.SpaceshipTraits spaceshipTraits;
    public TraitsData.HiddenSpaceshipTraits hiddenSpaceshipTraits;
    public string currentCampaignID;
    public SerializableDictionary<string, SSStoryStatus> storylineStatus;
    public SerializableDictionary<string, SSStoryStatus> timelineStatus;
    public List<string> activeStorylines;
    public List<string> activeTimelines;
    public SerializableDictionary<string, string> currentNodes;
    public SerializableDictionary<string, List<Tuple<Sprite, string, string>>> dialogueTimelines;

    public GameData()
    {
        gaugeValues = new SerializableDictionary<SystemType, float>();
        characterTraits = new SerializableDictionary<string, TraitsData.Traits>();
        spaceshipTraits = new TraitsData.SpaceshipTraits();
        hiddenSpaceshipTraits = new TraitsData.HiddenSpaceshipTraits();
        currentCampaignID = "";
        storylineStatus = new SerializableDictionary<string, SSStoryStatus>();
        timelineStatus = new SerializableDictionary<string, SSStoryStatus>();
        activeStorylines = new List<string>();
        currentNodes = new SerializableDictionary<string, string>();
        dialogueTimelines = new SerializableDictionary<string, List<Tuple<Sprite, string, string>>>();
    }
}