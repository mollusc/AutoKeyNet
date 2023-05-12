using AutoKeyNet.WindowsHooks.WindowsEnums;
using AutoKeyNet.WindowsHooks.WindowsStruct;

namespace AutoKeyNet.WindowsHooks.Helper;

internal static class CharExtension
{
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

    internal static IEnumerable<Input> ToInputsPressKey(this char letter, KeyEventFlags flags = 0, nuint extraInfo = Constants.KEY_IGNORE)
    {
        foreach (KeyEventFlags extraFlag in new[] { KeyEventFlags.KEYDOWN, KeyEventFlags.KEYUP })
            yield return letter.ToInput(flags | extraFlag, extraInfo);
    }
}