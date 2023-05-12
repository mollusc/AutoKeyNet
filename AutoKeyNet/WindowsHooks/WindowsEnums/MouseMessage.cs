namespace AutoKeyNet.WindowsHooks.WindowsEnums;

/// <summary>
///     Defines the mouse messages that can be sent to a window.
/// </summary>
internal enum MouseMessage
{
    /// <summary>
    ///     The mouse has been moved.
    /// </summary>
    WM_MOUSEMOVE = 0x200,

    /// <summary>
    ///     The left mouse button is pressed.
    /// </summary>
    WM_LBUTTONDOWN = 0x201,

    /// <summary>
    ///     The left mouse button is released.
    /// </summary>
    WM_LBUTTONUP = 0x202,

    /// <summary>
    ///     The left mouse button is double-clicked.
    /// </summary>
    WM_LBUTTONDBLCLK = 0x203,

    /// <summary>
    ///     The right mouse button is pressed.
    /// </summary>
    WM_RBUTTONDOWN = 0x204,

    /// <summary>
    ///     The right mouse button is released.
    /// </summary>
    WM_RBUTTONUP = 0x205,

    /// <summary>
    ///     The right mouse button is double-clicked.
    /// </summary>
    WM_RBUTTONDBLCLK = 0x206,

    /// <summary>
    ///     The middle mouse button is pressed.
    /// </summary>
    WM_MBUTTONDOWN = 0x207,

    /// <summary>
    ///     The middle mouse button is released.
    /// </summary>
    WM_MBUTTONUP = 0x208,

    /// <summary>
    ///     The middle mouse button is double-clicked.
    /// </summary>
    WM_MBUTTONDBLCLK = 0x209,

    /// <summary>
    ///     The mouse wheel is rotated.
    /// </summary>
    WM_MOUSEWHEEL = 0x20A,

    /// <summary>
    ///     The mouse horizontal wheel is rotated.
    /// </summary>
    WM_MOUSEHWHEEL = 0x20E,

    /// <summary>
    ///     The first or second X button is pressed.
    /// </summary>
    WM_XBUTTONDOWN = 0x020B,

    /// <summary>
    ///     The first or second X button is released.
    /// </summary>
    WM_XBUTTONUP = 0x020C
}