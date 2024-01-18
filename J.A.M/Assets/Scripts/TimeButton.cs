using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeButton : MonoBehaviour
{
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite defaultIconSprite;
    [SerializeField] private Sprite selectedIconSprite;

    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;

    public void SelectButton()
    {
        iconImage.sprite = selectedIconSprite;
        backgroundImage.sprite = selectedSprite;
    }

    public void DeselectButton()
    {
        iconImage.sprite = defaultIconSprite;
        backgroundImage.sprite = defaultSprite;
    }
    
    
}
