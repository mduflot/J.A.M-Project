using UnityEngine.EventSystems;

public class RecapUINotification : UINotification
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        // Remove notification on right click
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            handler.RemoveNotification(this);
        }
        else
        { 
            GameManager.Instance.UIManager.recapUI.Initialize(task);
        }
    }
}
