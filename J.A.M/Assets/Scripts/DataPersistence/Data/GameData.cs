[System.Serializable]
public class GameData
{
    public SerializableDictionary<SystemType, float> gaugeValues;
    public TraitsData.SpaceshipTraits spaceshipTraits;
    public TraitsData.HiddenSpaceshipTraits hiddenSpaceshipTraits;
    
    public GameData()
    {
        gaugeValues = new SerializableDictionary<SystemType, float>();
        spaceshipTraits = new TraitsData.SpaceshipTraits();
        hiddenSpaceshipTraits = new TraitsData.HiddenSpaceshipTraits();
    }
}