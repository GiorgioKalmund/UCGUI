using System;

namespace UCGUI
{
    [Flags]
    public enum DebugOptions 
    {
        None      = 0,
        TextOnly = 1 << 0,
        RectOnly = 1 << 1,
        Default = TextOnly | RectOnly,
    }
}