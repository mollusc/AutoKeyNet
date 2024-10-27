using System.Diagnostics;
using System.Runtime.InteropServices;
using AutoKeyNet.WindowsHooks.Rule;
using Action = System.Action;
using Timer = System.Threading.Timer;

namespace AutoKeyNetApp.RuleFactory;

internal class ShiftRule : BaseRuleFactory
{
    private const uint WM_INPUTLANGCHANGEREQUEST = 0x0050;
    private const int HWND_BROADCAST = 0xffff;
    private static readonly string en_US = "00000409";
    private static readonly string ru_RU = "00000419";
    private static readonly uint KLF_ACTIVATE = 1;
    private bool _isShiftDown;

    private Timer? _timer;

    [DllImport("user32.dll")]
    private static extern bool PostMessage(int hhwnd, uint msg, IntPtr wparam, IntPtr lparam);

    [DllImport("user32.dll")]
    private static extern IntPtr LoadKeyboardLayout(string pwszKLID, uint Flags);

    public override List<BaseRuleRecord> Create()
    {
        var rules = new List<BaseRuleRecord>
        {
            new HotKeyRuleRecord("{LSHIFT DOWN}", ActivateTimer()),
            new HotKeyRuleRecord("{LSHIFT UP}", TryChangeLanguage(en_US)),

            new HotKeyRuleRecord("{RSHIFT DOWN}", ActivateTimer()),
            new HotKeyRuleRecord("{RSHIFT UP}", TryChangeLanguage(ru_RU))
        };
        return rules;
    }

    private void SetFalse(object? state)
    {
        _isShiftDown = false;
    }

    private Action ActivateTimer()
    {
        return () =>
        {
            _isShiftDown = true;
            _timer = new Timer(SetFalse, null, 200, 0);
        };
    }

    private Action TryChangeLanguage(string pwszKLID)
    {
        return () =>
        {
            if (_isShiftDown)
            {
                PostMessage(HWND_BROADCAST, WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero,
                    LoadKeyboardLayout(pwszKLID, KLF_ACTIVATE));
            }
            _timer?.Dispose();
        };
    }
}