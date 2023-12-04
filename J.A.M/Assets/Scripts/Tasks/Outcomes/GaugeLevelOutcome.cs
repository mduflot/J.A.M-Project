using UnityEngine;

[CreateAssetMenu(menuName = "Task/TaskOutcome/GaugeLevelOutcome")]
public class GaugeLevelOutcome : BaseTaskOutcome
{
    public SystemType targetGauge;

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
            float newGaugeValue = 0.0f;
            foreach (var leader in tn.LeaderCharacters)
            {
                newGaugeValue += leader.GetVolition();
                //newGaugeValue += TraitSystem.ApplyJobBonus(TraitSystem.MatchJobFlags(leader.GetJob(), tn.taskData.taskTraits.GetJob()));
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