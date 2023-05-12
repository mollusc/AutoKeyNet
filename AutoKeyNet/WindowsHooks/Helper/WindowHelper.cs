using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using AutoKeyNet.WindowsHooks.WindowsStruct;

namespace AutoKeyNet.WindowsHooks.Helper;
internal static class WindowHelper
{
    /// <summary>
    /// This method retrieves the title of the currently active window.
    /// </summary>
    /// <returns>Title</returns>
    internal static string? GetActiveWindowTitle()
    {
        const int nChars = 256;
        StringBuilder buff = new StringBuilder(nChars);
        IntPtr handle = GetForegroundWindow();

        if (GetWindowText(handle, buff, nChars) > 0)
        {
            return buff.ToString();
        }

        return null;
    }

    internal static string? GetActiveWindowClass()
    {
        const int nChars = 256;
        StringBuilder buff = new StringBuilder(nChars);
        IntPtr handle = GetForegroundWindow();

        if (GetClassName(handle, buff, nChars) > 0)
        {
            return buff.ToString();
        }

        return null;
    }

    internal static string? GetActiveWindowModuleFileName()
    {
        try
        {
            IntPtr handle = GetForegroundWindow();
            if (handle == IntPtr.Zero)
                return null;
            GetWindowThreadProcessId(handle, out var processId);
            Process p = Process.GetProcessById((int)processId);
            return p.MainModule?.ModuleName;
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

    internal static string? GetActiveWindowFocusControlName()
    {
        IntPtr activeWindowHandle = GetForegroundWindow();

        IntPtr activeWindowThread = GetWindowThreadProcessId(activeWindowHandle, IntPtr.Zero);

        GetInfo(activeWindowThread, out GUIThreadInfo info);
        IntPtr focusedControlHandle = info.hwndFocus;

        StringBuilder className = new StringBuilder(256);
        if (GetClassName(focusedControlHandle, className, className.Capacity) != 0)
            return className.ToString();
        return null;
    }


    public static bool GetInfo(IntPtr hwnd, out GUIThreadInfo lpgui)
    {
        uint threadId = GetWindowThreadProcessId(hwnd, out _);

        lpgui = new GUIThreadInfo();
        lpgui.cbSize = Marshal.SizeOf(lpgui);

        return GetGUIThreadInfo(threadId, ref lpgui);
    }

    #region Windows API functions
    [DllImport("user32.dll")]
    static extern bool GetGUIThreadInfo(uint idThread, ref GUIThreadInfo lpgui);

    [DllImport("user32.dll")]
    static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("user32.dll")]
    static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
    #endregion
}