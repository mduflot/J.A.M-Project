using System;
using System.Collections.Generic;
using SS.Enumerations;
using UnityEngine;

[System.Serializable]
public class GameData
{
    /*** SPACESHIP ***/
    public SerializableDictionary<SystemType, float> gaugeValues;
    public SerializableDictionary<string, TraitsData.Traits> characterTraits;
    public SerializableDictionary<string, float> characterMoods;
    public SerializableDictionary<string, float> characterVolitions;
    public TraitsData.SpaceshipTraits spaceshipTraits;
    public TraitsData.HiddenSpaceshipTraits hiddenSpaceshipTraits;

    /*** STORYLINES ***/
    public string currentCampaignID;
    public SerializableDictionary<string, SSStoryStatus> storylineStatus;
    public SerializableDictionary<string, SSStoryStatus> timelineStatus;
    public List<string> activeStorylines;
    public List<string> activeTimelines;
    public SerializableDictionary<string, string> currentNodes;
    public SerializableDictionary<string, List<Tuple<Sprite, string, string>>> dialogueTimelines;
    public SerializableDictionary<string, List<string>> charactersActiveTimelines;
    public SerializableDictionary<string, List<string>> assignedActiveTimelines;
    public SerializableDictionary<string, List<string>> notAssignedActiveTimelines;
    public SerializableDictionary<string, List<string>> traitsCharactersActiveStorylines;

    /*** STORYLINES LOG ***/

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