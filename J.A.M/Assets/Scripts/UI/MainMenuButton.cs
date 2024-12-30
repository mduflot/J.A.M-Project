using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuButton : HoverableObject
{
    [SerializeField] private AudioClip menuButtonHover;
    [SerializeField] private AudioClip menuButtonClick;

    public override void OnHover(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySound(menuButtonHover);
    }

    public override void OnExit(PointerEventData eventData)
    {
    }

    public void OnClick()
    {
        SoundManager.Instance.PlaySound(menuButtonClick);
    }
}