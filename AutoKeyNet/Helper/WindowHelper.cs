using System.ComponentModel;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using static Windows.Win32.PInvoke;

namespace AutoKeyNet.Helper;

internal static class WindowHelper
{
    /// <summary>
    ///     This method retrieves the title of the currently active window.
    /// </summary>
    /// <returns>Title</returns>
    internal static string? GetActiveWindowTitle()
    {
        var handle = GetForegroundWindow();
        if (handle == HWND.Null)
            return null;
        var bufferSize = GetWindowTextLength(handle) + 1;
        unsafe
        {
            fixed (char* buff = new char[bufferSize])
            {
                if (GetWindowText(handle, buff, bufferSize) == 0)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    if (errorCode != 0)
                        throw new Win32Exception(errorCode);
                    return null;
                }
                return new string(buff);
            }
        }
    }

    /// <summary>
    ///     This method retrieves the class of the currently active window.
    /// </summary>
    /// <returns>Title</returns>
    internal static string? GetActiveWindowClass()
    {
        var handle = GetForegroundWindow();
        if (handle == HWND.Null)
            return null;
        var bufferSize = 256;
        unsafe
        {
            fixed (char* buff = new char[bufferSize])
            {
                if (GetClassName(handle, buff, bufferSize) == 0)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    if (errorCode != 0)
                        throw new Win32Exception(errorCode);
                    return null;
                }
                return new string(buff);
            }
        }
    }

    /// <summary>
    ///     This method retrieves module name (file *.exe) of the currently active window
    /// </summary>
    /// <returns>Module name</returns>
    internal static string? GetActiveWindowModuleFileName()
    {
        var handle = GetForegroundWindow();
        if (handle == HWND.Null)
            return null;
        Process[] processlist = Process.GetProcesses();
        uint processId;
        unsafe
        {
            uint* lpdwProcessId = &processId;
            if (GetWindowThreadProcessId(handle, lpdwProcessId) == nint.Zero)
            {
                int errorCode = Marshal.GetLastWin32Error();
                if (errorCode != 0)
                    throw new Win32Exception(errorCode);
            }
            Process? p = processlist.FirstOrDefault(pr => pr.Id == *lpdwProcessId);
            return p?.MainModule?.ModuleName;
        }
    }

    /// <summary>
    ///     This method returns the name of the focused control in the currently active window.
    /// </summary>
    /// <returns>Control name</returns>
    internal static unsafe string? GetActiveWindowFocusControlName()
    {
        var activeWindowHandle = GetForegroundWindow();
        if (activeWindowHandle == HWND.Null)
            return null;
        if (!GetInfo(activeWindowHandle, out var info) || info.hwndFocus == HWND.Null)
            return null;
        var focusedControlHandle = info.hwndFocus;
        int bufferSize = 256;
        fixed (char* buff = new char[bufferSize])
        {
            if (GetClassName(focusedControlHandle, buff, bufferSize) == 0)
            {
                int errorCode = Marshal.GetLastWin32Error();
                if (errorCode != 0)
                    throw new Win32Exception(errorCode);
                return null;
            }
            return new string(buff);
        }
    }


    /// <summary>
    ///     Retrieves information about the active window or a specified GUI thread.
    /// </summary>
    /// <param name="hwnd">A handle to the window.</param>
    /// <param name="lpgui">Information describing the thread</param>
    /// <returns>If the function succeeds, the return value is true. If the function fails, the return value is false.</returns>
    private static unsafe bool GetInfo(HWND hwnd, out GUITHREADINFO lpgui)
    {
        var threadId = GetWindowThreadProcessId(hwnd);

        lpgui = new GUITHREADINFO();
        lpgui.cbSize = (uint)Marshal.SizeOf(lpgui);

        return GetGUIThreadInfo(threadId, ref lpgui);
    }
}