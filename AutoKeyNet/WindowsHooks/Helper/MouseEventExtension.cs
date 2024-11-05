using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace AutoKeyNet.WindowsHooks.Helper;

public static class MouseEventExtension
{
    /// <summary>
    ///     Convert MOUSE_EVENT_FLAGS to Input
    /// </summary>
    /// <param name="mouseEvent">Mouse event</param>
    /// <param name="mouseData">Additional parameter for a mouse message used to detect the XBUTTON1 or XBUTTON2 keys.</param>
    /// <param name="extraInfo">An additional value associated with the keystroke</param>
    /// <returns>Input that represents mouse event</returns>
    public static INPUT ToInput(this MOUSE_EVENT_FLAGS mouseEvent, uint mouseData = 0,
        nuint extraInfo = Constants.KEY_IGNORE) =>
        new()
        {
            type = INPUT_TYPE.INPUT_MOUSE,
            Anonymous = new ()
            {
                mi = new ()
                {
                    dwFlags = mouseEvent,
                    mouseData = mouseData,
                    dwExtraInfo = extraInfo
                }
            }
        };
}