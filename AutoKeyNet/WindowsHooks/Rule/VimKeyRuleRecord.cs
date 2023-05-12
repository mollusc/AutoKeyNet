namespace AutoKeyNet.WindowsHooks.Rule;

/// <summary>
/// Класс определяющий условия по типу команд vim
/// </summary>
public class VimKeyRuleRecord : BaseRuleRecord
{
    public VimKeyRuleRecord(string keyWord, Action hotKeyRun, WindowCondition? checkWindowCondition = null) 
        : base(keyWord, hotKeyRun, checkWindowCondition)
    {
    }

    public VimKeyRuleRecord(string keyWord, string hotKeyString, WindowCondition? checkWindowCondition = null) 
        : base(keyWord, SendText(hotKeyString), checkWindowCondition)
    {
    }

    public VimKeyRuleRecord(string keyWord, Func<string> hotKeyFunc, WindowCondition? checkWindowCondition = null)
        : base(keyWord, SendText(hotKeyFunc), checkWindowCondition)
    {
    }
}