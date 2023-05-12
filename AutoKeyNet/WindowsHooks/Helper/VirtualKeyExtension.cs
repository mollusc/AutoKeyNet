using System.Text;
using AutoKeyNet.WindowsHooks.WindowsEnums;
using AutoKeyNet.WindowsHooks.WindowsStruct;
using static AutoKeyNet.WindowsHooks.WinApi.NativeMethods;

namespace AutoKeyNet.WindowsHooks.Helper;

/// <summary>
///     Extension methods for virtual keys
/// </summary>
internal static class VirtualKeyExtension
{
    /// <summary>
    ///     Method for converting a virtual key to an Input structure with a key down event
    /// </summary>
    /// <param name="virtualKey">The virtual key that will be converted into an Input structure</param>
    /// <param name="flags">Specifies various aspects of a keystroke</param>
    /// <param name="extraInfo">An additional value associated with the keystroke</param>
    /// <returns>An Input structure that represents the virtual key</returns>
    internal static Input ToInput(this VirtualKey virtualKey, KeyEventFlags flags,
        nuint extraInfo = KEY_IGNORE) => virtualKey switch
    {
        VirtualKey.LBUTTON when flags.HasFlag(KeyEventFlags.KEYUP) => MouseEvents.LEFTUP.ToInput(),
        VirtualKey.LBUTTON when flags.HasFlag(KeyEventFlags.KEYDOWN) => MouseEvents.LEFTDOWN.ToInput(),
        VirtualKey.RBUTTON when flags.HasFlag(KeyEventFlags.KEYUP) => MouseEvents.RIGHTUP.ToInput(),
        VirtualKey.RBUTTON when flags.HasFlag(KeyEventFlags.KEYDOWN) => MouseEvents.RIGHTDOWN.ToInput(),
        VirtualKey.MBUTTON when flags.HasFlag(KeyEventFlags.KEYUP) => MouseEvents.MIDDLEUP.ToInput(),
        VirtualKey.MBUTTON when flags.HasFlag(KeyEventFlags.KEYDOWN) => MouseEvents.MIDDLEDOWN.ToInput(),
        VirtualKey.XBUTTON1 when flags.HasFlag(KeyEventFlags.KEYUP) => MouseEvents.XUP.ToInput(XBUTTON1),
        VirtualKey.XBUTTON1 when flags.HasFlag(KeyEventFlags.KEYDOWN) => MouseEvents.XDOWN.ToInput(XBUTTON1),
        VirtualKey.XBUTTON2 when flags.HasFlag(KeyEventFlags.KEYUP) => MouseEvents.XUP.ToInput(XBUTTON2),
        VirtualKey.XBUTTON2 when flags.HasFlag(KeyEventFlags.KEYDOWN) => MouseEvents.XDOWN.ToInput(XBUTTON2),

        _ => GetKeyboardInput(virtualKey, flags, extraInfo)
    };

    /// <summary>
    ///     Converts a virtual key to a keyboard input
    /// </summary>
    /// <param name="virtualKey">The virtual key that will be converted into an Input structure</param>
    /// <param name="flags">Specifies various aspects of a keystroke</param>
    /// <param name="extraInfo">An additional value associated with the keystroke</param>
    /// <returns>A keyboard Input structure that represents the virtual key</returns>
    private static Input GetKeyboardInput(VirtualKey virtualKey, KeyEventFlags flags, nuint extraInfo) =>
        new()
        {
            Type = InputType.INPUT_KEYBOARD,
            Data = new InputUnion
            {
                KeyboardInput = new KeyboardInput
                {
                    VirtualKey = (ushort)virtualKey,
                    ScanCode = (ushort)MapVirtualKey((uint)virtualKey, MAPVK_VK_TO_VSC),
                    Flags = flags,
                    ExtraInfo = extraInfo
                }
            }
        };

    /// <summary>
    ///     Method for converting a virtual key to an Input structure with a key down and key up events
    /// </summary>
    /// <param name="virtualKey">Virtual key to be converted to an Input structure</param>
    /// <param name="flags">Specifies various aspects of a keystroke</param>
    /// <param name="extraInfo">An additional value associated with the keystroke</param>
    /// <returns>Input structures that represent the virtual key</returns>
    internal static IEnumerable<Input> ToInputsPressKey(this VirtualKey virtualKey, KeyEventFlags flags = 0,
        nuint extraInfo = KEY_IGNORE)
    {
        foreach (var extraFlag in new[] { KeyEventFlags.KEYDOWN, KeyEventFlags.KEYUP })
            yield return virtualKey.ToInput(flags | extraFlag, extraInfo);
    }

    /// <summary>
    ///     Converts a virtual key code to the corresponding Unicode character,
    ///     taking into account the current keyboard layout. If the virtual key code does
    ///     not have a corresponding character in the current layout, the method returns the null character ('\0').
    /// </summary>
    /// <param name="vkCode">Virtual key code</param>
    /// <param name="isInvariantCulture">
    ///     If parameter is set to false, the method will not take into
    ///     account the current language keyboard layout
    /// </param>
    /// <returns>Unicode character</returns>
    internal static char ToUnicode(this VirtualKey vkCode, bool isInvariantCulture = false)
    {
        var sbString = new StringBuilder();

        var bKeyState = new byte[256];
        GetKeyState(VirtualKey.SHIFT);
        GetKeyState(VirtualKey.MENU);
        var bKeyStateStatus = GetKeyboardState(bKeyState);
        if (!bKeyStateStatus)
            return '\0';
        var hkl = nint.Zero;
        var lScanCode = MapVirtualKey((uint)vkCode, MAPVK_VK_TO_VSC);
        if (!isInvariantCulture)
        {
            var focusedHWnd = GetForegroundWindow();
            var activeThread = GetWindowThreadProcessId(focusedHWnd, out var processId);
            hkl = GetKeyboardLayout(activeThread);
        }

        ToUnicodeEx(vkCode, lScanCode, bKeyState, sbString, 5, 0, hkl);
        return sbString.Length > 0 ? sbString[0] : '\0';
    }
}