using System;
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

    protected Task task;
    protected UINotificationsHandler handler;
    public int index;

    private bool hasToMove;
    private Vector3 initialPos;
    private Vector3 newPos;
    private float lerpValue = 0;
    public bool isActive;

    public void Initialize(Task t, UINotificationsHandler h, int i, bool b = false, Notification n = null)
    {
        notification = n;
        task = t;
        icon.sprite = task.Icon;
        handler = h;
        animator.SetBool("Appear", true);
        index = i;
        isActive = b;
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

    public async System.Threading.Tasks.Task Disappear()
    {
        animator.SetBool("Appear", false);
        await System.Threading.Tasks.Task.Delay(500);
        Destroy(gameObject);
    }

    public void MoveToNewPos(float newPos)
    {
        hasToMove = true;
        initialPos = transform.localPosition;
        this.newPos = new Vector3(newPos, 0);
    }

    private void FixedUpdate()
    {
        if (hasToMove)
        {
            transform.localPosition = Vector3.Lerp(initialPos, newPos, lerpValue);
            lerpValue += Time.deltaTime * 2;
            if (lerpValue >= 1)
            {
                lerpValue = 0;
                hasToMove = false;
            }
        }
    }

    public virtual void UpdateTimeLeftFill(float value)
    {
    }

    public virtual void UpdateCompletionFill(float value)
    {
    }
}