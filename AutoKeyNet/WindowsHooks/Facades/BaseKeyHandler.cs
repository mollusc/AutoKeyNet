using AutoKeyNet.WindowsHooks.Rule;

namespace AutoKeyNet.WindowsHooks.Facades;
/// <summary>
/// Base class for handling hotkeys
/// </summary>
internal abstract class BaseKeyHandler
{
    /// <summary>
    /// List of rules
    /// </summary>
    protected IEnumerable<BaseRuleRecord> Rules;

    /// <summary>
    /// Constructor of base class for handling hotkeys
    /// </summary>
    /// <param name="rules">List of rules for managing hotkeys</param>
    protected BaseKeyHandler(IEnumerable<BaseRuleRecord> rules)
    {
        Rules = rules;
    }
}
