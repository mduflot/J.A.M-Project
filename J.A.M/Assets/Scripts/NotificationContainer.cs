using UI;
using UnityEngine;

public class NotificationContainer : MonoBehaviour
{
    private Notification[] children;

    public void DisplayNotification()
    {
        children = GetComponentsInChildren<Notification>();
        for (int index = 0; index < children.Length; index++)
        {
            var child = children[index];
            child.transform.localPosition = new Vector3(0, -index * 100, 0);
        }
    }
}