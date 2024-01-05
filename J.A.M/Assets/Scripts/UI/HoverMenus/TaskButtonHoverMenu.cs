using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskButtonHoverMenu : HoverMenu
{
    [SerializeField] private TextMeshProUGUI taskDescription;
    public override void Initialize(HoverMenuData data)
    {
        base.Initialize(data);
        taskDescription.text = data.text1;
        transform.parent = data.parent;
    }
}
