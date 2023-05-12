using System.Runtime.InteropServices;
using AutoKeyNet.WindowsHooks.WindowsEnums;

namespace AutoKeyNet.WindowsHooks.WindowsStruct;

/// <summary>
///     Contains information about a simulated mouse input event.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct MouseInput
{
    /// <summary>
    ///     The amount of relative motion in the horizontal direction.
    ///     This is signed relative to the last position of the mouse.
    /// </summary>
    internal int Dx;

    /// <summary>
    ///     The amount of relative motion in the vertical direction.
    ///     This is signed relative to the last position of the mouse.
    /// </summary>
    internal int Dy;

    /// <summary>
    ///     If dwFlags contains MOUSEEVENTF_WHEEL, this specifies the amount of wheel movement.
    ///     A positive value indicates that the wheel was rotated forward, away from the user;
    ///     a negative value indicates that the wheel was rotated backward, toward the user.
    /// </summary>
    internal int MouseData;

    /// <summary>
    ///     A set of bit flags that specify various aspects of mouse motion and button clicks.
    ///     The bits in this member can be any reasonable combination of the following values:
    /// </summary>
    internal MouseEvents Flags;

    /// <summary>
    ///     The time stamp for the event, in milliseconds.
    ///     If this parameter is 0, the system will provide its own time stamp.
    /// </summary>
    internal uint Time;

    /// <summary>
    ///     An additional value associated with the mouse event.
    /// </summary>
    internal nuint ExtraInfo;
}