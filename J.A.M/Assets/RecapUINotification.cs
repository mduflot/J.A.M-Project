using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RecapUINotification : UINotification
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            handler.RemoveNotification(this);
        }
        else
        { 
            GameManager.Instance.UIManager.recapUI.Initialize(task, this);
            
        }
    }
}
