using TMPro;

public class GaugeHoverMenu : HoverMenu
{
    public TextMeshProUGUI gaugeTitle;
    public TextMeshProUGUI gaugeDescription;
    public string descriptionToAdd;

    public override void Initialize(HoverMenuData data)
    {
        base.Initialize(data);
        gaugeTitle.text = data.text1;
        gaugeDescription.text = data.text2 + descriptionToAdd;
    }

    public override void UpdateMenu(string description)
    {
        descriptionToAdd += description;
    }

    public override void ResetMenu()
    {
        descriptionToAdd = "";
    }
}