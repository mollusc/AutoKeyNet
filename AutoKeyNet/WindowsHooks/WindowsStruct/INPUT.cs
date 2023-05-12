using System.Runtime.InteropServices;
using AutoKeyNet.WindowsHooks.WindowsEnums;

namespace AutoKeyNet.WindowsHooks.WindowsStruct;

/// <summary>
///     Contains information about a simulated input event.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Input
{
    /// <summary>
    ///     The type of the input event.
    /// </summary>
    public InputType Type;

    /// <summary>
    ///     The input data.
    /// </summary>
    public InputUnion Data;

    /// <summary>
    ///     The size of the Input structure, in bytes.
    /// </summary>
    public static int Size => Marshal.SizeOf(typeof(Input));
}