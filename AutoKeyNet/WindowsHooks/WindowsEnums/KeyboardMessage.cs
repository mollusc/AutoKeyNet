namespace AutoKeyNet.WindowsHooks.WindowsEnums;

/// <summary>
///     Specifies the keyboard messages that can be sent to a window.
/// </summary>
internal enum KeyboardMessage
{
    /// <summary>
    ///     A non-system key is being pressed down.
    /// </summary>
    WM_KEYDOWN = 0x0100,

    /// <summary>
    ///     A non-system key is being released.
    /// </summary>
    WM_KEYUP = 0x0101,

    /// <summary>
    ///     The ALT key is being pressed down.
    /// </summary>
    WM_SYSKEYDOWN = 0x0104,

    /// <summary>
    ///     The ALT key is being released.
    /// </summary>
    WM_SYSKEYUP = 0x0105
}