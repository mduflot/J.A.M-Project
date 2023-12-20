using System;
using UnityEngine;

namespace Managers
{
    public class TimeTickSystem : MonoBehaviour
    {
        public static uint timePerTick = 1; // InGame Time Unit
        public static uint ticksPerHour = 48; 
        public static float timeScale = 1.0f;

        public static float lastActiveTimeScale = 1.0f;
        //[SerializeField] private const uint ticksPerTenMinutes = 5;
        public class OnTickEventArgs : EventArgs
        {
            public uint tick;
        }

        public static event EventHandler<OnTickEventArgs> OnTick;
    
        private const float tickTimerMax = .2f;

        private uint tick;
        private float tickTimer;
    
        private void Awake()
        {
            tick = 0;
        }

        private void Update()
        {
            tickTimer += Time.deltaTime * timeScale;
            if (tickTimer >= tickTimerMax)
            {
                tickTimer -= tickTimerMax;
                tick++;
                if (OnTick != null) OnTick(this, new OnTickEventArgs { tick = tick });
            }
        }
    
        public static string GetTimeAsInGameDate(OnTickEventArgs e)
        {
            uint currentTicks = e.tick;
            //ticksPerHour = ticksPerTenMinutes * 6;
            uint ticksPerTenMinutes = ticksPerHour/6;
            uint ticksPerDay = ticksPerHour * 24;
        
            uint days = currentTicks / ticksPerDay;
            currentTicks %= ticksPerDay;

            uint hours = currentTicks / ticksPerHour;
            currentTicks %= ticksPerHour;

            uint minutes = (currentTicks / ticksPerTenMinutes) * 10;
        
            return "Day : " + days.ToString("D2") + " // " + hours.ToString("D2") + ":" + minutes.ToString("D2");
        }

        public static string GetTicksAsTime(uint t)
        {
            uint ticks = t;
            uint ticksPerTenMinutes = ticksPerHour / 6;

            uint hours = ticks / ticksPerHour;
            ticks %= ticksPerHour;

            uint minutes = (ticks / ticksPerTenMinutes) * 10;

            return hours.ToString("D2") + ":" + minutes.ToString("D2");
        }

        public static void ModifyTimeScale(float newScale)
        {
            if (newScale != 0) lastActiveTimeScale = newScale;
            timeScale = newScale;
        }
    }
}