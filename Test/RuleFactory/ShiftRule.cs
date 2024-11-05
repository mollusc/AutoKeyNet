using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using AutoKeyNet.WindowsHooks.Helper;
using AutoKeyNet.WindowsHooks.Rule;
using Action = System.Action;
using Timer = System.Threading.Timer;
using static Windows.Win32.PInvoke;

namespace AutoKeyNetApp.RuleFactory;

internal class ShiftRule : BaseRuleFactory
{
    //private const uint WM_INPUTLANGCHANGEREQUEST = 0x0050;
    //private const int HWND_BROADCAST = 0xffff;
    private static readonly string en_US = "00000409";
    private static readonly string ru_RU = "00000419";
    //private static readonly uint KLF_ACTIVATE = 1;
    private bool _isShiftDown;

    private Timer? _timer;

    //[DllImport("user32.dll")]
    //private static extern bool PostMessage(int hhwnd, uint msg, IntPtr wparam, IntPtr lparam);

    //[DllImport("user32.dll")]
    //private static extern IntPtr LoadKeyboardLayout(string pwszKLID, uint Flags);

    public override List<BaseRuleRecord> Create()
    {
        var rules = new List<BaseRuleRecord>
        {
            new HotKeyRuleRecord([VIRTUAL_KEY.VK_LSHIFT.ToInput()], ActivateTimer()),
            new HotKeyRuleRecord([VIRTUAL_KEY.VK_LSHIFT.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP)], TryChangeLanguage(en_US)),

            new HotKeyRuleRecord([VIRTUAL_KEY.VK_RSHIFT.ToInput()], ActivateTimer()),
            new HotKeyRuleRecord([VIRTUAL_KEY.VK_RSHIFT.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP)], TryChangeLanguage(ru_RU))
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

    private Action TryChangeLanguage(string language)
    {
        return () =>
        {
            if (_isShiftDown)
            {
                unsafe
                {
                    fixed (char* pwszKLID = language)
                    {
                        LPARAM lParam = new LPARAM(LoadKeyboardLayout(pwszKLID, ACTIVATE_KEYBOARD_LAYOUT_FLAGS.KLF_ACTIVATE));
                        PostMessage(HWND_BROADCAST, WM_INPUTLANGCHANGEREQUEST, 0, lParam);
                    }
                }
            }
            _timer?.Dispose();
        };
    }
}