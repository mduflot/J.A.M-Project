using System.Collections.Generic;
using SS;
using SS.Enumerations;

[System.Serializable]
public class GameData
{
    /*** TIME ***/
    public uint time;

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
    public SerializableDictionary<string, uint> waitingTimesTimeline;
    public SerializableDictionary<string, List<SerializableTuple<string, string>>> dialogueTimelines;
    public SerializableDictionary<string, List<string>> charactersActiveTimelines;
    public SerializableDictionary<string, List<string>> assignedActiveTimelines;
    public SerializableDictionary<string, List<string>> notAssignedActiveTimelines;
    public SerializableDictionary<string, List<string>> traitsCharactersActiveStorylines;

    /*** STORYLINES LOG ***/
    public List<StorylineLog> allStorylineLogs;

    public GameData()
    {
        /*** TIME ***/
        time = 0;

        /*** SPACESHIP ***/
        gaugeValues = new SerializableDictionary<SystemType, float>();
        characterTraits = new SerializableDictionary<string, TraitsData.Traits>();
        spaceshipTraits = new TraitsData.SpaceshipTraits();
        hiddenSpaceshipTraits = new TraitsData.HiddenSpaceshipTraits();

        /*** STORYLINES ***/
        currentCampaignID = "";
        storylineStatus = new SerializableDictionary<string, SSStoryStatus>();
        timelineStatus = new SerializableDictionary<string, SSStoryStatus>();
        activeStorylines = new List<string>();
        currentNodes = new SerializableDictionary<string, string>();
        waitingTimesTimeline = new SerializableDictionary<string, uint>();
        dialogueTimelines = new SerializableDictionary<string, List<SerializableTuple<string, string>>>();
        charactersActiveTimelines = new SerializableDictionary<string, List<string>>();
        assignedActiveTimelines = new SerializableDictionary<string, List<string>>();
        notAssignedActiveTimelines = new SerializableDictionary<string, List<string>>();
        traitsCharactersActiveStorylines = new SerializableDictionary<string, List<string>>();

        /*** STORYLINES LOG ***/
        allStorylineLogs = new List<StorylineLog>();
    }
}