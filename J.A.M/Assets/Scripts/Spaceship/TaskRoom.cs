using SS;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Spaceship
{
    [RequireComponent(typeof(SSLauncher))]
    public class TaskRoom : MonoBehaviour, IDropHandler, IPointerDownHandler
    {
        private SSLauncher launcher;
        private float clickTime;
        private uint clicked;
        private float clickDelay = 0.5f;

        private void Start()
        {
            launcher = GetComponent<SSLauncher>();
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (GameManager.Instance.SpaceshipManager.IsInTutorial) return;
            GameObject dropped = eventData.pointerDrag;
            var icon = dropped.GetComponent<CharacterIcon>();
            if (icon != null)
            {
                launcher.StartTimelineOnDrop(icon);
                GameManager.Instance.SpaceshipManager.DisplayRooms(false);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (GameManager.Instance.SpaceshipManager.IsInTutorial) return;
            if (eventData.button != PointerEventData.InputButton.Left) return;
            clicked++;
            if (clicked == 1) clickTime = Time.time;
            if (clicked > 1 && Time.time - clickTime < clickDelay)
            {
                clicked = 0;
                clickTime = 0;
                launcher.StartTimeline();
            }
            else if (clicked >= 2 && Time.time - clickTime > clickDelay) clicked = 0;
        }
    }
}