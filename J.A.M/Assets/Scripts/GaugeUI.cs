using UnityEngine;

public class GaugeUI : HoverableObject

{
    public SystemType systemType;
    public bool IsPreviewing;

    public virtual void UpdateGauge(float value, float previewValue)
    {
        
    }

    public virtual void PreviewOutcomeGauge(float value)
    {
        
    }

    public virtual void ResetPreviewGauge()
    {
        
    }
}