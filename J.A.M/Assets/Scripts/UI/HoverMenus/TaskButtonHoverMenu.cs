using TMPro;
using UnityEngine;

public class TaskButtonHoverMenu : HoverMenu
{
    [SerializeField] private TextMeshProUGUI taskDescription;

    public override void Initialize(HoverMenuData data)
    {
        base.Initialize(data);
        taskDescription.text = data.text1;
        transform.SetParent(data.parent);
    }
}