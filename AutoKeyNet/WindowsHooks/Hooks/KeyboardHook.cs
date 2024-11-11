using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.WindowsAndMessaging;
using AutoKeyNet.Helper;
using AutoKeyNet.WindowsHooks.Hooks.EventArgs;
using static Windows.Win32.PInvoke;

namespace AutoKeyNet.WindowsHooks.Hooks;

/// <summary>
///     Class for keyboard hooking
/// </summary>
internal class KeyboardHook : BaseHook<HookEventArgs>
{
    /// <summary>
    ///     Delegate for the callback function
    /// </summary>
    private readonly HOOKPROC _hookCallback;

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
        HMODULE hModule;
        unsafe
        {
            fixed (char* lpModuleName = curModule?.ModuleName)
            {
                hModule = GetModuleHandle(lpModuleName);
            }
        }

        var hookHandle = SetWindowsHookEx(WINDOWS_HOOK_ID.WH_KEYBOARD_LL, _hookCallback, hModule, 0);

        if (hookHandle == HHOOK.Null)
        {
            var errorCode = Marshal.GetLastWin32Error();
            if (errorCode != 0)
                throw new Win32Exception(errorCode);
        }

        return hookHandle;
    }

    /// <summary>
    ///     Callback function that is called when a Windows hook is executed.
    ///     To prevent sending a pressed key to the system, you need to set KeyboardHookEventArgs.Cancel to true
    /// </summary>
    /// <param name="nCode">A code the hook procedure uses to determine how to process the message</param>
    /// <param name="wparam">The identifier of the keyboard message</param>
    /// <param name="lparam">A pointer to a Windows API KBDLLHOOKSTRUCT structure</param>
    /// <returns>A code the hook procedure uses to determine how to process the message</returns>
    /// <exception cref="InvalidOperationException">
    ///     An exception occurs when there is an error in retrieving
    ///     the KeyboardLowLevelHook struct from the lParam parameter.
    /// </exception>
    private LRESULT LowLevelKeyboardProc(int nCode, WPARAM wparam, LPARAM lparam)
    {
        if (nCode >= HC_ACTION)
        {
            var kbd = (KBDLLHOOKSTRUCT)(Marshal.PtrToStructure(lparam, typeof(KBDLLHOOKSTRUCT)) ??
                                        throw new InvalidOperationException());
            if (kbd.dwExtraInfo != Constants.KEY_IGNORE)
            {
                INPUT input = new()
                {
                    type = INPUT_TYPE.INPUT_KEYBOARD,
                    Anonymous = new INPUT._Anonymous_e__Union
                    {
                        ki = new KEYBDINPUT
                        {
                            wVk = (VIRTUAL_KEY)kbd.vkCode,
                            wScan = (ushort)kbd.scanCode,
                            time = kbd.time,
                            dwExtraInfo = kbd.dwExtraInfo,
                            dwFlags = wparam == WM_KEYDOWN || wparam == WM_SYSKEYDOWN
                                ? 0
                                : KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP
                        }
                    }
                };

                var keyboardHookEventArgs = new HookEventArgs(input,
                    WindowHelper.GetActiveWindowTitle(),
                    WindowHelper.GetActiveWindowClass(), WindowHelper.GetActiveWindowModuleFileName(),
                    WindowHelper.GetActiveWindowFocusControlName());

                OnHookEvent(keyboardHookEventArgs);
                if (keyboardHookEventArgs.Cancel)
                    return (LRESULT)1;
            }
        }

        return CallNextHookEx((HHOOK)HookId, nCode, wparam, lparam);
    }


    /// <summary>
    ///     Remove of the keyboard hook
    /// </summary>
    protected override void Unhook() => UnhookWindowsHookEx((HHOOK)HookId);
}