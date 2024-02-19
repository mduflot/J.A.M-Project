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
    [SerializeField] private Animator animator;
    private RecapUINotification notification;
    public void Initialize(Task task, RecapUINotification n)
    {
        taskIcon.sprite = task.Icon;
        titleText.text = task.Name;
        outcomesText.text = task.previewText;
        notification = n;
        Appear(true);
    }

    public void Appear(bool state)
    {
        switch (state)
        {
            case true :
                animator.SetBool("Appear", state);
                break;
            case false : 
                animator.SetBool("Appear", state);
                GameManager.Instance.UIManager.UINotificationsHandler.RemoveNotification(notification);
                break;
        }
    }
}
