using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TaskUINotification : UINotification
{
    [SerializeField] private Image fill; 
    public override void OnPointerDown(PointerEventData eventData)
    {
        notification.Display();
    }

    public override void UpdateFill(float value)
    {
        fill.fillAmount = value;
    }
}
