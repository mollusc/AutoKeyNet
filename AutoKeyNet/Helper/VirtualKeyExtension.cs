using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.TextServices;
using static Windows.Win32.PInvoke;

namespace AutoKeyNet.Helper;

/// <summary>
///     Extension methods for virtual keys
/// </summary>
public static class VirtualKeyExtension
{
    /// <summary>
    ///     Method for converting a virtual key to an Input structure with a key down event by default
    /// </summary>
    /// <param name="virtualKey">The virtual key that will be converted into an Input structure</param>
    /// <param name="flags">Specifies various aspects of a keystroke. Default is key down.</param>
    /// <param name="extraInfo">An additional value associated with the keystroke</param>
    /// <returns>An Input structure that represents the virtual key</returns>
    public static INPUT ToInput(this VIRTUAL_KEY virtualKey, KEYBD_EVENT_FLAGS flags = 0,
        nuint extraInfo = Constants.KEY_IGNORE) =>
    virtualKey switch
    {
        VIRTUAL_KEY.VK_LBUTTON when flags.HasFlag(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP) => MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTUP.ToInput(),
        VIRTUAL_KEY.VK_LBUTTON when !flags.HasFlag(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP) => MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTDOWN.ToInput(),
        VIRTUAL_KEY.VK_RBUTTON when flags.HasFlag(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP) => MOUSE_EVENT_FLAGS.MOUSEEVENTF_RIGHTUP.ToInput(),
        VIRTUAL_KEY.VK_RBUTTON when !flags.HasFlag(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP) => MOUSE_EVENT_FLAGS.MOUSEEVENTF_RIGHTDOWN.ToInput(),
        VIRTUAL_KEY.VK_MBUTTON when flags.HasFlag(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP) => MOUSE_EVENT_FLAGS.MOUSEEVENTF_MIDDLEUP.ToInput(),
        VIRTUAL_KEY.VK_MBUTTON when !flags.HasFlag(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP) => MOUSE_EVENT_FLAGS.MOUSEEVENTF_MIDDLEDOWN.ToInput(),
        VIRTUAL_KEY.VK_XBUTTON1 when flags.HasFlag(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP) => MOUSE_EVENT_FLAGS.MOUSEEVENTF_XUP.ToInput(Constants.XBUTTON1),
        VIRTUAL_KEY.VK_XBUTTON1 when !flags.HasFlag(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP) => MOUSE_EVENT_FLAGS.MOUSEEVENTF_XDOWN.ToInput(Constants.XBUTTON1),
        VIRTUAL_KEY.VK_XBUTTON2 when flags.HasFlag(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP) => MOUSE_EVENT_FLAGS.MOUSEEVENTF_XUP.ToInput(Constants.XBUTTON2),
        VIRTUAL_KEY.VK_XBUTTON2 when !flags.HasFlag(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP) => MOUSE_EVENT_FLAGS.MOUSEEVENTF_XDOWN.ToInput(Constants.XBUTTON2),

        _ => GetKeyboardInput(virtualKey, flags, extraInfo)
    };

    /// <summary>
    ///     Converts a virtual key to a keyboard input
    /// </summary>
    /// <param name="virtualKey">The virtual key that will be converted into an Input structure</param>
    /// <param name="flags">Specifies various aspects of a keystroke</param>
    /// <param name="extraInfo">An additional value associated with the keystroke</param>
    /// <returns>A keyboard Input structure that represents the virtual key</returns>
    private static INPUT GetKeyboardInput(VIRTUAL_KEY virtualKey, KEYBD_EVENT_FLAGS flags, nuint extraInfo) =>
        new()
        {
            type = INPUT_TYPE.INPUT_KEYBOARD,
            Anonymous = new()
            {
                ki = new()
                {
                    wVk = virtualKey,
                    wScan = (ushort)MapVirtualKey((uint)virtualKey, MAP_VIRTUAL_KEY_TYPE.MAPVK_VK_TO_VSC),
                    dwFlags = flags,
                    dwExtraInfo = extraInfo
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
    public static IEnumerable<INPUT> ToInputsPressKey(this VIRTUAL_KEY virtualKey, KEYBD_EVENT_FLAGS flags = 0,
        nuint extraInfo = Constants.KEY_IGNORE)
    {
        foreach (var extraFlag in new[] { (KEYBD_EVENT_FLAGS)0, KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP })
            yield return virtualKey.ToInput(flags | extraFlag, extraInfo);
    }

    /// <summary>
    ///     Converts a virtual key code to the corresponding Unicode character,
    ///     taking into account the current keyboard layout.If the virtual key code does
    ///     not have a corresponding character in the current layout, the method returns the null character('\0').
    /// </summary>
    /// <param name = "vkCode" > Virtual key code</param>
    /// <param name = "isInvariantCulture" >
    /// If parameter is set to false, the method will not take into
    ///     account the current language keyboard layout
    /// </param>
    /// <returns>Unicode character</returns>
    public static char ToUnicode(this VIRTUAL_KEY vkCode, bool isInvariantCulture = false)
    {
        var bKeyState = new byte[256];
        GetKeyState((int)VIRTUAL_KEY.VK_SHIFT);
        GetKeyState((int)VIRTUAL_KEY.VK_MENU);
        var bKeyStateStatus = GetKeyboardState(bKeyState);
        if (!bKeyStateStatus)
            return '\0';

        var hkl = new HKL(nint.Zero);
        var lScanCode = MapVirtualKey((uint)vkCode, MAP_VIRTUAL_KEY_TYPE.MAPVK_VK_TO_VSC);
        unsafe
        {
            if (!isInvariantCulture)
            {
                var focusedHWnd = GetForegroundWindow();
                var activeThread = GetWindowThreadProcessId(focusedHWnd);
                hkl = GetKeyboardLayout(activeThread);
            }


            char[] buff = new char[5];
            fixed (char* pwszBuff = buff)
            fixed (byte* pBkeystate = bKeyState)
            {
                if (ToUnicodeEx((uint)vkCode, lScanCode, pBkeystate, pwszBuff, 5, 0, hkl) == 0)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    if (errorCode != 0)
                    {
                        throw new Win32Exception(errorCode);
                    }
                    return '\0';
                }
                return new string(pwszBuff)[0];
            }
        }
    }
}