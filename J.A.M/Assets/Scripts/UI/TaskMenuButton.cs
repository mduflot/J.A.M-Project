using UnityEngine;
using UnityEngine.EventSystems;

public class TaskMenuButton : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private TasksMenu menu;
    [SerializeField] private bool computes;
    [SerializeField] private AudioClip hoverSound;

    public void OnPointerEnter(PointerEventData eventData)
    {
        menu.ClickedMenu(computes);
        SoundManager.Instance.PlaySound(hoverSound);
    }
    
}
