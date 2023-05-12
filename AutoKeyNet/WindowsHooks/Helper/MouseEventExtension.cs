using AutoKeyNet.WindowsHooks.WinApi;
using AutoKeyNet.WindowsHooks.WindowsEnums;
using AutoKeyNet.WindowsHooks.WindowsStruct;

namespace AutoKeyNet.WindowsHooks.Helper;

internal static class MouseEventExtension
{
    /// <summary>
    ///     Convert MouseEvents to Input
    /// </summary>
    /// <param name="mouseEvent">Mouse event</param>
    /// <param name="mouseData">Additional parameter for a mouse message used to detect the XBUTTON1 or XBUTTON2 keys.</param>
    /// <param name="extraInfo">An additional value associated with the keystroke</param>
    /// <returns>Input that represents mouse event</returns>
    public static Input ToInput(this MouseEvents mouseEvent, uint mouseData = 0,
        nuint extraInfo = NativeMethods.KEY_IGNORE) =>
        new()
        {
            Type = InputType.INPUT_MOUSE,
            Data = new InputUnion
            {
                MouseInput = new MouseInput
                {
                    Flags = mouseEvent,
                    MouseData = (int)mouseData,
                    ExtraInfo = extraInfo
                }
            }
        };
}