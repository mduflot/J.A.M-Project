using UnityEngine;
using UnityEngine.EventSystems;

public class HoverableObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public HoverMenu hoverMenu;
    public HoverMenuData data;

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHover(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnExit(eventData);
    }

    public virtual void OnHover(PointerEventData eventData)
    {
        hoverMenu?.Initialize(data);
    }

    public virtual void OnExit(PointerEventData eventData)
    {
        hoverMenu.QuitMenu(data);
    }
}
