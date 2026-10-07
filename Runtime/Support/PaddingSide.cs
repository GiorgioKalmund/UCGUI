using System;

namespace UCGUI
{
    [Flags]
    public enum PaddingSide
    {
        None      = 0,
        Leading   = 1 << 0,
        Trailing  = 1 << 1,
        Top       = 1 << 2,
        Bottom    = 1 << 3,
        Horizontal = Leading | Trailing,
        Vertical   = Top | Bottom,
        All = Bottom | Top | Leading | Trailing 
    }

}