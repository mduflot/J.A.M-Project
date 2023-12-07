using System;
using SS;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Spaceship
{
    [RequireComponent(typeof(SSLauncher))]
    public class TaskRoom : MonoBehaviour, IDropHandler
    {
        
        private SSLauncher launcher;
        private void Start()
        {
            launcher = GetComponent<SSLauncher>();
        }

        public void OnDrop(PointerEventData eventData)
        {
            GameObject dropped = eventData.pointerDrag;
            var icon = dropped.GetComponent<CharacterIcon>();
            if(icon != null) launcher.StartTimeline();
        }
    }
}
