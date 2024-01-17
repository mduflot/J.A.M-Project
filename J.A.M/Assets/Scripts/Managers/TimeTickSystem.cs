using System;
using UnityEngine;

namespace Managers
{
    public class TimeTickSystem : MonoBehaviour, IDataPersistence
    {
        public static uint timePerTick = 1; // InGame Time Unit
        public static uint ticksPerHour = 48;
        public static float timeScale = 3.0f;

        public static int lastActiveTimeScale = 1;

        public static int pauseScale = 0;

        public static int playScale = 2;

        public static int quickPlayScale = 5;
        //[SerializeField] private const uint ticksPerTenMinutes = 5;

        [SerializeField] private TimeButton[] timeButtons;

        public class OnTickEventArgs : EventArgs
        {
            public uint tick;
        }

        public static event EventHandler<OnTickEventArgs> OnTick;

        private const float tickTimerMax = .2f;

        private uint tick;
        private float tickTimer;

        delegate void TimeScaleChanged(int scale);

        private static TimeScaleChanged timeScaleChanged;
    
        private void Awake()
        {
            tick = 0;
            timeScaleChanged = UpdateTimeButtons;
        }

        private void Start()
        {
            timeButtons[1].SelectButton();
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
            uint ticksPerTenMinutes = ticksPerHour / 6;
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

        public static void ModifyTimeScale(int scale)
        {
            var newScale = 0;

            switch (scale)
            {
                case 0:
                    newScale = pauseScale;
                    break;

                case 1:
                    newScale = playScale;
                    break;

                case 2:
                    newScale = quickPlayScale;
                    break;
                default:
                    Debug.Log("Error while setting time scale");
                    break;
            }
            timeScaleChanged(scale);
            if (newScale != 0) lastActiveTimeScale = scale;
            timeScale = newScale;
        }

        public void UpdateTimeButtons(int index)
        {
            foreach (var button in timeButtons)
            {
                button.DeselectButton();
            }
            timeButtons[index].SelectButton();
        }

        public void LoadData(GameData gameData)
        {
            tick = gameData.time;
        }

        public void SaveData(ref GameData gameData)
        {
            gameData.time = tick;
        }
    }
}