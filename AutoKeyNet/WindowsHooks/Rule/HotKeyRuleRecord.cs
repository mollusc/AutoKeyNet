using System.DirectoryServices.ActiveDirectory;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AutoKeyNet.WindowsHooks.Helper;
using AutoKeyNet.WindowsHooks.Hooks.EventArgs;
using AutoKeyNet.WindowsHooks.WindowsEnums;
using AutoKeyNet.WindowsHooks.WindowsStruct;

namespace AutoKeyNet.WindowsHooks.Rule;

/// <summary>
/// Class responsible for defining hotkey rules. 
/// </summary>
public class HotKeyRuleRecord : BaseRuleRecord
{
    public HotKeyRuleRecord(string keyWord, Action hotKeyRun, WindowCondition? checkWindowCondition = null)
        : base(keyWord, hotKeyRun, checkWindowCondition)
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
