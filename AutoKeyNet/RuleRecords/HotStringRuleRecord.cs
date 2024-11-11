using System.Runtime.InteropServices;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using AutoKeyNet.Helper;
using static Windows.Win32.PInvoke;

namespace AutoKeyNet.RuleRecords;

/// <summary>
///     Class responsible for defining hotstring rules (auto replace)
/// </summary>
public sealed class HotStringRuleRecord : BaseRuleRecord
{
    /// <summary>
    ///     Constructor of hotstring class
    /// </summary>
    /// <param name="keyChars">Text of the rule that triggers the rule's action</param>
    /// <param name="replaceText">Text that replaces the key text</param>
    /// <param name="triggerByEndingCharacter">
    ///     Trigger the rule's action when the user presses one of the ending characters -
    ///     true, otherwise false
    /// </param>
    /// <param name="checkWindowCondition">Check the rule of the current window or control</param>
    public HotStringRuleRecord(string keyChars, string replaceText, bool triggerByEndingCharacter = true,
        WindowCondition? checkWindowCondition = null)
        : this(keyChars, SendText(keyChars, replaceText, triggerByEndingCharacter), triggerByEndingCharacter,
            checkWindowCondition)
    {
    }

    public HotStringRuleRecord(string keyString, Action run, bool triggerByEndingCharacter = true,
        WindowCondition? checkWindowCondition = null) : base(keyString.ToCharArray(), run, checkWindowCondition)
    {
        TriggerByEndingCharacter = triggerByEndingCharacter;
    }

    /// <summary>
    ///     Constructor of hotstring class
    /// </summary>
    /// <param name="keyString">Text of the rule that triggers the rule's action.</param>
    /// <param name="replaceFunc">Func that replaces the key text of the rule with a result</param>
    /// <param name="triggerByEndingCharacter">
    ///     Trigger the rule's action when the user presses one of the ending characters -
    ///     true, otherwise false
    /// </param>
    /// <param name="checkWindowCondition">Check the rule of the current window or control</param>
    public HotStringRuleRecord(string keyString, Func<string> replaceFunc, bool triggerByEndingCharacter = true, WindowCondition? checkWindowCondition = null)
        : this(keyString, SendText(keyString, replaceFunc, triggerByEndingCharacter), triggerByEndingCharacter, checkWindowCondition)
    {
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
        var inputs = GetInputsOfReplaceText(keyText, replaceText, triggerByEndingCharacter).ToArray();
        return () => SendInput(inputs, Marshal.SizeOf(typeof(INPUT)));
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
            var inputs = GetInputsOfReplaceText(keyText, replaceFunc.Invoke(), triggerByEndingCharacter).ToArray();
            SendInput(inputs, Marshal.SizeOf(typeof(INPUT)));
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
    private static List<INPUT> GetInputsOfReplaceText(string keyText, string replaceText, bool triggerByEndingCharacter)
    {
        List<INPUT> inputList = new();
        if (triggerByEndingCharacter)
            inputList.AddRange(VIRTUAL_KEY.VK_LEFT.ToInputsPressKey());
        inputList.AddRange(GetInputsOfBackspaces(keyText.Length));
        inputList.AddRange(replaceText.ToUnicodeInputs());
        if (triggerByEndingCharacter)
            inputList.AddRange(VIRTUAL_KEY.VK_RIGHT.ToInputsPressKey());
        return inputList;
    }

    /// <summary>
    ///     Create an array of Inputs for sending the Backspace key a number of times.
    /// </summary>
    /// <param name="count">The number of times the Backspace key should be pressed.</param>
    private static INPUT[] GetInputsOfBackspaces(int count) =>
        Enumerable.Repeat(VIRTUAL_KEY.VK_BACK.ToInputsPressKey(), count).SelectMany(x => x).ToArray();
}