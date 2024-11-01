using Windows.Win32.UI.Input.KeyboardAndMouse;
using static AutoKeyNet.WindowsHooks.Helper.Constants;

namespace AutoKeyNet.WindowsHooks.Helper;

internal static class InputExtension
{
    /// <summary>
    ///     Converts keyboard inputs and mouse inputs to virtual keys.
    /// </summary>
    /// <param name="inputs">Inputs to convert</param>
    /// <returns>Virtual keys</returns>
    public static IEnumerable<VIRTUAL_KEY> ToVirtualKeys(this IEnumerable<INPUT> inputs) =>
        inputs.Select(i => i.ToVirtualKey());

    /// <summary>
    ///     Converts keyboard input and mouse input to a virtual key.
    /// </summary>
    /// <param name="input">Input to convert</param>
    /// <returns>A virtual key</returns>
    public static VIRTUAL_KEY ToVirtualKey(this INPUT input) =>
        input.type == INPUT_TYPE.INPUT_KEYBOARD ? input.Anonymous.ki.wVk : input.Anonymous.mi.ToVirtualKey();

    /// <summary>
    ///     Converts mouse input to a virtual key.
    /// </summary>
    /// <param name="uMi">Mouse input to convert</param>
    /// <returns>A virtual key</returns>
    public static VIRTUAL_KEY ToVirtualKey(this MOUSEINPUT uMi) =>
        uMi.dwFlags switch
        {
            MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTDOWN => VIRTUAL_KEY.VK_LBUTTON,
            MOUSE_EVENT_FLAGS.MOUSEEVENTF_RIGHTDOWN => VIRTUAL_KEY.VK_RBUTTON,
            MOUSE_EVENT_FLAGS.MOUSEEVENTF_MIDDLEDOWN => VIRTUAL_KEY.VK_MBUTTON,
            MOUSE_EVENT_FLAGS.MOUSEEVENTF_XDOWN when (uMi.mouseData & XBUTTON1) == XBUTTON1 => VIRTUAL_KEY.VK_XBUTTON1,
            MOUSE_EVENT_FLAGS.MOUSEEVENTF_XDOWN when (uMi.mouseData & XBUTTON2) == XBUTTON2 => VIRTUAL_KEY.VK_XBUTTON1,
            _ => 0
        };
}