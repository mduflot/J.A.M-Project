using UnityEngine;

[CreateAssetMenu(menuName = "Task/TaskOutcome/GaugeLevelOutcome")]
public class GaugeLevelOutcome : BaseTaskOutcome
{
    public SpaceshipManager.SystemType targetGauge;

    public Operation operation;

    [Range(0, 20)] public float value;

    public enum Operation
    {
        Add,
        Substract
    }

    public override void Outcome()
    {
        if (operation == Operation.Add)
        {
            //float newGaugeValue = leaderCharacters[0].GetVolition();
            float newGaugeValue = 0.0f;
            foreach (var leader in leaderCharacters)
            {
                newGaugeValue += leader.GetVolition();
            }

            newGaugeValue = value / leaderCharacters.Count;

            GameManager.Instance.SpaceshipManager.GaugeValueOperation(targetGauge, newGaugeValue);
        }
        else
        {
            GameManager.Instance.SpaceshipManager.GaugeValueOperation(targetGauge, -value);
        }
    }
}