using UnityEngine;


[CreateAssetMenu(menuName = "Task/TaskOutcome/GaugeLevelOutcome")]
public class GaugeLevelOutcome : BaseTaskOutcome
{
    public SpaceshipManager.System targetGauge;

    public Operation operation;
    
    [Range(0, 20)] public float value;

    public enum Operation
    {
        Add, Substract
    }
    public override void Outcome()
    {
        if (operation == Operation.Add)
        {
            GameManager.Instance.SpaceshipManager.GaugeValueOperation(targetGauge, leaderCharacters[0].GetVolition());
        }
        else
        {
            GameManager.Instance.SpaceshipManager.GaugeValueOperation(targetGauge, -value);
        }
    }
}
