using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Action = System.Action;
using Timer = System.Threading.Timer;
using static Windows.Win32.PInvoke;
using AutoKeyNet.Helper;
using AutoKeyNet.RuleRecords;
using System.ComponentModel;

namespace AutoKeyNetApp.RuleFactory;

internal class ShiftRule : BaseRuleFactory
{
    private static readonly string en_US = "00000409";
    private static readonly string ru_RU = "00000419";
    private bool _isShiftDown;
    private Timer? _timer;

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
                nint lParam;
                unsafe
                {
                    fixed (char* pwszKLID = language)
                        lParam = LoadKeyboardLayout(pwszKLID, ACTIVATE_KEYBOARD_LAYOUT_FLAGS.KLF_ACTIVATE);
                }
                if (lParam == nint.Zero)
                {
                    var errorCode = Marshal.GetLastWin32Error();
                    if (errorCode != 0)
                        throw new Win32Exception(errorCode);
                }
                PostMessage(HWND_BROADCAST, WM_INPUTLANGCHANGEREQUEST, 0, lParam);
            }
            _timer?.Dispose();
        };
    }
}