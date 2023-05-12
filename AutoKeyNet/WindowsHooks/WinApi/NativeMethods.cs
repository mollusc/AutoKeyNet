using System.Runtime.InteropServices;
using System.Text;
using AutoKeyNet.WindowsHooks.WindowsEnums;
using AutoKeyNet.WindowsHooks.WindowsStruct;

namespace AutoKeyNet.WindowsHooks.WinApi;

internal static class NativeMethods
{
    internal const uint KEY_IGNORE = 0xFFC3D44F;
    internal const uint HC_ACTION = 0;
    internal const uint WINEVENT_OUTOFCONTEXT = 0;
    internal const uint EVENT_SYSTEM_FOREGROUND = 3;
    internal const uint MAPVK_VK_TO_VSC = 0x00;
    internal const uint XBUTTON1 = 0x0001;
    internal const uint XBUTTON2 = 0x0002;

    internal static Task SendInputAsync(Input[] inputs) => Task.Run(() => SendInput(inputs));

    internal static void SendInput(Input[] inputs) => SendInput((uint)inputs.Length, inputs, Input.Size);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern uint SendInput(uint nInputs, Input[] pInputs, int cbSize);

    [DllImport("user32.dll")]
    internal static extern uint MapVirtualKey(uint uCode, uint uMapType);

    [DllImport("USER32.dll")]
    internal static extern short GetKeyState(VirtualKey virtualKey);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetKeyboardState(byte[] lpKeyState);

    [DllImport("user32.dll")]
    internal static extern int ToUnicodeEx(VirtualKey virtualKey, uint wScanCode, byte[] lpKeyState,
        [Out] [MarshalAs(UnmanagedType.LPWStr)]
        StringBuilder pwszBuff, int cchBuff, uint wFlags, nint dwhkl);

    [DllImport("user32.dll")]
    internal static extern nint GetKeyboardLayout(uint idThread);

    [DllImport("user32.dll", ExactSpelling = true)]
    internal static extern uint GetWindowThreadProcessId(nint handle, out uint processId);

    [DllImport("user32.dll")]
    internal static extern nint GetForegroundWindow();

    [DllImport("user32.dll")]
    internal static extern bool GetGUIThreadInfo(uint idThread, ref GuiThreadInfo lpgui);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern int GetWindowText(nint handle, StringBuilder text, int count);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern int GetClassName(nint handle, StringBuilder lpClassName, int nMaxCount);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern nint CallNextHookEx(nint hhk, int nCode, nint wParam, nint lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern nint GetModuleHandle(string lpModuleName);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool UnhookWindowsHookEx(nint hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern nint SetWindowsHookEx(int idHook, HookCallbackDelegate lpfn, nint hMod, uint dwThreadId);

    [DllImport("user32.dll")]
    internal static extern nint SetWinEventHook(uint eventMin, uint eventMax, nint hmodWinEventProc,
        WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

    [DllImport("user32.dll")]
    internal static extern bool UnhookWinEvent(nint hWinEventHook);

    internal delegate nint HookCallbackDelegate(int nCode, nint wParam, nint lParam);

    internal delegate void WinEventDelegate(nint hWinEventHook, uint eventType, nint handle, int idObject,
        int idChild,
        uint dwEventThread, uint dwmsEventTime);
}