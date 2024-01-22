using TMPro;

public class GaugeHoverMenu : HoverMenu
{
    public TextMeshProUGUI gaugeTitle;
    public TextMeshProUGUI gaugeDescription;
    
    public override void Initialize(HoverMenuData data)
    {
        base.Initialize(data);
        gaugeTitle.text = data.text1;
        gaugeDescription.text = data.text2;
    }
}