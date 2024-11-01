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
    protected override HHOOK SetHook()
    {
        var hMod = new HINSTANCE(Marshal.GetHINSTANCE(typeof(KeyboardHook).Module));
        return SetWindowsHookEx(WINDOWS_HOOK_ID.WH_MOUSE_LL, _hookCallback, hMod, 0);
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
            if (hookStruct.dwExtraInfo != KEY_IGNORE)
            {
                MouseEvents mouseEvent = wparam switch
                {
                    WM_LBUTTONUP => MouseEvents.LEFTUP,
                    (nint)MouseMessage.WM_LBUTTONDOWN => MouseEvents.LEFTDOWN,
                    (nint)MouseMessage.WM_RBUTTONUP => MouseEvents.RIGHTUP,
                    (nint)MouseMessage.WM_RBUTTONDOWN => MouseEvents.RIGHTDOWN,
                    (nint)MouseMessage.WM_MBUTTONUP => MouseEvents.MIDDLEUP,
                    (nint)MouseMessage.WM_MBUTTONDOWN => MouseEvents.MIDDLEDOWN,
                    (nint)MouseMessage.WM_XBUTTONUP => MouseEvents.XUP,
                    (nint)MouseMessage.WM_XBUTTONDOWN => MouseEvents.XDOWN,
                    (nint)MouseMessage.WM_MOUSEMOVE => MouseEvents.MOVE,
                    (nint)MouseMessage.WM_MOUSEWHEEL => MouseEvents.WHEEL,
                    _ => 0
                };

                INPUT input = new()
                {
                    type = INPUT_TYPE.INPUT_MOUSE,
                    Anonymous = new INPUT._Anonymous_e__Union
                    {
                        mi = new MOUSEINPUT
                        {
                            dx = hookStruct.Point.X

                        }
                    }
                };
                
                //    = new ()
                //    {
                //        MouseInput = new()
                //        {
                //            Dx = hookStruct.Point.X,
                //            Dy = hookStruct.Point.Y,
                //            MouseData = hookStruct.MouseData,
                //            Flags = mouseEvent,
                //            Time = (uint)hookStruct.Time,
                //            ExtraInfo = hookStruct.ExtraInfo,
                //        }
                //    }
                //};
                var mouseHookEventArgs = new HookEventArgs(input, WindowHelper.GetActiveWindowTitle(),
                    WindowHelper.GetActiveWindowClass(),
                    WindowHelper.GetActiveWindowModuleFileName(), WindowHelper.GetActiveWindowFocusControlName());

                OnHookEvent(mouseHookEventArgs);
                if (mouseHookEventArgs.Cancel)
                    return 1;
            }
        }

        return CallNextHookEx(HookId, nCode, wparam, lparam);
    }

    /// <summary>
    ///     Remove of the mouse hook
    /// </summary>
    protected override void Unhook() => UnhookWindowsHookEx(HookId);
}