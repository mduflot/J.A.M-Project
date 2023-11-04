using System;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class TimeTickSystem : MonoBehaviour
{
    public static uint timePerTick = 1; // InGame Time Unit
    private const float maxTimeScale = 5.0f;
    private const float minTimeScale = .5f;
    [SerializeField] private float timeScale = 1.0f;
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
        uint ticksPerTenMinutes = 5;
        uint ticksPerHour = ticksPerTenMinutes * 6;
        uint ticksPerDay = ticksPerHour * 24;
        
        uint days = currentTicks / ticksPerDay;
        currentTicks %= ticksPerDay;

        uint hours = currentTicks / ticksPerHour;
        currentTicks %= ticksPerHour;

        uint minutes = (currentTicks / ticksPerTenMinutes) * 10;
        
        return "Day : " + days.ToString("D2") + " // " + hours.ToString("D2") + ":" + minutes.ToString("D2");
    }

    public void ModifyTimeScale(float newScale)
    {
        timeScale = newScale;
    }

    public void IncreaseTimeScale(float factor)
    {
        timeScale += factor;
        timeScale = Mathf.Clamp(timeScale, minTimeScale, maxTimeScale);
    }
}

