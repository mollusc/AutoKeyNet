using AutoKeyNet.WindowsHooks.Helper;
using AutoKeyNet.WindowsHooks.WinApi;
using AutoKeyNet.WindowsHooks.WindowsEnums;
using AutoKeyNet.WindowsHooks.WindowsStruct;

namespace AutoKeyNet.WindowsHooks.Rule;

/// <summary>
///     Class responsible for defining hotkey rules.
/// </summary>
public class HotKeyRuleRecord : BaseRuleRecord
{
    public HotKeyRuleRecord(Input[] hotKeys, IEnumerable<Input> sendKeys, WindowCondition? checkWindowCondition = null)
        : base(hotKeys, () => NativeMethods.SendInput(sendKeys.ToArray()), checkWindowCondition)
    {
    }
    public HotKeyRuleRecord(Input[] hotKeys, Action hotKeyRun, WindowCondition? checkWindowCondition = null)
        : base(hotKeys, hotKeyRun, checkWindowCondition)
    {
    }

    /// <summary>
    ///     Constructor of hotkey rule.
    /// </summary>
    /// <param name="keyText">Text of the rule that triggers the rule's action.</param>
    /// <param name="hotKeyRun">Action that is triggered when the rule is fired.</param>
    /// <param name="checkWindowCondition">Check the rule of the current window or control</param>
    public HotKeyRuleRecord(string keyText, Action hotKeyRun, WindowCondition? checkWindowCondition = null)
        : base(keyText, hotKeyRun, checkWindowCondition)
    {
    }

    /// <summary>
    ///     Constructor of hotkey rule.
    /// </summary>
    /// <param name="keyText">Text of the rule that triggers the rule's action.</param>
    /// <param name="hotKeyFunc">Func that replaces the key text of the rule with a result</param>
    /// <param name="checkWindowCondition">Check the rule of the current window or control</param>
    public HotKeyRuleRecord(string keyText, Func<string> hotKeyFunc, WindowCondition? checkWindowCondition = null)
        : base(keyText, SendText(hotKeyFunc, keyText), checkWindowCondition)
    {
    }

    /// <summary>
    ///     Constructor of hotkey rule
    /// </summary>
    /// <param name="keyText">Text of the rule that triggers the rule's action</param>
    /// <param name="hotKeyString">Text that replaces the key text</param>
    /// <param name="checkWindowCondition">Check the rule of the current window or control</param>
    public HotKeyRuleRecord(string keyText, string hotKeyString, WindowCondition? checkWindowCondition = null)
        : base(keyText, SendText(() => hotKeyString, keyText), checkWindowCondition)
    {
    }


    /// <summary>
    ///     Create an action that replaces text with the result of a Func. Before replace keys need to be released.
    /// </summary>
    /// <param name="replaceFunc">Func that replaces the key text of the rule with a result.</param>
    /// <param name="releaseKeys">Keys that need to be released</param>
    /// <returns>Created action</returns>
    protected static Action SendText(Func<string> replaceFunc, string releaseKeys) =>
        () =>
        {
            var listInputs = new List<Input>(ReleaseKeys(releaseKeys));
            listInputs.AddRange(replaceFunc.Invoke().ToInputs().ToArray());
            var inputs = listInputs.ToArray();
            NativeMethods.SendInput(inputs);
        };

    /// <summary>
    ///     Create an Input for releasing keys.
    /// </summary>
    /// <param name="keys">Keys that need to be released</param>
    /// <returns>Input of released keys</returns>
    internal static IEnumerable<Input> ReleaseKeys(string keys)
    {
        var inputs = keys.ToInputs().ToArray();
        for (var i = 0; i < inputs.Length; i++)
            if (inputs[i].Type == InputType.INPUT_KEYBOARD)
                inputs[i].Data.KeyboardInput.Flags = KeyEventFlags.KEYUP;
            else if (inputs[i].Type == InputType.INPUT_MOUSE)
                inputs[i].Data.MouseInput.Flags = (MouseEvents)((int)inputs[i].Data.MouseInput.Flags << 1);

        return inputs;
    }
}