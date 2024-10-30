using AutoKeyNet.WindowsHooks.Helper;
using AutoKeyNet.WindowsHooks.Rule;
using AutoKeyNet.WindowsHooks.WinApi;
using AutoKeyNet.WindowsHooks.WindowsEnums;

namespace AutoKeyNetApp.RuleFactory;

/// <summary>
///     Create rules with left key of mouse
/// </summary>
internal class HotKeyLButtonRuleFactory : BaseRuleFactory
{
    public override List<BaseRuleRecord> Create()
    {
        var rules = new List<BaseRuleRecord>
        {
            new HotKeyRuleRecord(
                [
                    VirtualKey.LBUTTON.ToInput(KeyEventFlags.KEYDOWN),
                    VirtualKey.KEY_V.ToInput(KeyEventFlags.KEYDOWN, NativeMethods.KEY_SUPRESS_NATIVE_BEHAVIOUR),
                ],
                [
                    VirtualKey.LBUTTON.ToInput(KeyEventFlags.KEYUP),

                    VirtualKey.CONTROL.ToInput(KeyEventFlags.KEYDOWN),
                    VirtualKey.KEY_V.ToInput(KeyEventFlags.KEYDOWN),
                    VirtualKey.KEY_V.ToInput(KeyEventFlags.KEYUP),
                    VirtualKey.CONTROL.ToInput(KeyEventFlags.KEYUP)
                ]),
            new HotKeyRuleRecord(
                [
                    VirtualKey.LBUTTON.ToInput(KeyEventFlags.KEYDOWN),
                    VirtualKey.KEY_C.ToInput(KeyEventFlags.KEYDOWN, NativeMethods.KEY_SUPRESS_NATIVE_BEHAVIOUR),
                ],
                [
                    VirtualKey.LBUTTON.ToInput(KeyEventFlags.KEYUP),

                    VirtualKey.CONTROL.ToInput(KeyEventFlags.KEYDOWN),
                    VirtualKey.KEY_C.ToInput(KeyEventFlags.KEYDOWN),
                    VirtualKey.KEY_C.ToInput(KeyEventFlags.KEYUP),
                    VirtualKey.CONTROL.ToInput(KeyEventFlags.KEYUP)
                ]),
            new HotKeyRuleRecord(
                [
                    VirtualKey.LBUTTON.ToInput(KeyEventFlags.KEYDOWN),
                    VirtualKey.KEY_B.ToInput(KeyEventFlags.KEYDOWN, NativeMethods.KEY_SUPRESS_NATIVE_BEHAVIOUR),
                ],
                PasteWithoutFormat()),
            new HotKeyRuleRecord(
                [
                    VirtualKey.LBUTTON.ToInput(KeyEventFlags.KEYDOWN),
                    VirtualKey.KEY_X.ToInput(KeyEventFlags.KEYDOWN, NativeMethods.KEY_SUPRESS_NATIVE_BEHAVIOUR),
                ],
                [
                    VirtualKey.LBUTTON.ToInput(KeyEventFlags.KEYUP),

                    VirtualKey.CONTROL.ToInput(KeyEventFlags.KEYDOWN),
                    VirtualKey.KEY_X.ToInput(KeyEventFlags.KEYDOWN),
                    VirtualKey.KEY_X.ToInput(KeyEventFlags.KEYUP),
                    VirtualKey.CONTROL.ToInput(KeyEventFlags.KEYUP)
                ]),
            new HotKeyRuleRecord(
                [
                    VirtualKey.LBUTTON.ToInput(KeyEventFlags.KEYDOWN),
                    VirtualKey.KEY_F.ToInput(KeyEventFlags.KEYDOWN, NativeMethods.KEY_SUPRESS_NATIVE_BEHAVIOUR),
                ],
                [
                    VirtualKey.LBUTTON.ToInput(KeyEventFlags.KEYUP),

                    VirtualKey.CONTROL.ToInput(KeyEventFlags.KEYDOWN),
                    VirtualKey.KEY_B.ToInput(KeyEventFlags.KEYDOWN),
                    VirtualKey.KEY_B.ToInput(KeyEventFlags.KEYUP),
                    VirtualKey.CONTROL.ToInput(KeyEventFlags.KEYUP)
                ]),
            new HotKeyRuleRecord(
                [
                    VirtualKey.LBUTTON.ToInput(KeyEventFlags.KEYDOWN),
                    VirtualKey.KEY_D.ToInput(KeyEventFlags.KEYDOWN, NativeMethods.KEY_SUPRESS_NATIVE_BEHAVIOUR),
                ],
                [
                    VirtualKey.LBUTTON.ToInput(KeyEventFlags.KEYUP),

                    VirtualKey.CONTROL.ToInput(KeyEventFlags.KEYDOWN),
                    VirtualKey.KEY_I.ToInput(KeyEventFlags.KEYDOWN),
                    VirtualKey.KEY_I.ToInput(KeyEventFlags.KEYUP),
                    VirtualKey.CONTROL.ToInput(KeyEventFlags.KEYUP)
                ]),
            new HotKeyRuleRecord(
                [
                    VirtualKey.LBUTTON.ToInput(KeyEventFlags.KEYDOWN),
                    VirtualKey.KEY_G.ToInput(KeyEventFlags.KEYDOWN, NativeMethods.KEY_SUPRESS_NATIVE_BEHAVIOUR),
                ],
                [
                    VirtualKey.LBUTTON.ToInput(KeyEventFlags.KEYUP),

                    VirtualKey.CONTROL.ToInput(KeyEventFlags.KEYDOWN),
                    VirtualKey.KEY_U.ToInput(KeyEventFlags.KEYDOWN),
                    VirtualKey.KEY_U.ToInput(KeyEventFlags.KEYUP),
                    VirtualKey.CONTROL.ToInput(KeyEventFlags.KEYUP)
                ]),
        };
        return rules;
    }



    /// <summary>
    /// Paste text from clapboard without format
    /// </summary>
    /// <returns></returns>
    private Action PasteWithoutFormat()
    {
        return () =>
        {
            Clipboard.SetText(GetTextFromClipboardWithoutFormat());
            NativeMethods.SendInputAsync(
            [
                VirtualKey.LBUTTON.ToInput(KeyEventFlags.KEYUP),

                VirtualKey.CONTROL.ToInput(KeyEventFlags.KEYDOWN),
                VirtualKey.KEY_V.ToInput(KeyEventFlags.KEYDOWN),
                VirtualKey.KEY_V.ToInput(KeyEventFlags.KEYUP),
                VirtualKey.CONTROL.ToInput(KeyEventFlags.KEYUP)
            ]).ConfigureAwait(false);
        };
    }


    /// <summary>
    ///     Retrieve plain text from the clipboard
    /// </summary>
    /// <returns>Plain text from clipboard</returns>
    private static string GetTextFromClipboardWithoutFormat()
    {
        return Clipboard.GetText(TextDataFormat.UnicodeText);
    }
}