using Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class WindowCloser : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private TaskUI taskUI;
    [SerializeField] private AudioClip taskCloseSound;

    public void OnPointerDown(PointerEventData eventData)
    {
        taskUI.CloseTask();
        SoundManager.Instance.PlaySound(taskCloseSound);
    }
}