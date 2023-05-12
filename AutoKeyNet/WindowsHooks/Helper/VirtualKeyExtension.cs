using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using AutoKeyNet.WindowsHooks.WindowsEnums;
using AutoKeyNet.WindowsHooks.WindowsStruct;

namespace AutoKeyNet.WindowsHooks.Helper;

/// <summary>
/// Extension methods for virtual keys
/// </summary>
internal static class VirtualKeyExtension
{

    /// <summary>
    /// Method for converting a virtual key to an Input structure with a key down event
    /// </summary>
    /// <param name="virtualKey">The virtual key that will be converted into an Input structure</param>
    /// <param name="flags">Specifies various aspects of a keystroke</param>
    /// <param name="extraInfo">An additional value associated with the keystroke</param>
    /// <returns>An Input structure that represents the virtual key</returns>
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
    /// Method for converting a virtual key to an Input structure with a key down and key up events
    /// </summary>
    /// <param name="virtualKey">Virtual key to be converted to an Input structure</param>
    /// <param name="flags">Specifies various aspects of a keystroke</param>
    /// <param name="extraInfo">An additional value associated with the keystroke</param>
    /// <returns>Input structures that represent the virtual key</returns>
    internal static IEnumerable<Input> ToInputsPressKey(this VirtualKey virtualKey, KeyEventFlags flags = 0,
        nuint extraInfo = Constants.KEY_IGNORE)
    {
        foreach (KeyEventFlags extraFlag in new[] { KeyEventFlags.KEYDOWN, KeyEventFlags.KEYUP })
            yield return virtualKey.ToInput(flags | extraFlag, extraInfo);
    }

    /// <summary>
    /// Преобразование виртуальной клавиши в конкретный символ Юникода с учетом регистра
    /// (клавиши Shift)
    /// </summary>
    /// <param name="vkCode">Virtual key code</param>
    /// <param name="isInvariantCulture">If parameter is set to false, the method will not take into
    /// account the current language keyboard layout</param>
    /// <returns>Unicode character</returns>
    internal static char ToUnicode(this VirtualKey vkCode, bool isInvariantCulture = false)
    {
        StringBuilder sbString = new StringBuilder();

        byte[] bKeyState = new byte[256];
        GetKeyState(VirtualKey.SHIFT);
        GetKeyState(VirtualKey.MENU);
        bool bKeyStateStatus = GetKeyboardState(bKeyState);
        if (!bKeyStateStatus)
            return '\0';
        nint hkl = nint.Zero;
        var lScanCode = MapVirtualKey((uint)vkCode, Constants.MAPVK_VK_TO_VSC);
        if (!isInvariantCulture)
        {
            var focusedHWnd = GetForegroundWindow();
            var activeThread = GetWindowThreadProcessId(focusedHWnd, out uint processId);
            hkl = GetKeyboardLayout(activeThread);
            Debug.WriteLine($"ForegroundWindow={focusedHWnd}, ActiveThread={activeThread}, ProcessId={processId}, KeyboardLayout={hkl}");
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
        [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff, int cchBuff, uint wFlags, nint dwhkl);

    [DllImport("user32.dll")]
    private static extern nint GetKeyboardLayout(uint idThread);

    [DllImport("user32.dll", ExactSpelling = true)]
    internal static extern uint GetWindowThreadProcessId(nint hwindow, out uint processId);

    [DllImport("user32.dll")]
    private static extern nint GetForegroundWindow();
    #endregion
}