public class GaugeCheck
{
    public enum GaugeValue
    {
        Any,
        Low,
        Average,
        High
    }

    public static GaugeValue GetGaugeValue(SystemType gaugeType)
    {
        var gaugeValue = GameManager.Instance.SpaceshipManager.GetGaugeValue(gaugeType);
        var maxGaugeValue = 50.0f;
        var gaugePercent = gaugeValue / maxGaugeValue;

        if (gaugePercent >= .75f) return GaugeValue.High;

        if (gaugePercent < .75f && gaugePercent >= .25f) return GaugeValue.Average;

        return GaugeValue.Low;
    }

    public static bool CheckGaugeValue(float valueToCheck, SystemType gaugeType)
    {
        var currentGaugeValue = GameManager.Instance.SpaceshipManager.GetGaugeValue(gaugeType);
        return currentGaugeValue > valueToCheck;
    }
}