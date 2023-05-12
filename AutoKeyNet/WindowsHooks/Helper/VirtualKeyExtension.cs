using System.Text;
using AutoKeyNet.WindowsHooks.WindowsEnums;
using AutoKeyNet.WindowsHooks.WindowsStruct;
using static AutoKeyNet.WindowsHooks.WinApi.NativeMethods;

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
    internal static Input ToInput(this VirtualKey virtualKey, KeyEventFlags flags,
        nuint extraInfo = KEY_IGNORE) => virtualKey switch
    {
        VirtualKey.LBUTTON when flags.HasFlag(KeyEventFlags.KEYUP) => GetMouseInput(MouseEvents.LEFTUP),
        VirtualKey.LBUTTON when flags.HasFlag(KeyEventFlags.KEYDOWN) => GetMouseInput(MouseEvents.LEFTDOWN),
        VirtualKey.RBUTTON when flags.HasFlag(KeyEventFlags.KEYUP) => GetMouseInput(MouseEvents.RIGHTUP),
        VirtualKey.RBUTTON when flags.HasFlag(KeyEventFlags.KEYDOWN) => GetMouseInput(MouseEvents.RIGHTDOWN),
        VirtualKey.MBUTTON when flags.HasFlag(KeyEventFlags.KEYUP) => GetMouseInput(MouseEvents.MIDDLEUP),
        VirtualKey.MBUTTON when flags.HasFlag(KeyEventFlags.KEYDOWN) => GetMouseInput(MouseEvents.MIDDLEDOWN),
        VirtualKey.XBUTTON1 when flags.HasFlag(KeyEventFlags.KEYUP) => GetMouseInput(MouseEvents.XUP, XBUTTON1),
        VirtualKey.XBUTTON1 when flags.HasFlag(KeyEventFlags.KEYDOWN) => GetMouseInput(MouseEvents.XDOWN, XBUTTON1),
        VirtualKey.XBUTTON2 when flags.HasFlag(KeyEventFlags.KEYUP) => GetMouseInput(MouseEvents.XUP, XBUTTON2),
        VirtualKey.XBUTTON2 when flags.HasFlag(KeyEventFlags.KEYDOWN) => GetMouseInput(MouseEvents.XDOWN, XBUTTON2),

        _ => GetKeyboardInput(virtualKey, flags, extraInfo)
    };

    private static Input GetMouseInput(MouseEvents mouseEvents, uint mouseData = 0, nuint extraInfo = KEY_IGNORE) =>
        new Input()
        {
            type = InputType.INPUT_MOUSE,
            U = new InputUnion()
            {
                mi = new MouseInput()
                {
                    dwFlags = mouseEvents,
                    mouseData = (int)mouseData,
                    dwExtraInfo = extraInfo,
                }
            }
        };

    private static Input GetKeyboardInput(VirtualKey virtualKey, KeyEventFlags flags, nuint extraInfo) =>
        new Input
        {
            type = InputType.INPUT_KEYBOARD,
            U = new InputUnion
            {
                ki = new KeyboardInput
                {
                    wVk = (ushort)virtualKey,
                    wScan = (ushort)MapVirtualKey((uint)virtualKey, MAPVK_VK_TO_VSC),
                    dwFlags = flags,
                    dwExtraInfo = extraInfo,
                },
            }
        };

    /// <summary>
    /// Method for converting a virtual key to an Input structure with a key down and key up events
    /// </summary>
    /// <param name="virtualKey">Virtual key to be converted to an Input structure</param>
    /// <param name="flags">Specifies various aspects of a keystroke</param>
    /// <param name="extraInfo">An additional value associated with the keystroke</param>
    /// <returns>Input structures that represent the virtual key</returns>
    internal static IEnumerable<Input> ToInputsPressKey(this VirtualKey virtualKey, KeyEventFlags flags = 0,
        nuint extraInfo = KEY_IGNORE)
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
        var lScanCode = MapVirtualKey((uint)vkCode, MAPVK_VK_TO_VSC);
        if (!isInvariantCulture)
        {
            var focusedHWnd = GetForegroundWindow();
            var activeThread = GetWindowThreadProcessId(focusedHWnd, out uint processId);
            hkl = GetKeyboardLayout(activeThread);
            //Debug.WriteLine($"ForegroundWindow={focusedHWnd}, ActiveThread={activeThread}, ProcessId={processId}, KeyboardLayout={hkl}");
        }

        ToUnicodeEx(vkCode, lScanCode, bKeyState, sbString, (int)5, (uint)0, hkl);
        return sbString.Length > 0 ? sbString[0] : '\0';
    }
}