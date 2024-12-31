using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;

public class UINotificationsHandler : MonoBehaviour
{
    [SerializeField] private List<UINotification> notifications;
    [SerializeField] private int maxNotifications;
    [SerializeField] private UINotification taskNotificationPrefab;
    [SerializeField] private UINotification recapNotificationPrefab;
    [SerializeField] private Transform notificationsParent;
    [SerializeField] private float maxRightPosX;
    [SerializeField] private int spacing;

    private Queue<Notification> notificationsQueue = new();
    private Queue<Notification> recapNotificationsQueue = new();

    /// <summary>
    /// Called when task is created
    /// </summary>
    /// <param name="n">Notification to display</param>
    public void CreateTaskNotification(Notification n)
    {
        if (notifications.Count >= maxNotifications)
        {
            notificationsQueue.Enqueue(n);
            return;
        }

        var note = Instantiate(taskNotificationPrefab, notificationsParent);
        note.Initialize(n.Task, this, notifications.Count, true, n);
        notifications.Add(note);
        n.uiNotification = note;
        ReplaceNotifications();
    }

    /// <summary>
    /// Called when task is completed
    /// </summary>
    /// <param name="n">Notification to display</param>
    public void CreateRecapNotification(Notification n)
    {
        if (notifications.Count >= maxNotifications)
        {
            recapNotificationsQueue.Enqueue(n);
            return;
        }

        var note = Instantiate(recapNotificationPrefab, notificationsParent);
        note.Initialize(n.Task, this, notifications.Count, true);
        notifications.Add(note);
        ReplaceNotifications();
    }

    /// <summary>
    /// Called when task needs to be removed
    /// </summary>
    /// <param name="n">UINotification to remove</param>
    public async void RemoveNotification(UINotification n)
    {
        if (n)
        {
            if (notifications.Contains(n))
            {
                await n.Disappear();
                notifications.Remove(n);
            }
            else if (notificationsQueue.Contains(n.notification))
                notificationsQueue = new Queue<Notification>(notificationsQueue.Where(x => x != n.notification));
            else if (recapNotificationsQueue.Contains(n.notification))
                recapNotificationsQueue = new Queue<Notification>(recapNotificationsQueue.Where(x => x != n.notification));
        }

        ReplaceNotifications();

        if (notificationsQueue.Count > 0 && notifications.Count < maxNotifications)
        {
            var newNotification = notificationsQueue.Dequeue();
            CreateTaskNotification(newNotification);
        }
        else if (recapNotificationsQueue.Count > 0 && notifications.Count < maxNotifications)
        {
            var newNotification = recapNotificationsQueue.Dequeue();
            CreateRecapNotification(newNotification);
        }
    }

    /// <summary>
    /// Called when notifications need to be replaced
    /// </summary>
    private void ReplaceNotifications()
    {
        for (int i = 0; i < notifications.Count; i++)
        {
            notifications[i].MoveToNewPos(maxRightPosX - (spacing * i - 1));
        }
    }
}