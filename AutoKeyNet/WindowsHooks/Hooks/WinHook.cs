using AutoKeyNet.WindowsHooks.Helper;
using AutoKeyNet.WindowsHooks.Hooks.EventArgs;
using static AutoKeyNet.WindowsHooks.WinApi.NativeMethods;

namespace AutoKeyNet.WindowsHooks.Hooks;

/// <summary>
///     Class for Windows hooking
/// </summary>
internal class WinHook : BaseHook, IHookEvent<WinBaseHookEventArgs>
{
    /// <summary>
    ///     Delegate for the callback function
    /// </summary>
    private readonly WinEventDelegate _hookEvent;

    /// <summary>
    ///     Constructor of the class for Windows hooking
    /// </summary>
    public WinHook()
    {
        _hookEvent = WinEventProc;
        InitializeHook();
    }

    /// <summary>
    ///     Event that is triggered when a Windows event occurs.
    /// </summary>
    public event EventHandler<WinBaseHookEventArgs>? OnHookEvent;

    /// <summary>
    ///     Set of the Windows hook
    /// </summary>
    /// <returns>Identifier for the hook</returns>
    protected override nint SetHook()
    {
        return SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, nint.Zero,
            _hookEvent, 0, 0, WINEVENT_OUTOFCONTEXT);
    }

    /// <summary>
    ///     Callback function that is called when a Windows hook is executed.
    /// </summary>
    /// <param name="hWinEventHook">The handle to the WinEvent hook.</param>
    /// <param name="eventType">The type of event that occurred</param>
    /// <param name="handle">The handle to the window that triggered the event</param>
    /// <param name="idObject">The ID of the object that triggered the event</param>
    /// <param name="idChild">The ID of the child object that triggered the event</param>
    /// <param name="dwEventThread">The ID of the thread that triggered the event</param>
    /// <param name="dwmsEventTime">The time at which the event occurred</param>
    public void WinEventProc(nint hWinEventHook, uint eventType, nint handle, int idObject, int idChild,
        uint dwEventThread, uint dwmsEventTime)
    {
        var winBaseHookEventArgs =
            new WinBaseHookEventArgs(WindowHelper.GetActiveWindowTitle(), eventType, handle);
        OnHookEvent?.Invoke(hWinEventHook, winBaseHookEventArgs);
    }

    /// <summary>
    ///     Remove of the Windows hook
    /// </summary>
    protected override void Unhook() => UnhookWinEvent(HookId);
}