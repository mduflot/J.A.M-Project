using TMPro;
using UnityEngine;

public class TraitHoverable : HoverableObject
{
    [SerializeField] private TextMeshProUGUI traitTitle;
    private string text;
    public void Initialize(string title)
    {
        text = title;
        traitTitle.text = text;
    }

    public string GetName()
    {
        return traitTitle.text;
    }

    public void ChangeColor(bool isPositive)
    {
        traitTitle.text = (isPositive ? "<color=green>" : "<color=red>") + text;
    }
}
