using System;

namespace DONT_TOUCH.Enums
{
    [Flags]
    public enum GeneratorFlags : byte
    {
        None = 1,
        Unlocked = 2,
        Open = 4,
        Activating = 8,
        Engaged = 16, // 0x10
    }
}