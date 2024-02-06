using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TrajectoryGaugeUI : GaugeUI
{
    [SerializeField] private Image topGauge;
    [SerializeField] private Image bottomGauge;
    [SerializeField] private Image topPreviewGauge;
    [SerializeField] private Image bottomPreviewGauge;
    [SerializeField] private RectTransform shipTransform;
    [SerializeField] private Image arrow;
    [SerializeField] private Sprite greenArrow;
    [SerializeField] private Sprite redArrow;

    private Color colorOpaque = new(1.0f, 1.0f, 1.0f, 1.0f);
    private Color colorTransparent = new(1.0f, 1.0f, 1.0f, 0.0f);
    private bool isTop;
    private bool isDecreasing;
    private float maxAngle = 27.0f;

    public override void UpdateGauge(float value, float previewValue)
    {
        if (isTop)
        {
            if (!IsPreviewing || topGauge.fillAmount < 1 - (value - previewValue) / 50)
                topGauge.fillAmount = 1 - (value - previewValue) / 50;
            topPreviewGauge.fillAmount = 1 - value / 50;
            shipTransform.eulerAngles = new Vector3(0, 0, topPreviewGauge.fillAmount * -maxAngle);
        }
        else
        {
            if (!isDecreasing)
            {
                if (!IsPreviewing || bottomGauge.fillAmount < 1 - (value - previewValue) / 50)
                    bottomGauge.fillAmount = 1 - (value - previewValue) / 50;
                bottomPreviewGauge.fillAmount = 1 - value / 50;
                shipTransform.eulerAngles = new Vector3(0, 0, bottomPreviewGauge.fillAmount * maxAngle);
            }
            else
            {
                bottomGauge.fillAmount = 1 - value / 50;
                if (!IsPreviewing || bottomPreviewGauge.fillAmount > 1 - (value - previewValue) / 50)
                    bottomPreviewGauge.fillAmount = 1 - (value - previewValue) / 50;
                shipTransform.eulerAngles = new Vector3(0, 0, bottomGauge.fillAmount * maxAngle);
            }
        }

        if (previewValue > 0.0f)
        {
            arrow.sprite = greenArrow;
            arrow.color = colorOpaque;
        }
        else if (GameManager.Instance.SpaceshipManager.systems.Any(system =>
                     system.type == systemType && system.isBlocked))
        {
            arrow.sprite = null;
            arrow.color = colorTransparent;
        }
        else
        {
            arrow.sprite = redArrow;
            arrow.color = colorOpaque;
        }
    }

    public override void AddPreviewGauge(float value)
    {
        var gauge = isTop ? topGauge : bottomGauge;
        var previewGauge = isTop ? topPreviewGauge : bottomPreviewGauge;

        if (value < 0)
        {
            isDecreasing = true;
            value *= -1;
            value += gauge.fillAmount * 50;
            previewGauge.fillAmount = value / 50;
        }
        else
        {
            isDecreasing = false;
            previewGauge.fillAmount = 1 - GameManager.Instance.SpaceshipManager.GetGaugeValue(systemType) / 50;
            gauge.fillAmount = 1 - (GameManager.Instance.SpaceshipManager.GetGaugeValue(systemType) + value) / 50;
        }
    }
}