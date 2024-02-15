using System.Collections;
using System.Collections.Generic;
using Tasks;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class TaskUINotification : UINotification
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        notification.Display();
    }
}
