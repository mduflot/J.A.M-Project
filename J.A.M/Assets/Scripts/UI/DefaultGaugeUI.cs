using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DefaultGaugeUI : GaugeUI
{
    [SerializeField] private Image gauge;
    [SerializeField] private Image previewGauge;
    [SerializeField] private Image arrow;
    [SerializeField] private Sprite greenArrow;
    [SerializeField] private Sprite redArrow;
    [SerializeField] private int totalGaugeValue = 50;

    // Colors for the arrow sprite (alpha is 0 for transparent and 1 for opaque)
    private Color colorOpaque = new(1.0f, 1.0f, 1.0f, 1.0f);
    private Color colorTransparent = new(1.0f, 1.0f, 1.0f, 0.0f);
    private bool isHovered;

    public override void UpdateGauge(float value, float previewValue)
    {
        if (previewValue < 0.0f)
        {
            gauge.fillAmount = (value + previewValue) / totalGaugeValue;
            previewGauge.fillAmount = value / totalGaugeValue;
            arrow.sprite = redArrow;
        }
        else
        {
            gauge.fillAmount = value / totalGaugeValue;
            previewGauge.fillAmount = (value + previewValue) / totalGaugeValue;
            arrow.sprite = greenArrow;
        }

        arrow.color = colorOpaque;

        if (!GameManager.Instance.SpaceshipManager.systems.Any(system => system.type == systemType && system.isBlocked))
            return;
        arrow.sprite = null;
        arrow.color = colorTransparent;
    }

    public override void AddPreviewGauge(float value)
    {
        if (value > 0)
        {
            value += gauge.fillAmount * totalGaugeValue;
            previewGauge.fillAmount = value / totalGaugeValue;
            arrow.sprite = greenArrow;
        }
        else
        {
            previewGauge.fillAmount = GameManager.Instance.SpaceshipManager.GetGaugeValue(systemType) / totalGaugeValue;
            gauge.fillAmount = (GameManager.Instance.SpaceshipManager.GetGaugeValue(systemType) + value) /
                               totalGaugeValue;
            arrow.sprite = redArrow;
        }
    }

    public override void ResetPreviewGauge()
    {
        gauge.fillAmount = GameManager.Instance.SpaceshipManager.GetGaugeValue(systemType) / totalGaugeValue;
        previewGauge.fillAmount = GameManager.Instance.SpaceshipManager.GetGaugeValue(systemType) / totalGaugeValue;
    }
}