using Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class WindowCloser : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private TaskUI taskUI;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        taskUI.CloseTask();
    }
}
