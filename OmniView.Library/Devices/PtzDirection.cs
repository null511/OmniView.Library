using System;

namespace OmniView.Library.Devices
{
    [Flags]
    public enum PtzDirection
    {
        None = 0,
        Left = 1,
        Up = 2,
        Right = 4,
        Down = 8,
    }
}
