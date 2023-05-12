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
internal class MouseHook : BaseHook, IHookEvent<MouseHookEventArgs>
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
    ///     Event that is triggered when the mouse is moved or a mouse button is pressed.
    /// </summary>
    public event EventHandler<MouseHookEventArgs>? OnHookEvent;

    /// <summary>
    ///     Set of the mouse hook
    /// </summary>
    /// <returns>Identifier for the hook</returns>
    protected override nint SetHook()
    {
        using var curProcess = Process.GetCurrentProcess();
        using var curModule = curProcess.MainModule;
        if (curModule != null)
            return SetWindowsHookEx((int)HookType.WH_MOUSE_LL, _hookCallback, GetModuleHandle(curModule.ModuleName),
                0);

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
                var mouseHookEventArgs = new MouseHookEventArgs(wParam, lParam,
                    hookStruct.MouseData >> 16, WindowHelper.GetActiveWindowTitle(),
                    WindowHelper.GetActiveWindowClass(),
                    WindowHelper.GetActiveWindowModuleFileName(), WindowHelper.GetActiveWindowFocusControlName());
                OnHookEvent?.Invoke(wParam, mouseHookEventArgs);
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