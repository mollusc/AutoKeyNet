using System.Runtime.InteropServices;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using static Windows.Win32.PInvoke;

namespace AutoKeyNet.RuleRecords;

/// <summary>
///     Class responsible for defining hotkey rules.
/// </summary>
public class HotKeyRuleRecord : BaseRuleRecord
{
    public HotKeyRuleRecord(INPUT[]? hotKeys, IEnumerable<INPUT> sendKeys, WindowCondition? checkWindowCondition = null)
        : base(hotKeys, () =>
        {
            var inputs = sendKeys as INPUT[] ?? sendKeys.ToArray();
            SendInput(inputs.ToArray(), Marshal.SizeOf(typeof(INPUT)));
        }, checkWindowCondition)
    {
    }
    public HotKeyRuleRecord(INPUT[]? hotKeys, Action hotKeyRun, WindowCondition? checkWindowCondition = null)
        : base(hotKeys, hotKeyRun, checkWindowCondition)
    {
    }
}