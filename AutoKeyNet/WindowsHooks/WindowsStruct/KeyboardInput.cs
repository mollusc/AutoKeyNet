using System.Runtime.InteropServices;
using AutoKeyNet.WindowsHooks.WindowsEnums;

namespace AutoKeyNet.WindowsHooks.WindowsStruct;

/// <summary>
///     Contains information about a keyboard input event.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct KeyboardInput
{
    /// <summary>
    ///     The virtual-key code of the key.
    /// </summary>
    public ushort VirtualKey;

    /// <summary>
    ///     The hardware scan code of the key.
    /// </summary>
    public ushort ScanCode;

    /// <summary>
    ///     Additional information about the key event.
    /// </summary>
    public KeyEventFlags Flags;

    /// <summary>
    ///     The time stamp for the event, in milliseconds.
    /// </summary>
    public int Time;

    /// <summary>
    ///     An additional value associated with the keystroke.
    /// </summary>
    public nuint ExtraInfo;
}