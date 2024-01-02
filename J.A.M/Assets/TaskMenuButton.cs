using UnityEngine;
using UnityEngine.EventSystems;

public class TaskMenuButton : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private TasksMenu menu;
    [SerializeField] private bool computes;

    public void OnPointerEnter(PointerEventData eventData)
    {
        menu.ClickedMenu(computes);
    }
    
}
