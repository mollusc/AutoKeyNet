using System.Diagnostics;
using System.Runtime.InteropServices;
using LogKeyConsoleApp.WindowsEnums;
using LogKeyConsoleApp.WindowsStruct;
using static LogKeyConsoleApp.WinApi.NativeMethods;

namespace LogKeyConsoleApp;
/// <summary>
///     Class for keyboard hooking
/// </summary>
internal class KeyboardHook : BaseHook
{
    /// <summary>
    ///     Delegate for the callback function
    /// </summary>
    private readonly HookCallbackDelegate _hookCallback;

    /// <summary>
    ///     Constructor of the class for keyboard hooking
    /// </summary>
    public KeyboardHook()
    {
        _hookCallback = LowLevelKeyboardProc;
        InitializeHook();
    }

    /// <summary>
    ///     Set of the keyboard hook
    /// </summary>
    /// <returns>Identifier for the hook</returns>
    protected override nint SetHook()
    {
        using var curProcess = Process.GetCurrentProcess();
        using var curModule = curProcess.MainModule;
        if (curModule != null)
            return SetWindowsHookEx((int)HookType.WH_KEYBOARD_LL, _hookCallback, GetModuleHandle(curModule.ModuleName),
                0);

        throw new NullReferenceException();
    }

    private nint LowLevelKeyboardProc(int nCode, nint wParam, nint lParam)
    {
        if (nCode >= HC_ACTION)
        {
            var kbd = (KeyboardLowLevelHook)(Marshal.PtrToStructure(lParam, typeof(KeyboardLowLevelHook)) ??
                                             throw new InvalidOperationException());
            Input input = new()
            {
                Type = InputType.INPUT_KEYBOARD,
                Data = new()
                {
                    KeyboardInput = new()
                    {
                        VirtualKey = (ushort)kbd.VirtualKey,
                        ScanCode = (ushort)kbd.ScanCode,
                        Time = (int)kbd.Time,
                        ExtraInfo = kbd.ExtraInfo,
                        Flags = (wParam == (uint)KeyboardMessage.WM_KEYDOWN || wParam == (uint)KeyboardMessage.WM_SYSKEYDOWN) ? KeyEventFlags.KEYDOWN : KeyEventFlags.KEYUP
                    }
                }
            };
            if (FilterInput(input))
                Debug.WriteLine("info: K: " + input.ToString());
        }
        return CallNextHookEx(HookId, nCode, wParam, lParam);
    }


    /// <summary>
    ///     Remove of the keyboard hook
    /// </summary>
    protected override void Unhook() => UnhookWindowsHookEx(HookId);
}