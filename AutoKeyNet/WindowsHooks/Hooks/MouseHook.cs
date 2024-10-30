using System.Diagnostics;
using System.Runtime.InteropServices;
using AutoKeyNet.WindowsHooks.Helper;
using AutoKeyNet.WindowsHooks.Hooks.EventArgs;
using AutoKeyNet.WindowsHooks.WindowsEnums;
using AutoKeyNet.WindowsHooks.WindowsStruct;
using static AutoKeyNet.WindowsHooks.WinApi.NativeMethods;

namespace AutoKeyNet.WindowsHooks.Hooks;

/// <summary>
///     Class for mouse hooking
/// </summary>
internal class MouseHook : BaseHook<HookEventArgs>
{
    /// <summary>
    ///     Delegate for the callback function
    /// </summary>
    private readonly HookCallbackDelegate _hookCallback;

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
    protected override nint SetHook()
    {
        using var curProcess = Process.GetCurrentProcess();
        using var curModule = curProcess.MainModule;
        if (curModule != null)
            return SetWindowsHookEx((int)HookType.WH_MOUSE_LL, _hookCallback, GetModuleHandle(curModule.ModuleName), 0);
        throw new NullReferenceException();
    }

    /// <summary>
    ///     Callback function that is called when a mouse hook is executed.
    ///     To prevent sending a mouse event to the system, you need to set MouseHookEventArgs.Cancel to true
    /// </summary>
    /// <param name="nCode">A code the hook procedure uses to determine how to process the message</param>
    /// <param name="wParam">The identifier of the mouse message</param>
    /// <param name="lParam">A pointer to an Windows API MSLLHOOKSTRUCT structure</param>
    /// <returns>A code the hook procedure uses to determine how to process the message</returns>
    /// <exception cref="InvalidOperationException">
    ///     An exception occurs when there is an error in retrieving
    ///     the MouseLowLevelHook struct from the lParam parameter.
    /// </exception>
    private nint LowLevelMouseProc(int nCode, nint wParam, nint lParam)
    {
        if (nCode >= HC_ACTION)
        {
            var hookStruct = (MouseLowLevelHook)(Marshal.PtrToStructure(lParam, typeof(MouseLowLevelHook)) ??
                                                 throw new InvalidOperationException());
            if (hookStruct.ExtraInfo != KEY_IGNORE)
            {
                MouseEvents mouseEvent = wParam switch
                {
                    (nint)MouseMessage.WM_LBUTTONUP => MouseEvents.LEFTUP,
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

                Input input = new()
                {
                    Type = InputType.INPUT_MOUSE,
                    Data = new ()
                    {
                        MouseInput = new()
                        {
                            Dx = hookStruct.Point.X,
                            Dy = hookStruct.Point.Y,
                            MouseData = hookStruct.MouseData,
                            Flags = mouseEvent,
                            Time = (uint)hookStruct.Time,
                            ExtraInfo = hookStruct.ExtraInfo,
                        }
                    }
                };
                var mouseHookEventArgs = new HookEventArgs(input, WindowHelper.GetActiveWindowTitle(),
                    WindowHelper.GetActiveWindowClass(),
                    WindowHelper.GetActiveWindowModuleFileName(), WindowHelper.GetActiveWindowFocusControlName());

                OnHookEvent(mouseHookEventArgs);
                if (mouseHookEventArgs.Cancel)
                    return 1;
            }
        }

        return CallNextHookEx(HookId, nCode, wParam, lParam);
    }

    /// <summary>
    ///     Remove of the mouse hook
    /// </summary>
    protected override void Unhook() => UnhookWindowsHookEx(HookId);
}