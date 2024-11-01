using Windows.Win32.Foundation;
using Windows.Win32.UI.Accessibility;
using Windows.Win32.UI.WindowsAndMessaging;
using AutoKeyNet.WindowsHooks.Helper;
using AutoKeyNet.WindowsHooks.Hooks.EventArgs;
using static Windows.Win32.PInvoke;

namespace AutoKeyNet.WindowsHooks.Hooks;

/// <summary>
///     Class for Windows hooking
/// </summary>
internal class WinHook : BaseHook<WinBaseHookEventArgs>
{
    /// <summary>
    ///     Delegate for the callback function
    /// </summary>
    private readonly WINEVENTPROC _hookEvent;

    /// <summary>
    ///     Constructor of the class for Windows hooking
    /// </summary>
    public WinHook()
    {
        _hookEvent = WinEventProc;
        InitializeHook();
    }

    /// <summary>
    ///     Set of the Windows hook
    /// </summary>
    /// <returns>Identifier for the hook</returns>
    protected override HHOOK SetHook()
    {
        return SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, HMODULE.Null, 
            _hookEvent, 0, 0, WINEVENT_OUTOFCONTEXT);
    }

    /// <summary>
    ///     Callback function that is called when a Windows hook is executed.
    /// </summary>
    /// <param name="hwineventhook">The handle to the WinEvent hook.</param>
    /// <param name="eventType">The type of event that occurred</param>
    /// <param name="hwnd">The handle to the window that triggered the event</param>
    /// <param name="idObject">The ID of the object that triggered the event</param>
    /// <param name="idChild">The ID of the child object that triggered the event</param>
    /// <param name="dwEventThread">The ID of the thread that triggered the event</param>
    /// <param name="dwmsEventTime">The time at which the event occurred</param>
    public void WinEventProc(HWINEVENTHOOK hwineventhook, uint eventType, HWND hwnd, int idObject, int idChild,
        uint dwEventThread, uint dwmsEventTime)
    {
        var winBaseHookEventArgs =
            new WinBaseHookEventArgs(WindowHelper.GetActiveWindowTitle(), eventType, hwnd);
        OnHookEvent(winBaseHookEventArgs);
    }

    /// <summary>
    ///     Remove of the Windows hook
    /// </summary>
    protected override void Unhook() => UnhookWinEvent(HookId);
}