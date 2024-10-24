using System.Runtime.InteropServices;
using AutoKeyNet.WindowsHooks.Helper;
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

    public override string ToString()
    {
        switch (Type)
        {
            case InputType.INPUT_MOUSE:
                return $"Mouse: {Data.MouseInput.ToVirtualKey()} {Data.MouseInput.Flags}";
            case InputType.INPUT_KEYBOARD:
                return $"KeyBoard: {(VirtualKey)Data.KeyboardInput.VirtualKey} {Data.KeyboardInput.Flags}";
            case InputType.INPUT_HARDWARE:
                return "HardWare: " + Data.HardwareInput.Message;
            default:
                return "Undefined type";
        }
    }
}