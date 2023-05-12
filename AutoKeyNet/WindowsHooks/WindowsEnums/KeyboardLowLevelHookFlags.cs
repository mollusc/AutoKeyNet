namespace AutoKeyNet.WindowsHooks.WindowsEnums;

/// <summary>
///     Specifies the flags that can be used with the low-level keyboard input event hook.
/// </summary>
[Flags]
internal enum KeyboardLowLevelHookFlags : uint
{
    /// <summary>
    ///     Specifies if the key is an extended key.
    /// </summary>
    LLKHF_EXTENDED = 0x01,

    /// <summary>
    ///     Specifies if the event was injected by the SendInput function.
    /// </summary>
    LLKHF_INJECTED = 0x10,

    /// <summary>
    ///     Specifies if the key is an extended key with the ALT key down.
    /// </summary>
    LLKHF_ALTDOWN = 0x20,

    /// <summary>
    ///     Specifies if the key is an up event.
    /// </summary>
    LLKHF_UP = 0x80
}