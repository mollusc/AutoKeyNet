using System.Runtime.InteropServices;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using static Windows.Win32.PInvoke;
using AutoKeyNet.Helper;
using AutoKeyNet.RuleRecords;

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
                    VIRTUAL_KEY.VK_LBUTTON.ToInput(),
                    VIRTUAL_KEY.VK_V.ToInput(0, Constants.KEY_SUPPRESS_NATIVE_BEHAVIOUR),
                ],
                [
                    VIRTUAL_KEY.VK_LBUTTON.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP),

                    VIRTUAL_KEY.VK_CONTROL.ToInput(),
                    VIRTUAL_KEY.VK_V.ToInput(),
                    VIRTUAL_KEY.VK_V.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP),
                    VIRTUAL_KEY.VK_CONTROL.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP)
                ]),
            new HotKeyRuleRecord(
                [
                    VIRTUAL_KEY.VK_LBUTTON.ToInput(),
                    VIRTUAL_KEY.VK_C.ToInput(0, Constants.KEY_SUPPRESS_NATIVE_BEHAVIOUR),
                ],
                [
                    VIRTUAL_KEY.VK_LBUTTON.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP),

                    VIRTUAL_KEY.VK_CONTROL.ToInput(),
                    VIRTUAL_KEY.VK_C.ToInput(),
                    VIRTUAL_KEY.VK_C.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP),
                    VIRTUAL_KEY.VK_CONTROL.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP)
                ]),
            new HotKeyRuleRecord(
                [
                    VIRTUAL_KEY.VK_LBUTTON.ToInput(),
                    VIRTUAL_KEY.VK_B.ToInput(0, Constants.KEY_SUPPRESS_NATIVE_BEHAVIOUR),
                ],
                PasteWithoutFormat()),
            new HotKeyRuleRecord(
                [
                    VIRTUAL_KEY.VK_LBUTTON.ToInput(),
                    VIRTUAL_KEY.VK_X.ToInput(0, Constants.KEY_SUPPRESS_NATIVE_BEHAVIOUR),
                ],
                [
                    VIRTUAL_KEY.VK_LBUTTON.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP),

                    VIRTUAL_KEY.VK_CONTROL.ToInput(),
                    VIRTUAL_KEY.VK_X.ToInput(),
                    VIRTUAL_KEY.VK_X.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP),
                    VIRTUAL_KEY.VK_CONTROL.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP)
                ]),
            new HotKeyRuleRecord(
                [
                    VIRTUAL_KEY.VK_LBUTTON.ToInput(),
                    VIRTUAL_KEY.VK_F.ToInput(0, Constants.KEY_SUPPRESS_NATIVE_BEHAVIOUR),
                ],
                [
                    VIRTUAL_KEY.VK_LBUTTON.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP),

                    VIRTUAL_KEY.VK_CONTROL.ToInput(),
                    VIRTUAL_KEY.VK_B.ToInput(),
                    VIRTUAL_KEY.VK_B.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP),
                    VIRTUAL_KEY.VK_CONTROL.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP)
                ]),
            new HotKeyRuleRecord(
                [
                    VIRTUAL_KEY.VK_LBUTTON.ToInput(),
                    VIRTUAL_KEY.VK_D.ToInput(0, Constants.KEY_SUPPRESS_NATIVE_BEHAVIOUR),
                ],
                [
                    VIRTUAL_KEY.VK_LBUTTON.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP),

                    VIRTUAL_KEY.VK_CONTROL.ToInput(),
                    VIRTUAL_KEY.VK_I.ToInput(),
                    VIRTUAL_KEY.VK_I.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP),
                    VIRTUAL_KEY.VK_CONTROL.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP)
                ]),
            new HotKeyRuleRecord(
                [
                    VIRTUAL_KEY.VK_LBUTTON.ToInput(),
                    VIRTUAL_KEY.VK_G.ToInput(0, Constants.KEY_SUPPRESS_NATIVE_BEHAVIOUR),
                ],
                [
                    VIRTUAL_KEY.VK_LBUTTON.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP),

                    VIRTUAL_KEY.VK_CONTROL.ToInput(),
                    VIRTUAL_KEY.VK_U.ToInput(),
                    VIRTUAL_KEY.VK_U.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP),
                    VIRTUAL_KEY.VK_CONTROL.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP)
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
            Span<INPUT> inputs = new Span<INPUT>(
                [
                    VIRTUAL_KEY.VK_LBUTTON.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP),

                    VIRTUAL_KEY.VK_CONTROL.ToInput((KEYBD_EVENT_FLAGS)0),
                    VIRTUAL_KEY.VK_V.ToInput(),
                    VIRTUAL_KEY.VK_V.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP),
                    VIRTUAL_KEY.VK_CONTROL.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP)
                ]
            );
            inputs.Send();
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