using TMPro;
using UnityEngine;

public class TraitHoverable : HoverableObject
{
    [SerializeField] private TextMeshProUGUI traitTitle;
    
    public void Initialize(string title)
    {
        traitTitle.text = title;
    }
}
