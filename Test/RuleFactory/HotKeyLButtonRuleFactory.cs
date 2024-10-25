using AutoKeyNet.WindowsHooks.Rule;
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
            new HotKeyRuleRecord("{LBUTTON DOWN}{KEY_C DOWN}{KEY_C UP}{LBUTTON UP}",
                "{CONTROL DOWN}{KEY_C DOWN}{KEY_C UP}{CONTROL UP}",
                option:HotKeyRuleRecordOptionFlags.SuppressNativeBehavior|HotKeyRuleRecordOptionFlags.SuppressNativeBehaviorForPrefixKey), // Copy
            new HotKeyRuleRecord("{LBUTTON DOWN}{KEY_V DOWN}",
                "{CONTROL DOWN}{KEY_V DOWN}{KEY_V UP}{CONTROL UP}", option:HotKeyRuleRecordOptionFlags.SuppressNativeBehavior), // Insert
            new HotKeyRuleRecord("{LBUTTON DOWN}{KEY_B DOWN}", PasteWithoutFormat()), // Insert text without formatting
            new HotKeyRuleRecord("{LBUTTON DOWN}{KEY_X DOWN}",
                "{CONTROL DOWN}{KEY_X DOWN}{KEY_X UP}{CONTROL UP}", option : HotKeyRuleRecordOptionFlags.SuppressNativeBehavior), // Cut
            new HotKeyRuleRecord("{LBUTTON DOWN}{KEY_F DOWN}",
                "{CONTROL DOWN}{KEY_B DOWN}{KEY_B UP}{CONTROL UP}", option : HotKeyRuleRecordOptionFlags.SuppressNativeBehavior), // Set text as bold
            new HotKeyRuleRecord("{LBUTTON DOWN}{KEY_D DOWN}",
                "{CONTROL DOWN}{KEY_I DOWN}{KEY_I UP}{CONTROL UP}", option : HotKeyRuleRecordOptionFlags.SuppressNativeBehavior), // Set text as italic
            new HotKeyRuleRecord("{LBUTTON DOWN}{KEY_G DOWN}",
                "{CONTROL DOWN}{KEY_U DOWN}{KEY_U UP}{CONTROL UP}", option : HotKeyRuleRecordOptionFlags.SuppressNativeBehavior) // Set text as underscore
        };
        return rules;
    }



    /// <summary>
    /// Paste text from clapboard without format
    /// </summary>
    /// <returns></returns>
    private Func<string> PasteWithoutFormat()
    {
        return () =>
        {
            Clipboard.SetText(GetTextFromClipboardWithoutFormat());
            return "{CONTROL DOWN}{KEY_V DOWN}{KEY_V UP}{CONTROL UP}";
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