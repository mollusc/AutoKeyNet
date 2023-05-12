using System.Runtime.InteropServices;

namespace AutoKeyNet.WindowsHooks.WindowsStruct;

/// <summary>
///     The MOUSEHOOKSTRUCT structure contains information about a mouse event passed to a WH_MOUSE hook procedure,
///     MouseProc.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct MouseLowLevelHook
{
    /// <summary>
    ///     Specifies a Point structure that contains the x- and y-coordinates of the cursor, in screen coordinates.
    /// </summary>
    public Point Point;

    /// <summary>
    ///     Specifies the wheel delta. The value is a multiple of WHEEL_DELTA, which is set to 120.
    /// </summary>
    public int MouseData;

    /// <summary>
    ///     Specifies additional information associated with the message.
    /// </summary>
    public int Flags;

    /// <summary>
    ///     Specifies the time stamp for this message.
    /// </summary>
    public int Time;

    /// <summary>
    ///     Specifies an additional value associated with the message.
    /// </summary>
    public nuint ExtraInfo;
}