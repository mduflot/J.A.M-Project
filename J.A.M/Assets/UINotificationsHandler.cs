using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class UINotificationsHandler : MonoBehaviour
{
    [SerializeField] private List<UINotification> notifications;
    [SerializeField] private int maxNotifications;
    [SerializeField] private UINotification taskNotificationPrefab;
    [SerializeField] private UINotification recapNotificationPrefab;
    [SerializeField] private Transform notificationsParent;

    public void CreateTaskNotification(Notification n)
    {
        var note = Instantiate(taskNotificationPrefab, notificationsParent);
        note.Initialize(n.Task, this, n);
        notifications.Add(note);
        n.uiNotification = note;
    }
    
    public void CreateRecapNotification(Notification n)
    {
        var note = Instantiate(recapNotificationPrefab, notificationsParent);
        note.Initialize(n.Task, this);
        notifications.Add(note);
    }

    public void RemoveNotification(UINotification n)
    {
        notifications.Remove(n);
        n.Disappear();
    }
}
