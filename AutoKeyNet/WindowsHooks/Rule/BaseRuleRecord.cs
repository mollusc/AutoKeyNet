using AutoKeyNet.WindowsHooks.Helper;
using AutoKeyNet.WindowsHooks.WindowsEnums;
using AutoKeyNet.WindowsHooks.WindowsStruct;
using static AutoKeyNet.WindowsHooks.WinApi.NativeMethods;

namespace AutoKeyNet.WindowsHooks.Rule;

/// <summary>
///     Base class for rules
/// </summary>
public class BaseRuleRecord
{
    /// <summary>
    ///     Delegate that checks the rule of the current window or control.
    /// </summary>
    /// <param name="windowTitle">Title of the foreground window</param>
    /// <param name="windowClass">Class of the foreground window</param>
    /// <param name="windowModule">Module name (file *.exe) of the foreground window</param>
    /// <param name="windowControl">Name of the focused control</param>
    /// <returns>Result of check</returns>
    public delegate bool WindowCondition(string? windowTitle, string? windowClass, string? windowModule,
        string? windowControl);

    /// <summary>
    ///     Check the rule of the current window or control.
    /// </summary>
    public WindowCondition? CheckWindowCondition;

    /// <summary>
    ///     Constructor of rule
    /// </summary>
    /// <param name="keyText">Text of the rule that triggers the rule's action (Run).</param>
    /// <param name="run">Action that is triggered when the rule is fired.</param>
    /// <param name="checkWindowCondition">Check the rule of the current window or control.</param>
    protected BaseRuleRecord(string keyText, Action run, WindowCondition? checkWindowCondition)
    {
        KeyText = keyText;
        KeyInputs = keyText.ToInputs().ToArray();
        Run = run;
        CheckWindowCondition = checkWindowCondition;
    }

    /// <summary>
    ///     Text of the rule that triggers the rule's action (Run).
    /// </summary>
    public string KeyText { get; }

    /// <summary>
    ///     Array of Inputs struct for the rule that triggers the rule's action (Run).
    /// </summary>
    internal Input[] KeyInputs { get; }

    /// <summary>
    ///     Action that is triggered when the rule is fired.
    /// </summary>
    public Action Run { get; }
}