using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using AutoKeyNet.WindowsHooks.WindowsStruct;
using static AutoKeyNet.WindowsHooks.WinApi.NativeMethods;

namespace AutoKeyNet.WindowsHooks.Helper;

internal static class WindowHelper
{
    /// <summary>
    ///     This method retrieves the title of the currently active window.
    /// </summary>
    /// <returns>Title</returns>
    internal static string? GetActiveWindowTitle()
    {
        const int nChars = 256;
        var buff = new StringBuilder(nChars);
        var handle = GetForegroundWindow();

        return GetWindowText(handle, buff, nChars) > 0 ? buff.ToString() : null;
    }

    /// <summary>
    ///     This method retrieves the class of the currently active window.
    /// </summary>
    /// <returns>Title</returns>
    internal static string? GetActiveWindowClass()
    {
        const int nChars = 256;
        var buff = new StringBuilder(nChars);
        var handle = GetForegroundWindow();

        return GetClassName(handle, buff, nChars) > 0 ? buff.ToString() : null;
    }

    /// <summary>
    ///     This method retrieves module name (file *.exe) of the currently active window
    /// </summary>
    /// <returns>Module name</returns>
    internal static string? GetActiveWindowModuleFileName()
    {
        try
        {
            var handle = GetForegroundWindow();
            if (handle == nint.Zero)
                return null;
            GetWindowThreadProcessId(handle, out var processId);
            //Process p = Process.GetProcessById((int)processId);
            Process[] processlist = Process.GetProcesses();
            Process? p = processlist.FirstOrDefault(pr => pr.Id == (int)processId);
            return p?.MainModule?.ModuleName;
        }
        catch (Win32Exception ex)
        {
            //EventLog Log = new EventLog();
            //Log.Source = "AutoKeyNet";
            //Log.WriteEntry(ex.Message, EventLogEntryType.Error);

            // TODO: Необходимо выполнить логирование ошибки
        }

        return null;
    }

    /// <summary>
    ///     This method returns the name of the focused control in the currently active window.
    /// </summary>
    /// <returns>Control name</returns>
    internal static string? GetActiveWindowFocusControlName()
    {
        var activeWindowHandle = GetForegroundWindow();
        var activeWindowThread = GetWindowThreadProcessId(activeWindowHandle, out _);

        if (!GetInfo((nint)activeWindowThread, out var info))
            return null;
        var focusedControlHandle = info.FocusedWindowHandle;

        var className = new StringBuilder(256);
        return GetClassName(focusedControlHandle, className, className.Capacity) != 0
            ? className.ToString()
            : null;
    }


    /// <summary>
    ///     Retrieves information about the active window or a specified GUI thread.
    /// </summary>
    /// <param name="hwnd">A handle to the window.</param>
    /// <param name="lpgui">Information describing the thread</param>
    /// <returns>If the function succeeds, the return value is true. If the function fails, the return value is false.</returns>
    private static bool GetInfo(nint hwnd, out GuiThreadInfo lpgui)
    {
        var threadId = GetWindowThreadProcessId(hwnd, out _);

        lpgui = new GuiThreadInfo();
        lpgui.Size = Marshal.SizeOf(lpgui);

        return GetGUIThreadInfo(threadId, ref lpgui);
    }
}