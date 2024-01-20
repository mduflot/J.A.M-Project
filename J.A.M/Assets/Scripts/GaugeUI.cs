using System.Linq;
using UnityEngine;

public abstract class GaugeUI : HoverableObject
{
    public SystemType systemType;
    public bool IsPreviewing;
    public GameObject parentGauge;

    public void InitializeGauge()
    {
        data = new HoverMenuData
        {
            text1 = systemType.ToString(),
            text2 = "Decrease : " + GameManager.Instance.SpaceshipManager.systems.First(system => system.type == systemType).decreaseSpeed,
            baseParent = parentGauge.transform,
            parent = transform
        };
        hoverMenu.Initialize(data);
        hoverMenu.gameObject.SetActive(false);
    }

    public abstract void UpdateGauge(float value, float previewValue);

    public abstract void PreviewOutcomeGauge(float value);

    public abstract void ResetPreviewGauge();
}