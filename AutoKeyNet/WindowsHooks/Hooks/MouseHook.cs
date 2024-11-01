using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.WindowsAndMessaging;
using AutoKeyNet.WindowsHooks.Helper;
using AutoKeyNet.WindowsHooks.Hooks.EventArgs;
using static Windows.Win32.PInvoke;
namespace AutoKeyNet.WindowsHooks.Hooks;

/// <summary>
///     Class for mouse hooking
/// </summary>
internal class MouseHook : BaseHook<HookEventArgs>
{
    /// <summary>
    ///     Delegate for the callback function
    /// </summary>
    private readonly HOOKPROC _hookCallback;

    /// <summary>
    ///     Constructor of the class for keyboard hooking
    /// </summary>
    public MouseHook()
    {
        _hookCallback = LowLevelMouseProc;
        InitializeHook();
    }

    /// <summary>
    ///     Set of the mouse hook
    /// </summary>
    /// <returns>Identifier for the hook</returns>
    protected override IntPtr SetHook()
    {
        using Process curProcess = Process.GetCurrentProcess();
        using ProcessModule? curModule = curProcess.MainModule;
        using var hMode = GetModuleHandle(curModule?.ModuleName);
        var hModePtr = new HINSTANCE(hMode.DangerousGetHandle());
        var hookHandle = SetWindowsHookEx(WINDOWS_HOOK_ID.WH_MOUSE_LL, _hookCallback, hModePtr, 0);

        if (hookHandle == IntPtr.Zero)
        {
            throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
        }

        return hookHandle;
    }

    /// <summary>
    ///     Callback function that is called when a mouse hook is executed.
    ///     To prevent sending a mouse event to the system, you need to set MouseHookEventArgs.Cancel to true
    /// </summary>
    /// <param name="nCode">A code the hook procedure uses to determine how to process the message</param>
    /// <param name="wparam">The identifier of the mouse message</param>
    /// <param name="lparam">A pointer to an Windows API MSLLHOOKSTRUCT structure</param>
    /// <returns>A code the hook procedure uses to determine how to process the message</returns>
    /// <exception cref="InvalidOperationException">
    ///     An exception occurs when there is an error in retrieving
    ///     the MouseLowLevelHook struct from the lParam parameter.
    /// </exception>
    private LRESULT LowLevelMouseProc(int nCode, WPARAM wparam, LPARAM lparam)
    {
        if (nCode >= HC_ACTION)
        {
            var hookStruct = (MSLLHOOKSTRUCT)(Marshal.PtrToStructure(lparam, typeof(MSLLHOOKSTRUCT)) ??
                                              throw new InvalidOperationException());
            if (hookStruct.dwExtraInfo != Constants.KEY_IGNORE)
            {
                MOUSE_EVENT_FLAGS mouseEventFlags = (uint)wparam switch
                {
                    WM_LBUTTONUP => MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTUP,
                    WM_LBUTTONDOWN => MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTDOWN,
                    WM_RBUTTONUP => MOUSE_EVENT_FLAGS.MOUSEEVENTF_RIGHTUP,
                    WM_RBUTTONDOWN => MOUSE_EVENT_FLAGS.MOUSEEVENTF_RIGHTDOWN,
                    WM_MBUTTONUP => MOUSE_EVENT_FLAGS.MOUSEEVENTF_MIDDLEUP,
                    WM_MBUTTONDOWN => MOUSE_EVENT_FLAGS.MOUSEEVENTF_MIDDLEDOWN,
                    WM_XBUTTONUP => MOUSE_EVENT_FLAGS.MOUSEEVENTF_XUP,
                    WM_XBUTTONDOWN => MOUSE_EVENT_FLAGS.MOUSEEVENTF_XDOWN,
                    WM_MOUSEMOVE => MOUSE_EVENT_FLAGS.MOUSEEVENTF_MOVE,
                    WM_MOUSEWHEEL => MOUSE_EVENT_FLAGS.MOUSEEVENTF_WHEEL,
                    _ => 0
                };

                INPUT input = new()
                {
                    type = INPUT_TYPE.INPUT_MOUSE,
                    Anonymous = new INPUT._Anonymous_e__Union
                    {
                        mi = new MOUSEINPUT
                        {
                            dx = hookStruct.pt.X,
                            dy = hookStruct.pt.Y,
                            dwExtraInfo = hookStruct.dwExtraInfo,
                            time = hookStruct.time,
                            dwFlags = mouseEventFlags,
                            mouseData = hookStruct.mouseData
                        }
                    }
                };
                var mouseHookEventArgs = new HookEventArgs(input, WindowHelper.GetActiveWindowTitle(),
                    WindowHelper.GetActiveWindowClass(),
                    WindowHelper.GetActiveWindowModuleFileName(), WindowHelper.GetActiveWindowFocusControlName());

                OnHookEvent(mouseHookEventArgs);
                if (mouseHookEventArgs.Cancel)
                    return new LRESULT(1);
            }
        }

        return CallNextHookEx((HHOOK)HookId, nCode, wparam, lparam);
    }

    /// <summary>
    ///     Remove of the mouse hook
    /// </summary>
    protected override void Unhook() => UnhookWindowsHookEx((HHOOK)HookId);
}