namespace AutoKeyNet.WindowsHooks.WindowsEnums;

/// <summary>
///     Specifies the mouse events that can be sent to a window.
/// </summary>
[Flags]
internal enum MouseEvents : uint
{
    /// <summary>
    ///     The coordinates are absolute screen coordinates.
    /// </summary>
    ABSOLUTE = 0x8000,

    /// <summary>
    ///     The horizontal mouse wheel button is rotated.
    /// </summary>
    HWHEEL = 0x01000,

    /// <summary>
    ///     The mouse has been moved.
    /// </summary>
    MOVE = 0x0001,

    /// <summary>
    ///     The mouse has been moved, but coalescing must not be performed.
    /// </summary>
    MOVE_NOCOALESCE = 0x2000,

    /// <summary>
    ///     The left mouse button is pressed down.
    /// </summary>
    LEFTDOWN = 0x0002,

    /// <summary>
    ///     The left mouse button is released.
    /// </summary>
    LEFTUP = 0x0004,

    /// <summary>
    ///     The right mouse button is pressed down.
    /// </summary>
    RIGHTDOWN = 0x0008,

    /// <summary>
    ///     The right mouse button is released.
    /// </summary>
    RIGHTUP = 0x0010,

    /// <summary>
    ///     The middle mouse button is pressed down.
    /// </summary>
    MIDDLEDOWN = 0x0020,

    /// <summary>
    ///     The middle mouse button is released.
    /// </summary>
    MIDDLEUP = 0x0040,

    /// <summary>
    ///     The virtual desktop is used as the coordinate space.
    /// </summary>
    VIRTUALDESK = 0x4000,

    /// <summary>
    ///     The mouse wheel button is rotated.
    /// </summary>
    WHEEL = 0x0800,

    /// <summary>
    ///     An X button on the mouse is pressed down.
    /// </summary>
    XDOWN = 0x0080,

    /// <summary>
    ///     An X button on the mouse is released.
    /// </summary>
    XUP = 0x0100
}