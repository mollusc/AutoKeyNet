namespace AutoKeyNet.WindowsHooks.Rule;

/// <summary>
/// Class responsible for defining hotkey rules. 
/// </summary>
public class HotKeyRuleRecord : BaseRuleRecord
{
    public HotKeyRuleRecordOptionFlags Options { get; }


    public HotKeyRuleRecord(string keyWord, Action hotKeyRun, WindowCondition? checkWindowCondition = null, HotKeyRuleRecordOptionFlags option = HotKeyRuleRecordOptionFlags.None)
        : base(keyWord, hotKeyRun, checkWindowCondition)
    {
        Options = option;
    }
    public HotKeyRuleRecord(string keyWord, Func<string> hotKeyFunc, WindowCondition? checkWindowCondition = null, HotKeyRuleRecordOptionFlags option = HotKeyRuleRecordOptionFlags.None)
        : base(keyWord, SendText(hotKeyFunc, keyWord), checkWindowCondition)
    {
        Options = option;
    }
    public HotKeyRuleRecord(string keyWord, string hotKeyString, WindowCondition? checkWindowCondition = null, HotKeyRuleRecordOptionFlags option = HotKeyRuleRecordOptionFlags.None)
        : base(keyWord, SendText(() => hotKeyString, keyWord), checkWindowCondition)
    {
        Options = option;
    }
}
