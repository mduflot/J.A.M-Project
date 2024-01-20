using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DefaultGaugeUI : GaugeUI
{
    [SerializeField] private Image gauge;
    [SerializeField] private Image previewGauge;
    [SerializeField] private Image arrow;
    [SerializeField] private Sprite greenArrow;
    [SerializeField] private Sprite redArrow;

    private Color colorOpaque = new(1.0f, 1.0f, 1.0f, 1.0f);
    private Color colorTransparent = new(1.0f, 1.0f, 1.0f, 0.0f);
    private bool isHovered;

    public override void UpdateGauge(float value, float previewValue)
    {
        gauge.fillAmount = value / 50;
        if (!IsPreviewing || (previewGauge.fillAmount > (value + previewValue) / 50))
            previewGauge.fillAmount = (value + previewValue) / 50;
        if (previewValue > 0.0f)
        {
            arrow.sprite = greenArrow;
            arrow.color = colorOpaque;
        }
        else if (GameManager.Instance.SpaceshipManager.systems.Any(system => system.type == systemType && system.isBlocked))
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

    public override void PreviewOutcomeGauge(float value)
    {
        if (value > 0)
        {
            value += gauge.fillAmount * 50;
            previewGauge.fillAmount = value / 50;
            arrow.sprite = greenArrow;
        }
        else
        {
            previewGauge.fillAmount = GameManager.Instance.SpaceshipManager.GetGaugeValue(systemType) / 50;
            gauge.fillAmount = (GameManager.Instance.SpaceshipManager.GetGaugeValue(systemType) + value) / 50;
            arrow.sprite = redArrow;
        }
    }

    public override void ResetPreviewGauge()
    {
        previewGauge.fillAmount = 0;
    }
}