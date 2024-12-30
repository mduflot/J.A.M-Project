using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TaskUINotification : UINotification
{
    [SerializeField] private Image timeLeftFill;
    [SerializeField] private Image completionFill;

    public override void OnPointerDown(PointerEventData eventData)
    {
        notification.Display();
    }

    public override void UpdateTimeLeftFill(float value)
    {
        timeLeftFill.enabled = true;
        timeLeftFill.fillAmount = value;
    }

    public override void UpdateCompletionFill(float value)
    {
        timeLeftFill.enabled = false;
        completionFill.fillAmount = 1 - value;
    }
}