using System.Runtime.InteropServices;
using LogKeyConsoleApp.Helper;
using LogKeyConsoleApp.WindowsEnums;

namespace LogKeyConsoleApp.WindowsStruct;

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
                return $"{Data.MouseInput.Flags} {Data.MouseInput.MouseData:x8}" ;
            case InputType.INPUT_KEYBOARD:
                VirtualKey vk = (VirtualKey)Data.KeyboardInput.VirtualKey;
                string? eventFlag = Data.KeyboardInput.Flags switch
                {
                    KeyEventFlags.KEYUP => "↑",
                    KeyEventFlags.KEYDOWN => "↓",
                    _ => null
                };
                return $"{vk.GetDisplayName() ?? vk.ToString()}{eventFlag}";
            case InputType.INPUT_HARDWARE:
                return "HardWare: " + Data.HardwareInput.Message;
            default:
                return "Undefined type";
        }
    }
}