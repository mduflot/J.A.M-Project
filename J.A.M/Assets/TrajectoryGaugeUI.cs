using UnityEngine;
using UnityEngine.UI;

public class TrajectoryGaugeUI : GaugeUI
{
    [SerializeField] private Image topGauge;
    [SerializeField] private Image bottomGauge;
    [SerializeField] private Image topPreviewGauge;
    [SerializeField] private Image bottomPreviewGauge;
    [SerializeField] private RectTransform shipTransform;
    private bool isTop;
    private float maxAngle = 27.0f;

    public override void UpdateGauge(float value, float previewValue)
    {
        if (isTop)
        {
            topGauge.fillAmount = value / 50;
            topPreviewGauge.fillAmount = (value + previewValue) / 50;
            shipTransform.eulerAngles = new Vector3(0, 0, topGauge.fillAmount * -maxAngle);
        }
        else
        {
            bottomGauge.fillAmount = value / 50;
            bottomPreviewGauge.fillAmount = (value + previewValue) / 50;
            shipTransform.eulerAngles = new Vector3(0, 0, bottomGauge.fillAmount * maxAngle);
        }
    }

    public override void PreviewOutcomeGauge(float value)
    {
        var gauge = isTop ? topGauge : bottomGauge;
        var previewGauge = isTop ? topPreviewGauge : bottomPreviewGauge;
        
        if (value > 0)
        {
            value += gauge.fillAmount * 50;
            previewGauge.fillAmount = value / 50;
        }
        else
        {
            previewGauge.fillAmount = GameManager.Instance.SpaceshipManager.GetGaugeValue(systemType) / 50;
            gauge.fillAmount = (GameManager.Instance.SpaceshipManager.GetGaugeValue(systemType) + value) / 50;
        }
    }

    public override void ResetPreviewGauge()
    {
        if (isTop) topPreviewGauge.fillAmount = 0;
        else bottomGauge.fillAmount = 0;
    }
    
}
