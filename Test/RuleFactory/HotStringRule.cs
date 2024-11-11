namespace AutoKeyNetApp.RuleFactory;

/// <summary>
///     Struct used for configuring hotstrings.
/// </summary>
public struct HotStringRule
{
    /// <summary>
    ///     The text that needs to be replaced
    /// </summary>
    public string KeyText { get; set; }

    /// <summary>
    ///     The text that replaces the key text.
    /// </summary>
    public string ReplaceText { get; set; }

    /// <summary>
    ///     Trigger the rule's action when the user presses one of the ending characters -
    ///     true, otherwise false
    /// </summary>
    public bool TriggerByEndingCharacter { get; set; }
}