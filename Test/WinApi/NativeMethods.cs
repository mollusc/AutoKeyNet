using System.Runtime.InteropServices;

namespace AutoKeyNet.WindowsHooks.WinApi;

public static class NativeMethods
{
    public const int SW_MAXIMIZE = 3;

    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(nint hWnd);

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(nint hWnd, int nCmdShow);
}