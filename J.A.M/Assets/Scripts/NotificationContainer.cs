using UI;
using UnityEngine;

public class NotificationContainer : MonoBehaviour
{
    private Notification[] children;

    public void DisplayNotification()
    {
        children = GetComponentsInChildren<Notification>();
        var startX = 0 - ((children.Length - 1) * 75) / 2;
        for (int index = 0; index < children.Length; index++)
        {
            var child = children[index];
            child.transform.localPosition = new Vector3(startX + (index * 75), 0, 0);
        }
    }
}