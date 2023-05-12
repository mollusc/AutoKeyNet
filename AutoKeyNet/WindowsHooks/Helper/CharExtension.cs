using AutoKeyNet.WindowsHooks.WindowsEnums;
using AutoKeyNet.WindowsHooks.WindowsStruct;

namespace AutoKeyNet.WindowsHooks.Helper;

/// <summary>
/// Extension methods for the char type
/// </summary>
internal static class CharExtension
{
    /// <summary>
    /// Method for converting a character to an Input structure with a key down event
    /// </summary>
    /// <param name="letter">The character that will be converted into an Input structure</param>
    /// <param name="flags">Specifies various aspects of a keystroke</param>
    /// <param name="extraInfo">An additional value associated with the keystroke</param>
    /// <returns>An Input structure that represents the letter</returns>
    internal static Input ToInput(this char letter, KeyEventFlags flags, nuint extraInfo = Constants.KEY_IGNORE)
    {
        return new Input
        {
            type = InputType.INPUT_KEYBOARD,
            U = new InputUnion
            {
                ki = new KeyboardInput
                {
                    wVk = 0,
                    wScan = letter,
                    dwFlags = KeyEventFlags.UNICODE | flags,
                    dwExtraInfo = extraInfo,
                },
            }
        };
    }

    /// <summary>
    /// Method for converting a character to an Input structure with a key down and key up events
    /// </summary>
    /// <param name="letter">Character to be converted to an Input structure</param>
    /// <param name="flags">Specifies various aspects of a keystroke</param>
    /// <param name="extraInfo">An additional value associated with the keystroke</param>
    /// <returns>Input structures that represent the letter</returns>
    internal static IEnumerable<Input> ToInputsPressKey(this char letter, KeyEventFlags flags = 0, nuint extraInfo = Constants.KEY_IGNORE)
    {
        foreach (KeyEventFlags extraFlag in new[] { KeyEventFlags.KEYDOWN, KeyEventFlags.KEYUP })
            yield return letter.ToInput(flags | extraFlag, extraInfo);
    }
}