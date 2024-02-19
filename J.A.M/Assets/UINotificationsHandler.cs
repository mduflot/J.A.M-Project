using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UI;
using Unity.Mathematics;
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

    public void CreateTaskNotification(Notification n)
    {
        var note = Instantiate(taskNotificationPrefab, notificationsParent);
        var position = new Vector3(maxRightPosX - spacing * notifications.Count - 1,
            0);
        note.transform.localPosition = position;
        note.Initialize(n.Task, this, notifications.Count, n);
        notifications.Add(note);
        n.uiNotification = note;
    }
    
    public void CreateRecapNotification(Notification n)
    {
        var note = Instantiate(recapNotificationPrefab, notificationsParent);
        var position = new Vector3(maxRightPosX - (spacing * notifications.Count - 1),
            0);
        note.transform.localPosition = position;
        note.Initialize(n.Task, this, notifications.Count);
        notifications.Add(note);
    }

    public async void RemoveNotification(UINotification n)
    {
        await n.Disappear();
        notifications.Remove(n);
        for (int i = n.index; i < notifications.Count; i++)
        {
            notifications[i].MoveToNewPos(maxRightPosX - (spacing * i - 1));
        }
    }
}
