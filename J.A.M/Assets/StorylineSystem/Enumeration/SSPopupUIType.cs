using System;

namespace SS.Enumerations
{
    [Flags]
    public enum SSPopupUIType
    {
        None = 0,
        Gauges = 1,
        Tasks = 2,
        Spaceship = 4
    }
}