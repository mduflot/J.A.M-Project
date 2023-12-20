using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoUI : MonoBehaviour
{
    [SerializeField] private Image icon;

    [SerializeField] private TextMeshProUGUI name;

    [SerializeField] private TextMeshProUGUI traits;


    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetupCharacterInfo(CharacterDataScriptable c)
    {
        icon.sprite = c.characterIcon;
        name.text = c.firstName + " " + c.lastName;
        gameObject.SetActive(true);
    }

    public void ClearCharacterInfo()
    {
        icon.sprite = null;
        name.text = null;
        traits.text = null;
        gameObject.SetActive(false);
    }
}
