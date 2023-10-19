using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image[] gauges;
    private Dictionary<SpaceshipManager.System, Image> gaugeReferences = new Dictionary<SpaceshipManager.System, Image>();

    private void InitializeGauges()
    {
        /*for (int i = 0; i < gauges.Length; i++)
        {
            gaugeReferences.Add(, gauges[i]);
        }*/
    }
    public void UpdateGauges(SpaceshipManager.System system, float value)
    {
        gaugeReferences[system].fillAmount = value/100;
    }
}
