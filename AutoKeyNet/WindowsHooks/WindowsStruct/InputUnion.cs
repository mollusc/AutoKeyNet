using System.Runtime.InteropServices;

namespace AutoKeyNet.WindowsHooks.WindowsStruct;

/// <summary>
///     Union of different input types: mouse, keyboard, and hardware.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public struct InputUnion
{
    /// <summary>
    ///     The mouse input.
    /// </summary>
    [FieldOffset(0)] public MouseInput MouseInput;

    /// <summary>
    ///     The keyboard input.
    /// </summary>
    [FieldOffset(0)] public KeyboardInput KeyboardInput;

    /// <summary>
    ///     The hardware input.
    /// </summary>
    [FieldOffset(0)] public HardwareInput HardwareInput;
}