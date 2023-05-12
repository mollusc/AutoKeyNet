using System.Runtime.InteropServices;
using AutoKeyNet.WindowsHooks.WindowsEnums;

namespace AutoKeyNet.WindowsHooks.WindowsStruct;

/// <summary>
///     Contains information about a low-level keyboard hook event.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct KeyboardLowLevelHook
{
    /// <summary>
    ///     Specifies a virtual-key code. The code must be a value in the range 1 to 254.
    /// </summary>
    public VirtualKey VirtualKey;

    /// <summary>
    ///     Specifies a hardware scan code for the key.
    /// </summary>
    public uint ScanCode;

    /// <summary>
    ///     Specifies various aspects of the event, such as extended key status, context code, and transition state.
    /// </summary>
    public KeyboardLowLevelHookFlags Flags;

    /// <summary>
    ///     Specifies the time stamp for this message.
    /// </summary>
    public uint Time;

    /// <summary>
    ///     Specifies additional information associated with the message.
    /// </summary>
    public nuint ExtraInfo;
}