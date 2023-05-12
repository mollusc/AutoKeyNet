using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using AutoKeyNet.WindowsHooks.WindowsEnums;
using AutoKeyNet.WindowsHooks.WindowsStruct;

namespace AutoKeyNet.WindowsHooks.Helper;

internal static class VirtualKeyExtension
{
    internal static IEnumerable<Input> ToInputsPressKey(this VirtualKey virtualKey, KeyEventFlags flags = 0,
        nuint extraInfo = Constants.KEY_IGNORE)
    {
        foreach (KeyEventFlags extraFlag in new[] { KeyEventFlags.KEYDOWN, KeyEventFlags.KEYUP })
            yield return virtualKey.ToInput(flags | extraFlag, extraInfo);
    }

    /// <summary>
    /// Выполняется преобразование KeyInput в Input для последующей отправки через метод SendInput
    /// </summary>
    /// <param name="virtualKey"></param>
    /// <param name="flags"></param>
    /// <param name="extraInfo"></param>
    /// <returns></returns>
    internal static Input ToInput(this VirtualKey virtualKey, KeyEventFlags flags, nuint extraInfo = Constants.KEY_IGNORE)
    {
        return new Input
        {
            type = InputType.INPUT_KEYBOARD,
            U = new InputUnion
            {
                ki = new KeyboardInput
                {
                    wVk = (ushort)virtualKey,
                    wScan = MapVirtualKey((uint)virtualKey, Constants.MAPVK_VK_TO_VSC),
                    dwFlags = flags,
                    dwExtraInfo = extraInfo,
                },
            }
        };
    }


    /// <summary>
    /// Преобразование виртуальной клавиши в конкретный символ Юникода с учетом регистра
    /// (клавиши Shift)
    /// </summary>
    /// <param name="vkCode">Виртуальный код клавиши</param>
    /// <param name="lScanCode"></param>
    /// <param name="isInvariantCulture"></param>
    /// <returns>Символ в формате Юникод</returns>
    internal static char ToUnicode(this VirtualKey vkCode, bool isInvariantCulture = false)
    {
        StringBuilder sbString = new StringBuilder();

        byte[] bKeyState = new byte[256];
        GetKeyState(VirtualKey.SHIFT);
        GetKeyState(VirtualKey.MENU);
        bool bKeyStateStatus = GetKeyboardState(bKeyState);
        if (!bKeyStateStatus)
            return '\0';
        IntPtr hkl = IntPtr.Zero;
        var lScanCode = MapVirtualKey((uint)vkCode, Constants.MAPVK_VK_TO_VSC);
        if (!isInvariantCulture)
        {
            var focusedHWnd = GetForegroundWindow();
            var activeThread = GetWindowThreadProcessId(focusedHWnd, out _);
            hkl = GetKeyboardLayout(activeThread);
        }

        ToUnicodeEx((uint)vkCode, lScanCode, bKeyState, sbString, (int)5, (uint)0, hkl);
        return sbString.Length > 0 ? sbString[0] : '\0';
    }

    [DllImport("user32.dll")]
    private static extern ushort MapVirtualKey(uint uCode, uint uMapType);
    [DllImport("USER32.dll")]
    private static extern short GetKeyState(VirtualKey nVirtKey);

    [DllImport("user32.dll")]
    private static extern bool GetKeyboardState(byte[] lpKeyState);

    [DllImport("user32.dll")]
    private static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState,
        [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);

    [DllImport("user32.dll")]
    private static extern IntPtr GetKeyboardLayout(uint idThread);

    [DllImport("user32.dll", ExactSpelling = true)]
    internal static extern uint GetWindowThreadProcessId(IntPtr hwindow, out uint processId);

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();
}