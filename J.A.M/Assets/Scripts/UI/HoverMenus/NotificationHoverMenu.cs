using TMPro;
using UnityEngine;

public class NotificationHoverMenu : HoverMenu
{
    [SerializeField] private SpriteRenderer[] charIcons;
    [SerializeField] private TextMeshPro taskName;
    [SerializeField] private TextMeshPro taskDuration;

    public override void Initialize(HoverMenuData data)
    {
        foreach (var icon in charIcons)
        {
            icon.gameObject.SetActive(false);
        }

        taskName.text = data.text1;
        taskDuration.text = data.text2;
        if (data.Task.leaderCharacters.Count > 0)
        {
            for (int i = 0; i < data.Task.assistantCharacters.Count + 1; i++)
            {
                charIcons[i].gameObject.SetActive(true);
                charIcons[i].sprite =
                    i == 0 ? data.Task.leaderCharacters[0].GetSprite() : data.Task.assistantCharacters[i - 1].GetSprite();
            }
        }

        base.Initialize(data);
    }

    public override void UpdateMenu(HoverMenuData data)
    {
        taskDuration.text = data.text2;
    }
}