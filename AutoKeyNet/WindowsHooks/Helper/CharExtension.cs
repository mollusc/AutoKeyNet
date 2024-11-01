using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace AutoKeyNet.WindowsHooks.Helper;

/// <summary>
///     Extension methods for the char type
/// </summary>
internal static class CharExtension
{
    /// <summary>
    ///     Method for converting a character to an Input structure with a key down event
    /// </summary>
    /// <param name="letter">The character that will be converted into an Input structure</param>
    /// <param name="flags">Specifies various aspects of a keystroke</param>
    /// <param name="extraInfo">An additional value associated with the keystroke</param>
    /// <returns>An Input structure that represents the letter</returns>
    internal static INPUT ToInput(this char letter, KEYBD_EVENT_FLAGS flags, nuint extraInfo = Constants.KEY_IGNORE)
    {
        return new INPUT
        {
            type = INPUT_TYPE.INPUT_KEYBOARD,
            Anonymous = new ()
            {
                ki = new ()
                {
                    wVk = 0,
                    wScan = letter,
                    dwFlags = KEYBD_EVENT_FLAGS.KEYEVENTF_UNICODE | flags,
                    dwExtraInfo = extraInfo
                }
            }
        };
    }

    /// <summary>
    ///     Method for converting a character to an Input structure with a key down and key up events
    /// </summary>
    /// <param name="letter">Character to be converted to an Input structure</param>
    /// <param name="flags">Specifies various aspects of a keystroke</param>
    /// <param name="extraInfo">An additional value associated with the keystroke</param>
    /// <returns>Input structures that represent the letter</returns>
    internal static IEnumerable<INPUT> ToInputsPressKey(this char letter, KEYBD_EVENT_FLAGS flags = 0,
        nuint extraInfo = Constants.KEY_IGNORE)
    {
        foreach (var extraFlag in new[] { (KEYBD_EVENT_FLAGS)0, KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP })
            yield return letter.ToInput(flags | extraFlag, extraInfo);
    }
}