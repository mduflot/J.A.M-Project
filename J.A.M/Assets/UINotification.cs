using System.Collections;
using Tasks;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UINotification : HoverableObject, IPointerDownHandler
{
    [SerializeField] private Image icon;

    [SerializeField] private Animator animator;

    public Notification notification;

    public Task task;

    public virtual void Initialize(Task t, UINotificationsHandler handler, Notification n = null)
    {
        notification = n;
        task = t;
        icon.sprite = task.Icon;
        animator.SetBool("Appear", true);
    }

    public override void OnHover(PointerEventData eventData)
    {
        animator.SetBool("Hovered", true);
    }

    public override void OnExit(PointerEventData eventData)
    {
        animator.SetBool("Hovered", false);
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        
    }
    
    public async void Disappear()
    {
        animator.SetBool("Appear", false);
        await System.Threading.Tasks.Task.Delay(1000);
        Destroy(gameObject);
    }
}
