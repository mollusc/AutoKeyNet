using System.Diagnostics;
using System.Runtime.InteropServices;
using LogKeyConsoleApp.WindowsEnums;
using LogKeyConsoleApp.WindowsStruct;
using static LogKeyConsoleApp.WinApi.NativeMethods;

namespace LogKeyConsoleApp;

/// <summary>
///     Class for mouse hooking
/// </summary>
internal class MouseHook : BaseHook
{
    /// <summary>
    ///     Delegate for the callback function
    /// </summary>
    private readonly HookCallbackDelegate _hookCallback;

    public MouseHook()
    {
        _hookCallback = LowLevelMouseProc;
        InitializeHook();
    }

    protected override nint SetHook()
    {
        using var curProcess = Process.GetCurrentProcess();
        using var curModule = curProcess.MainModule;
        if (curModule != null)
            return SetWindowsHookEx((int)HookType.WH_MOUSE_LL, _hookCallback, GetModuleHandle(curModule.ModuleName), 0);
        throw new NullReferenceException();
    }

    private nint LowLevelMouseProc(int nCode, nint wParam, nint lParam)
    {
        if (nCode >= HC_ACTION)
        {
            var hookStruct = (MouseLowLevelHook)(Marshal.PtrToStructure(lParam, typeof(MouseLowLevelHook)) ??
                                                 throw new InvalidOperationException());
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
                Data = new()
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
            if (FilterInput(input))
                Debug.WriteLine("info: M: " + input.ToString());
        }
        return CallNextHookEx(HookId, nCode, wParam, lParam);
    }

    protected override void Unhook() => UnhookWindowsHookEx(HookId);
}