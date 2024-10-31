using System.Runtime.InteropServices;

namespace LogKeyConsoleApp.WinApi;

public static class NativeMethods
{
    public const uint KEY_IGNORE = 0xFFC3D44F;
    internal const uint HC_ACTION = 0;
    public const uint XBUTTON1 = 0x0001<<16;
    public const uint XBUTTON2 = 0x0002<<16;

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern nint CallNextHookEx(nint hhk, int nCode, nint wParam, nint lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern nint GetModuleHandle(string lpModuleName);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool UnhookWindowsHookEx(nint hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern nint SetWindowsHookEx(int idHook, HookCallbackDelegate lpfn, nint hMod, uint dwThreadId);

    internal delegate nint HookCallbackDelegate(int nCode, nint wParam, nint lParam);
}