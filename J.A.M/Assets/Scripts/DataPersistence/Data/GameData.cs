using System.Collections.Generic;
using SS.Enumerations;

[System.Serializable]
public class GameData
{
    public SerializableDictionary<SystemType, float> gaugeValues;
    public SerializableDictionary<string, TraitsData.Traits> characterTraits;
    public TraitsData.SpaceshipTraits spaceshipTraits;
    public TraitsData.HiddenSpaceshipTraits hiddenSpaceshipTraits;
    public string currentCampaignID;
    public SerializableDictionary<string, SSStoryStatus> storylineStatus;
    public SerializableDictionary<string, SSStoryStatus> timelineStatus;
    public List<string> activeStorylines;

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
        
    }
}