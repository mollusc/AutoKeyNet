namespace AutoKeyNet.WindowsHooks.WindowsEnums;

/// <summary>
///     Specifies the flags that can be used with a keyboard input event.
/// </summary>
[Flags]
public enum KeyEventFlags : uint
{
    /// <summary>
    ///     Specifies a key down event.
    /// </summary>
    KEYDOWN = 0x000,

    /// <summary>
    ///     Specifies an extended key event.
    /// </summary>
    EXTENDEDKEY = 0x0001,

    /// <summary>
    ///     Specifies a key up event.
    /// </summary>
    KEYUP = 0x0002,

    /// <summary>
    ///     Specifies that the scan code of the key should be used instead of the virtual key code.
    /// </summary>
    SCANCODE = 0x0008,

    /// <summary>
    ///     Specifies that the key is a Unicode character.
    /// </summary>
    UNICODE = 0x0004
}