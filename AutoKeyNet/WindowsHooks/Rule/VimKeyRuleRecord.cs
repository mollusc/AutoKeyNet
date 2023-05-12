using AutoKeyNet.WindowsHooks.Helper;
using AutoKeyNet.WindowsHooks.WinApi;

namespace AutoKeyNet.WindowsHooks.Rule;

/// <summary>
///     Class responsible for defining rules similar to Vim commands.
/// </summary>
public class VimKeyRuleRecord : BaseRuleRecord
{
    /// <summary>
    ///     Constructor of rule
    /// </summary>
    /// <param name="keyText">Text of the rule that triggers the rule's action (Run)</param>
    /// <param name="hotKeyRun">Action that is triggered when the rule is fired.</param>
    /// <param name="checkWindowCondition">Check the rule of the current window or control.</param>
    public VimKeyRuleRecord(string keyText, Action hotKeyRun, WindowCondition? checkWindowCondition = null)
        : base(keyText, hotKeyRun, checkWindowCondition)
    {
    }

    /// <summary>
    ///     Constructor of rule
    /// </summary>
    /// <param name="keyText">Text of the rule that triggers the rule's action (Run)</param>
    /// <param name="replaceText">Text that replaces the key text</param>
    /// <param name="checkWindowCondition">Check the rule of the current window or control.</param>
    public VimKeyRuleRecord(string keyText, string replaceText, WindowCondition? checkWindowCondition = null)
        : base(keyText, SendText(replaceText), checkWindowCondition)
    {
    }

    /// <summary>
    ///     Constructor of rule
    /// </summary>
    /// <param name="keyText">Text of the rule that triggers the rule's action (Run)</param>
    /// <param name="replaceFunc">Func that replaces the key text of the rule with a result</param>
    /// <param name="checkWindowCondition">Check the rule of the current window or control.</param>
    public VimKeyRuleRecord(string keyText, Func<string> replaceFunc, WindowCondition? checkWindowCondition = null)
        : base(keyText, SendText(replaceFunc), checkWindowCondition)
    {
    }

    /// <summary>
    ///     Create action that replaces text with another text.
    /// </summary>
    /// <param name="replaceWord">Text that replaces the key text of the rule.</param>
    /// <returns>Created action</returns>
    protected static Action SendText(string replaceWord)
    {
        var inputs = replaceWord.ToInputs().ToArray();
        return () => { NativeMethods.SendInput(inputs); };
    }

    /// <summary>
    ///     Create an action that replaces text with the result of a Func.
    /// </summary>
    /// <param name="replaceFunc">Func that replaces the key text of the rule with a result.</param>
    /// <returns>Created action</returns>
    protected static Action SendText(Func<string> replaceFunc) =>
        () =>
        {
            var inputs = replaceFunc.Invoke().ToInputs().ToArray();
            NativeMethods.SendInput(inputs);
        };
}