using AutoKeyNet.WindowsHooks.Rule;

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
            new HotKeyRuleRecord("{LBUTTON DOWN}{KEY_C DOWN}",
                "{CONTROL DOWN}{KEY_C DOWN}{KEY_C UP}{CONTROL UP}"), // Copy
            new HotKeyRuleRecord("{LBUTTON DOWN}{KEY_V DOWN}",
                "{CONTROL DOWN}{KEY_V DOWN}{KEY_V UP}{CONTROL UP}"), // Insert
            new HotKeyRuleRecord("{LBUTTON DOWN}{KEY_B DOWN}",
                GetTextFromClipboardWithoutFormat), // Insert text without formatting
            new HotKeyRuleRecord("{LBUTTON DOWN}{KEY_X DOWN}",
                "{CONTROL DOWN}{KEY_X DOWN}{KEY_X UP}{CONTROL UP}"), // Cut
            new HotKeyRuleRecord("{LBUTTON DOWN}{KEY_F DOWN}",
                "{CONTROL DOWN}{KEY_B DOWN}{KEY_B UP}{CONTROL UP}"), // Set text as bold
            new HotKeyRuleRecord("{LBUTTON DOWN}{KEY_D DOWN}",
                "{CONTROL DOWN}{KEY_I DOWN}{KEY_I UP}{CONTROL UP}"), // Set text as italic
            new HotKeyRuleRecord("{LBUTTON DOWN}{KEY_G DOWN}",
                "{CONTROL DOWN}{KEY_U DOWN}{KEY_U UP}{CONTROL UP}") // Set text as underscore
        };
        return rules;
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