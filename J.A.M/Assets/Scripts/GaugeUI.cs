using System.Linq;

public abstract class GaugeUI : HoverableObject
{
    public SystemType systemType;
    public bool IsPreviewing;

    public void InitializeGauge()
    {
        data = new HoverMenuData
        {
            text1 = systemType.ToString(),
            text2 = "Decrease : " + GameManager.Instance.SpaceshipManager.systems.First(system => system.type == systemType).decreaseSpeed,
            baseParent = transform.parent,
            parent = transform
        };
        hoverMenu.Initialize(data);
        hoverMenu.gameObject.SetActive(false);
    }

    public abstract void UpdateGauge(float value, float previewValue);

    public abstract void PreviewOutcomeGauge(float value);

    public abstract void ResetPreviewGauge();
}