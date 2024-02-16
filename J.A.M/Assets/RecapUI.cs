using System.Collections;
using System.Collections.Generic;
using Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecapUI : MonoBehaviour
{
    [SerializeField] private Image taskIcon;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI outcomesText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    private RecapUINotification notification;
    public void Initialize(Task task, RecapUINotification n)
    {
        taskIcon.sprite = task.Icon;
        titleText.text = task.Name;
        descriptionText.text = task.Description;
        outcomesText.text = task.previewText;
        notification = n;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        GameManager.Instance.UIManager.UINotificationsHandler.RemoveNotification(notification);
        gameObject.SetActive(false);
    }
}
