using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterProfile : MonoBehaviour
{
    [Header("Character Data")]
    public CharacterDataScriptable characterData;
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI nameText;

    [Header("Ship Control")] 
    [SerializeField] private Button characterButton;
    private ShipControlManager shipControlManager;

    public void Initialize(CharacterDataScriptable data, Sprite image, string name, ShipControlManager shipControlManager)
    {
        characterData = data;
        characterImage.sprite = image;
        nameText.text = name;
        this.shipControlManager = shipControlManager;
        characterButton.onClick.AddListener(() => this.shipControlManager.DisplayCharacterInfo(characterData));
    }
}