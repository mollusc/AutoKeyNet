using AutoKeyNet.WindowsHooks.Helper;
using AutoKeyNet.WindowsHooks.WinApi;
using AutoKeyNet.WindowsHooks.WindowsEnums;
using AutoKeyNet.WindowsHooks.WindowsStruct;

namespace AutoKeyNet.WindowsHooks.Rule;

/// <summary>
///     Class responsible for defining hotstring rules (auto replace)
/// </summary>
public sealed class HotStringRuleRecord : BaseRuleRecord
{
    /// <summary>
    ///     Constructor of hotstring class
    /// </summary>
    /// <param name="keyText">Text of the rule that triggers the rule's action.</param>
    /// <param name="hotKeyRun">Action that is triggered when the rule is fired.</param>
    /// <param name="triggerByEndingCharacter">
    ///     Trigger the rule's action when the user presses one of the ending characters -
    ///     true, otherwise false
    /// </param>
    /// <param name="checkWindowCondition">Check the rule of the current window or control</param>
    public HotStringRuleRecord(string keyText, Action hotKeyRun, bool triggerByEndingCharacter = true,
        WindowCondition? checkWindowCondition = null)
        : base(keyText, hotKeyRun, checkWindowCondition)
    {
        TriggerByEndingCharacter = triggerByEndingCharacter;
    }

    /// <summary>
    ///     Constructor of hotstring class
    /// </summary>
    /// <param name="keyText">Text of the rule that triggers the rule's action.</param>
    /// <param name="replaceFunc">Func that replaces the key text of the rule with a result</param>
    /// <param name="triggerByEndingCharacter">
    ///     Trigger the rule's action when the user presses one of the ending characters -
    ///     true, otherwise false
    /// </param>
    /// <param name="checkWindowCondition">Check the rule of the current window or control</param>
    public HotStringRuleRecord(string keyText, Func<string> replaceFunc, bool triggerByEndingCharacter = true,
        WindowCondition? checkWindowCondition = null)
        : base(keyText, SendText(keyText, replaceFunc, triggerByEndingCharacter), checkWindowCondition)
    {
        TriggerByEndingCharacter = triggerByEndingCharacter;
    }

    /// <summary>
    ///     Constructor of hotstring class
    /// </summary>
    /// <param name="keyText">Text of the rule that triggers the rule's action</param>
    /// <param name="replaceText">Text that replaces the key text</param>
    /// <param name="triggerByEndingCharacter">
    ///     Trigger the rule's action when the user presses one of the ending characters -
    ///     true, otherwise false
    /// </param>
    /// <param name="checkWindowCondition">Check the rule of the current window or control</param>
    public HotStringRuleRecord(string keyText, string replaceText, bool triggerByEndingCharacter = true,
        WindowCondition? checkWindowCondition = null)
        : base(keyText, SendText(keyText, replaceText, triggerByEndingCharacter), checkWindowCondition)
    {
        TriggerByEndingCharacter = triggerByEndingCharacter;
    }

    /// <summary>
    ///     Trigger the rule's action when the user presses one of the ending characters.
    /// </summary>
    public bool TriggerByEndingCharacter { get; }

    /// <summary>
    ///     Create action that replaces text with another text.
    /// </summary>
    /// <param name="keyText">Text of the rule that triggers the rule's action</param>
    /// <param name="replaceText">Text that replaces the key text</param>
    /// <param name="triggerByEndingCharacter">
    ///     Trigger the rule's action when the user presses one of the ending characters -
    ///     true, otherwise false
    /// </param>
    /// <returns>Created action</returns>
    private static Action SendText(string keyText, string replaceText, bool triggerByEndingCharacter)
    {
        var inputs = GetInputsOfReplaceText(keyText, replaceText, triggerByEndingCharacter);
        return () => { NativeMethods.SendInput(inputs); };
    }

    /// <summary>
    ///     Create an action that replaces text with the result of a Func.
    /// </summary>
    /// <param name="keyText">Text of the rule that triggers the rule's action</param>
    /// <param name="replaceFunc">Func that replaces the key text of the rule with a result</param>
    /// <param name="triggerByEndingCharacter">
    ///     Trigger the rule's action when the user presses one of the ending characters -
    ///     true, otherwise false
    /// </param>
    /// <returns>Created action</returns>
    private static Action SendText(string keyText, Func<string> replaceFunc, bool triggerByEndingCharacter)
    {
        return () =>
        {
            var inputs = GetInputsOfReplaceText(keyText, replaceFunc.Invoke(), triggerByEndingCharacter);
            NativeMethods.SendInput(inputs);
        };
    }

    /// <summary>
    ///     Create an array of Inputs for replacing text with another text.
    /// </summary>
    /// <param name="keyText">Text of the rule that triggers the rule's action</param>
    /// <param name="replaceText">Text that replaces the key text</param>
    /// <param name="triggerByEndingCharacter">
    ///     Trigger the rule's action when the user presses one of the ending characters -
    ///     true, otherwise false
    /// </param>
    /// <returns>Array of Inputs</returns>
    private static Input[] GetInputsOfReplaceText(string keyText, string replaceText, bool triggerByEndingCharacter)
    {
        List<Input> inputList = new();
        if (triggerByEndingCharacter)
            inputList.AddRange(VirtualKey.LEFT.ToInputsPressKey());
        inputList.AddRange(SendBackspaces(keyText.Length));
        inputList.AddRange(replaceText.ToInputs());
        if (triggerByEndingCharacter)
            inputList.AddRange(VirtualKey.RIGHT.ToInputsPressKey());
        var inputs = inputList.ToArray();
        return inputs;
    }

    /// <summary>
    ///     Create an array of Inputs for sending the Backspace key a number of times.
    /// </summary>
    /// <param name="count">The number of times the Backspace key should be pressed.</param>
    private static Input[] SendBackspaces(int count) =>
        Enumerable.Repeat(VirtualKey.BACK.ToInputsPressKey(), count).SelectMany(x => x).ToArray();
}