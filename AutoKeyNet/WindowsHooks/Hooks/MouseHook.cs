using System.Diagnostics;
using System.Runtime.InteropServices;
using AutoKeyNet.WindowsHooks.Helper;
using AutoKeyNet.WindowsHooks.Hooks.EventArgs;
using AutoKeyNet.WindowsHooks.WindowsEnums;
using AutoKeyNet.WindowsHooks.WindowsStruct;

namespace AutoKeyNet.WindowsHooks.Hooks;
/// <summary>
/// Class for mouse hooking
/// </summary>
internal class MouseHook : BaseHook, IHookEvent<MouseHookEventArgs>
{
    /// <summary>
    /// Delegate for the callback function
    /// </summary>
    private readonly HookCallbackDelegate _hookCallback;

    /// <summary>
    /// Constructor of the class for keyboard hooking
    /// </summary>
    public MouseHook()
    {
        _hookCallback = LowLevelMouseProc;
        InitializeHook();
    }

    /// <summary>
    /// Event that is triggered when the mouse is moved or a mouse button is pressed.
    /// </summary>
    public event EventHandler<MouseHookEventArgs>? OnHookEvent;

    /// <summary>
    /// Set of the mouse hook
    /// </summary>
    /// <returns>Identifier for the hook</returns>
    protected override nint SetHook()
    {
        using Process curProcess = Process.GetCurrentProcess();
        using ProcessModule? curModule = curProcess.MainModule;
        if (curModule != null)
        {
            return SetWindowsHookEx((int)HookType.WH_MOUSE_LL, _hookCallback, GetModuleHandle(curModule.ModuleName),
                0);
        }

        throw new NullReferenceException();
    }

    /// <summary>
    /// Callback function that is called when a Windows hook is executed.
    /// To prevent sending a mouse event to the system, you need to set MouseHookEventArgs.Cancel to true
    /// </summary>
    /// <param name="nCode">A code the hook procedure uses to determine how to process the message</param>
    /// <param name="wParam">The identifier of the mouse message</param>
    /// <param name="lParam">A pointer to an Windows API MSLLHOOKSTRUCT structure</param>
    /// <returns>A code the hook procedure uses to determine how to process the message</returns>
    /// <exception cref="InvalidOperationException">An exception occurs when there is an error in retrieving
    /// the MouseLowLevelHook struct from the lParam parameter.</exception>
    private nint LowLevelMouseProc(int nCode, nint wParam, nint lParam)
    {
        if (nCode >= Constants.HC_ACTION)
        {
            MouseLowLevelHook hookStruct = (MouseLowLevelHook)(Marshal.PtrToStructure(lParam, typeof(MouseLowLevelHook)) ??
                                                         throw new InvalidOperationException());
            if (hookStruct.dwExtraInfo != (nuint)Constants.KEY_IGNORE)
            {
                MouseHookEventArgs mouseHookEventArgs = new MouseHookEventArgs(wParam, lParam,
                    hookStruct.mouseData >> 16, WindowHelper.GetActiveWindowTitle(),
                    WindowHelper.GetActiveWindowClass(),
                    WindowHelper.GetActiveWindowModuleFileName(), WindowHelper.GetActiveWindowFocusControlName());
                OnHookEvent?.Invoke(wParam, mouseHookEventArgs);
            }
        }

        return CallNextHookEx(HookId, nCode, wParam, lParam);
    }

    /// <summary>
    /// Remove of the mouse hook
    /// </summary>
    protected override void Unhook() => UnhookWindowsHookEx(HookId);

    #region Windows API functions
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(nint hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern nint SetWindowsHookEx(int idHook, HookCallbackDelegate lpfn, nint hMod,
        uint dwThreadId);

    private delegate nint HookCallbackDelegate(int nCode, nint wParam, nint lParam);
    #endregion
}