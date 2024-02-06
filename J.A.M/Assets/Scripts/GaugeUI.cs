using System.Linq;
using SS;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class GaugeUI : HoverableObject, IPointerDownHandler
{
    public SystemType systemType;
    public bool IsPreviewing;
    public GameObject parentGauge;
    public SSLauncher launcher;
    [SerializeField] private AudioClip hoverSound;

    public void InitializeGauge()
    {
        data = new HoverMenuData
        {
            text1 = systemType.ToString(),
            text2 = "Decrease : " + GameManager.Instance.SpaceshipManager.systems
                .First(system => system.type == systemType).decreaseValues[0],
            baseParent = parentGauge.transform,
            parent = transform
        };
        hoverMenu.Initialize(data);
        hoverMenu.gameObject.SetActive(false);
    }

    public abstract void UpdateGauge(float value, float previewValue);

    public abstract void AddPreviewGauge(float value);

    public void OnPointerDown(PointerEventData eventData)
    {
        launcher.StartTimeline();
    }

    public override void OnHover(PointerEventData eventData)
    {
        base.OnHover(eventData);
        SoundManager.Instance.PlaySound(hoverSound);
    }
}