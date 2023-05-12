using AutoKeyNet.WindowsHooks.WinApi;
using AutoKeyNet.WindowsHooks.WindowsEnums;
using AutoKeyNet.WindowsHooks.WindowsStruct;

namespace AutoKeyNet.WindowsHooks.Helper;

internal static class InputExtension
{
    /// <summary>
    ///     Converts keyboard inputs and mouse inputs to virtual keys.
    /// </summary>
    /// <param name="inputs">Inputs to convert</param>
    /// <returns>Virtual keys</returns>
    public static IEnumerable<VirtualKey> ToVirtualKeys(this IEnumerable<Input> inputs) =>
        inputs.Select(i => i.ToVirtualKey());

    /// <summary>
    ///     Converts keyboard input and mouse input to a virtual key.
    /// </summary>
    /// <param name="input">Input to convert</param>
    /// <returns>A virtual key</returns>
    public static VirtualKey ToVirtualKey(this Input input) =>
        input.Data.KeyboardInput.VirtualKey != 0 ? (VirtualKey)input.Data.KeyboardInput.VirtualKey : input.Data.MouseInput.ToVirtualKey();

    /// <summary>
    ///     Converts mouse input to a virtual key.
    /// </summary>
    /// <param name="uMi">Mouse input to convert</param>
    /// <returns>A virtual key</returns>
    public static VirtualKey ToVirtualKey(this MouseInput uMi) =>
        uMi.Flags switch
        {
            MouseEvents.LEFTDOWN => VirtualKey.LBUTTON,
            MouseEvents.RIGHTDOWN => VirtualKey.RBUTTON,
            MouseEvents.MIDDLEDOWN => VirtualKey.MBUTTON,
            MouseEvents.XDOWN when uMi.MouseData == NativeMethods.XBUTTON1 => VirtualKey.XBUTTON1,
            MouseEvents.XDOWN when uMi.MouseData == NativeMethods.XBUTTON2 => VirtualKey.XBUTTON2,
            _ => 0
        };
}