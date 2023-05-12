using System.Diagnostics;
using AutoKeyNet.WindowsHooks.WindowsEnums;
using AutoKeyNet.WindowsHooks.WindowsStruct;
using System.Runtime.InteropServices;
using AutoKeyNet.WindowsHooks.Rule;
using AutoKeyNet.WindowsHooks.Hooks;
using AutoKeyNet.WindowsHooks.Hooks.EventArgs;

namespace AutoKeyNet.WindowsHooks.Facades;

/// <summary>
/// Class for handling hotkeys
/// </summary>
internal class HotKeyHandler : BaseKeyHandler, IDisposable
{
    /// <summary>
    /// Dictionary of Windows mouse events and virtual keys, used for adding virtual mouse keys to the buffer.
    /// </summary>
    private readonly Dictionary<(MouseMessage, int), VirtualKey> _activateMouseKeyEvent = new()
    {
        { (MouseMessage.WM_LBUTTONDOWN, 0), VirtualKey.LBUTTON },
        { (MouseMessage.WM_RBUTTONDOWN, 0), VirtualKey.RBUTTON },
        { (MouseMessage.WM_MBUTTONDOWN, 0), VirtualKey.MBUTTON },
        { (MouseMessage.WM_XBUTTONDOWN, Constants.XBUTTON1), VirtualKey.XBUTTON1 },
        { (MouseMessage.WM_XBUTTONDOWN, Constants.XBUTTON2), VirtualKey.XBUTTON2 },
    };
    /// <summary>
    /// Dictionary of Windows mouse events and virtual keys, used for removing virtual mouse keys to the buffer.
    /// </summary>
    private readonly Dictionary<(MouseMessage, int), VirtualKey> _deactivateMouseKeyEvent = new()
    {
        { (MouseMessage.WM_LBUTTONUP, 0), VirtualKey.LBUTTON },
        { (MouseMessage.WM_RBUTTONUP, 0), VirtualKey.RBUTTON },
        { (MouseMessage.WM_MBUTTONUP, 0), VirtualKey.MBUTTON },
        { (MouseMessage.WM_XBUTTONUP, Constants.XBUTTON1), VirtualKey.XBUTTON1 },
        { (MouseMessage.WM_XBUTTONUP, Constants.XBUTTON2), VirtualKey.XBUTTON2 },
    };

    /// <summary>
    /// Buffer for pressed keys
    /// </summary>
    private readonly HashSet<ushort> _buffer = new();

    private readonly MouseHook _mouseHook;
    private readonly KeyboardHook _keyboardHook;

    /// <summary>
    /// Constructor of the class for handling hotkeys
    /// </summary>
    /// <param name="rules">List of rules</param>
    /// <param name="kbdHook">Keyboard hook</param>
    /// <param name="mouseHook">Mouse hook</param>
    public HotKeyHandler(IEnumerable<BaseRuleRecord> rules, KeyboardHook kbdHook, MouseHook mouseHook) : base(rules)
    {
        _mouseHook = mouseHook;
        _mouseHook.OnHookEvent += OnMouseHookEvent;
        _keyboardHook = kbdHook;
        _keyboardHook.OnHookEvent += OnKeyboardHookEvent;
    }

    /// <summary>
    /// Method for handling mouse events
    /// </summary>
    /// <param name="sender">Sender of the event</param>
    /// <param name="e">Event arguments</param>
    private void OnMouseHookEvent(object? sender, MouseHookEventArgs e)
    {
        if (_activateMouseKeyEvent.TryGetValue(((MouseMessage)e.WParam, e.MouseData), out var aKey))
            _buffer.Add((ushort)aKey);
        if (_deactivateMouseKeyEvent.TryGetValue(((MouseMessage)e.WParam, e.MouseData), out var dKey))
            _buffer.Remove((ushort)dKey);

        CheckRules(e.WindowTitle, e.WindowTitle, e.WindowModule, e.WindowControl);
    }

    /// <summary>
    /// Method for handling keyboard events
    /// </summary>
    /// <param name="sender">Sender of the event</param>
    /// <param name="e">Event arguments</param>
    private void OnKeyboardHookEvent(object? sender, KeyboardHookEventArgs e)
    {
        KeyboardLowLevelHook kbd = (KeyboardLowLevelHook)(Marshal.PtrToStructure(e.LParam, typeof(KeyboardLowLevelHook)) ??
                                                throw new InvalidOperationException());
        if (e.WParam == (nint)KeyboardMessage.WM_KEYDOWN)
        {
            _buffer.Add((ushort)kbd.vkCode);
            Debug.WriteLine($"HotKey {(Keys)kbd.vkCode} --> {string.Join(',', _buffer.Select(k => (Keys)k))}");
            if (CheckRules(e.WindowTitle, e.WindowClass, e.WindowModule, e.WindowControl))
                e.Cancel = true;
        }

        if (e.WParam == (nint)KeyboardMessage.WM_KEYUP)
        {
            _buffer.Remove((ushort)kbd.vkCode);
        }

        // Panic Button
        if (kbd.vkCode == VirtualKey.ESCAPE)
            _buffer.Clear();
    }

    /// <summary>
    /// Method for checking rules
    /// </summary>
    /// <param name="windowTitle">Title of the foreground window for filtering rules. If the variable is null, the filter is not applied.</param>
    /// <param name="windowClass">Class of the foreground window for filtering rules. If the variable is null, the filter is not applied.</param>
    /// <param name="windowModule">Module name (file *.exe) of the foreground window for filtering rules. If the variable is null, the filter is not applied.</param>
    /// <param name="windowControl">Name of the focused control for filtering rules. If the variable is null, the filter is not applied.</param>
    /// <returns>True if a rule was triggered; otherwise, false.</returns>
    private bool CheckRules(string? windowTitle, string? windowClass, string? windowModule, string? windowControl)
    {
        bool result = false;
        if (_buffer.Count > 0)
        {
            foreach (BaseRuleRecord rule in Rules)
            {
                if (rule is HotKeyRuleRecord
                    && _buffer.SetEquals(rule.InputKeys.Select(x => x.U.ki.wVk != 0 ? x.U.ki.wVk: (ushort)MouseInputToVirtualKey(x.U.mi)))
                    && (rule.CheckWindowCondition?.Invoke(windowTitle, windowClass, windowModule, windowControl) ??
                        true))
                {
                    rule.Run.Invoke();
                    result = true;
                }
            }
        }

        return result;
    }

    private static VirtualKey MouseInputToVirtualKey(MouseInput uMi) =>
        uMi.dwFlags switch
        {
            MouseEvents.LEFTDOWN => VirtualKey.LBUTTON,
            MouseEvents.RIGHTDOWN => VirtualKey.RBUTTON,
            MouseEvents.MIDDLEDOWN => VirtualKey.MBUTTON,
            MouseEvents.XDOWN when uMi.mouseData == Constants.XBUTTON1 => VirtualKey.XBUTTON1,
            MouseEvents.XDOWN when uMi.mouseData == Constants.XBUTTON2 => VirtualKey.XBUTTON2,
            _ => 0
        };

    /// <summary>
    /// Method for disposing of hooks
    /// </summary>
    public void Dispose()
    {
        _mouseHook.OnHookEvent -= OnMouseHookEvent;
        _keyboardHook.OnHookEvent -= OnKeyboardHookEvent;
    }
}