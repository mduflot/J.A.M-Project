using TMPro;

public class TraitsHoverMenu : HoverMenu
{
    public TextMeshProUGUI traitTitle;
    public TextMeshProUGUI traitDescription;
    
    public override void Initialize(HoverMenuData data)
    {
        base.Initialize(data);
        traitTitle.text = data.text1;
        traitDescription.text = data.text2;
    }
}