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
    private static bool _isLShiftDown;
    private static bool _isRShiftDown;

    private Timer _timer;
    private readonly TimerCallback tm = SetFalse;

    [DllImport("user32.dll")]
    private static extern bool PostMessage(int hhwnd, uint msg, IntPtr wparam, IntPtr lparam);

    [DllImport("user32.dll")]
    private static extern IntPtr LoadKeyboardLayout(string pwszKLID, uint Flags);

    public override List<BaseRuleRecord> Create()
    {
        var rules = new List<BaseRuleRecord>
        {
            new HotKeyRuleRecord("{LSHIFT DOWN}", ChangeLanguageDown()),
            new HotKeyRuleRecord("{LSHIFT UP}", ChangeLanguage(en_US)),

            new HotKeyRuleRecord("{RSHIFT DOWN}", ChangeLanguageDown()),
            new HotKeyRuleRecord("{RSHIFT UP}", ChangeLanguage(ru_RU))
        };
        return rules;
    }

    private static void SetFalse(object? state)
    {
        _isShiftDown = false;
        Debug.WriteLine("_isShiftDown = false;");
    }

    private Action ChangeLanguageDown()
    {
        return () =>
        {
            _isShiftDown = true;
            Debug.WriteLine("_isShiftDown = true;");
            _timer = new Timer(tm, _isShiftDown, 100, 0);
        };
    }

    private static Action ChangeLanguage(string pwszKLID)
    {
        return () =>
        {
            if (_isShiftDown)
            {
                PostMessage(HWND_BROADCAST, WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero,
                    LoadKeyboardLayout(pwszKLID, KLF_ACTIVATE));
            }
        };
    }

    internal delegate nint HookCallbackDelegate(int nCode, nint wParam, nint lParam);
}