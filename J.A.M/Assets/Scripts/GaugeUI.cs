public abstract class GaugeUI : HoverableObject
{
    public SystemType systemType;
    public bool IsPreviewing;

    public abstract void UpdateGauge(float value, float previewValue);

    public abstract void PreviewOutcomeGauge(float value);

    public abstract void ResetPreviewGauge();
}