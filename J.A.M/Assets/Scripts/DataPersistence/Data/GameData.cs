[System.Serializable]
public class GameData
{
    public SerializableDictionary<SystemType, float> gaugeValues;
    public SerializableDictionary<string, TraitsData.Traits> characterTraits;
    public TraitsData.SpaceshipTraits spaceshipTraits;
    public TraitsData.HiddenSpaceshipTraits hiddenSpaceshipTraits;

    public GameData()
    {
        gaugeValues = new SerializableDictionary<SystemType, float>();
        characterTraits = new SerializableDictionary<string, TraitsData.Traits>();
        spaceshipTraits = new TraitsData.SpaceshipTraits();
        hiddenSpaceshipTraits = new TraitsData.HiddenSpaceshipTraits();
    }
}