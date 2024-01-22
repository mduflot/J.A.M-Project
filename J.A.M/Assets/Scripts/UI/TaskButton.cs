using SS;
using SS.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(SSLauncher))]
public class TaskButton : HoverableObject
{
    [SerializeField] private TextMeshProUGUI taskName;
    [SerializeField] private TextMeshProUGUI taskDuration;
    [SerializeField] private Image taskIcon;
    [TextArea(4, 10)]
    [SerializeField] private string taskShortDescription;
    private SSTaskNodeSO task;
    private SSLauncher launcher;
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;

    private void Start()
    {
        launcher = GetComponent<SSLauncher>();
        task = (SSTaskNodeSO)launcher.node;
        taskName.text = task.name;
        taskDuration.text = task.Duration + " hours";
        taskIcon.sprite = task.Icon;
        data = new HoverMenuData();
        data.text1 = taskShortDescription;
        data.baseParent = transform;
        data.parent = transform.parent.parent.parent;
    }

    public void OnClick()
    {
        SoundManager.Instance.PlaySound(clickSound);
    }

    public override void OnHover(PointerEventData eventData)
    {
        base.OnHover(eventData);
        SoundManager.Instance.PlaySound(hoverSound);
    }
}
