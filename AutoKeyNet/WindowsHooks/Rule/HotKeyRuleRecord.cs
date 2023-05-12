namespace AutoKeyNet.WindowsHooks.Rule;

/// <summary>
/// Class responsible for defining hotkey rules. 
/// </summary>
public class HotKeyRuleRecord : BaseRuleRecord
{
    public HotKeyRuleRecord(string keyWord, Action hotKeyRun, WindowCondition? checkWindowCondition = null, bool releaseKeysBeforeRun = false)
        : base(keyWord, hotKeyRun, checkWindowCondition, releaseKeysBeforeRun)
    {
    }
    public HotKeyRuleRecord(string keyWord, Func<string> hotKeyFunc, WindowCondition? checkWindowCondition = null)
        : base(keyWord, SendText(hotKeyFunc, keyWord), checkWindowCondition)
    {
    }
    public HotKeyRuleRecord(string keyWord, string hotKeyString, WindowCondition? checkWindowCondition = null)
        : base(keyWord, SendText(() => hotKeyString, keyWord), checkWindowCondition)
    {
    }
}
