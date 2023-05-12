using System.Collections;
using System.Diagnostics;
using AutoKeyNet.WindowsHooks.WindowsEnums;
using AutoKeyNet.WindowsHooks.WindowsStruct;
using System.Runtime.InteropServices;
using AutoKeyNet.WindowsHooks.Helper;
using AutoKeyNet.WindowsHooks.Rule;
using AutoKeyNet.WindowsHooks.Hooks;
using AutoKeyNet.WindowsHooks.Hooks.EventArgs;
using AutoKeyNet.WindowsHooks.WinApi;

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

    private readonly HashSet<ushort> _prefixKeysToSuppressKeyBehaviorInHotKey = new();
    //private bool _prefixKeyDown = false;

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
        Preprocessing();
    }

    private void Preprocessing()
    {
        foreach (BaseRuleRecord rule in Rules)
        {
            if (rule is HotKeyRuleRecord hotKeyRule
                && hotKeyRule.Options.HasFlag(HotKeyRuleRecordOptionFlags.DelayKeyDownToKyeUpForPrefixKey))
            {
                if (InputToVirtualKey(hotKeyRule).FirstOrDefault() is var virtualKey)
                    _prefixKeysToSuppressKeyBehaviorInHotKey.Add(virtualKey);
            }
        }
    }

    private static IEnumerable<ushort> InputToVirtualKey(BaseRuleRecord rule)
    {
        return rule.InputKeys.Select(x => x.U.ki.wVk != 0 ? x.U.ki.wVk : (ushort)MouseInputToVirtualKey(x.U.mi));
    }

    /// <summary>
    /// Method for handling mouse events
    /// </summary>
    /// <param name="sender">Sender of the event</param>
    /// <param name="e">Event arguments</param>
    private void OnMouseHookEvent(object? sender, MouseHookEventArgs e)
    {
        if (_activateMouseKeyEvent.TryGetValue(((MouseMessage)e.WParam, e.MouseData), out var vkDown))
            e.Cancel = ProcessKeyDown((ushort)vkDown, e.WindowTitle, e.WindowClass, e.WindowModule, e.WindowControl);

        if (_deactivateMouseKeyEvent.TryGetValue(((MouseMessage)e.WParam, e.MouseData), out var vkUp))
            e.Cancel = ProcessKeyUp((ushort)vkUp);
    }

    /// <summary>
    /// Method for handling keyboard events
    /// </summary>
    /// <param name="sender">Sender of the event</param>
    /// <param name="e">Event arguments</param>
    private void OnKeyboardHookEvent(object? sender, KeyboardHookEventArgs e)
    {
        KeyboardLowLevelHook kbd = (KeyboardLowLevelHook)(Marshal.PtrToStructure(e.LParam, typeof(KeyboardLowLevelHook)) ?? throw new InvalidOperationException());
        VirtualKey vk = kbd.vkCode;

        if (e.WParam == (nint)KeyboardMessage.WM_KEYDOWN)
            e.Cancel = ProcessKeyDown((ushort)vk, e.WindowTitle, e.WindowClass, e.WindowModule, e.WindowControl);

        if (e.WParam == (nint)KeyboardMessage.WM_KEYUP)
            e.Cancel = ProcessKeyUp((ushort)vk);

    }


    //private bool _ruleTriggered = false;
    private HashSet<ushort> _pressedKeys = new();
    private bool ProcessKeyDown(ushort vk, string? windowTitle, string? windowClass, string? windowModule, string? windowControl)
    {
        _pressedKeys.Clear();

        _buffer.Add(vk);
        Debug.WriteLine($"HotKey {(Keys)vk} --> {string.Join(',', _buffer.Select(k => (Keys)k))}");
        if (_buffer.Count == 1 && _prefixKeysToSuppressKeyBehaviorInHotKey.Contains(vk))
        {
            Debug.WriteLine($"      DEBUG: Suppress key down: {(Keys)vk}");
            return true;
        }

        if (CheckRules(windowTitle, windowClass, windowModule, windowControl))
        {
            Debug.WriteLine($"      DEBUG: Suppress by check rules key down: {(Keys)vk}");
            _pressedKeys = new HashSet<ushort>(_buffer);
            return true;
        }
        return false;
    }

    private bool ProcessKeyUp(ushort vk)
    {
        Debug.WriteLine($"      DEBUG: Remove key '{(Keys)vk}' from buffer '{string.Join(',', _buffer.Select(k => (Keys)k))}'");
        _buffer.Remove(vk);

        if (_pressedKeys.Count > 0)
        {
            Debug.WriteLine($"      DEBUG: Suppress key '{(Keys)vk}' and remove from _pressedKeys: '{string.Join(',', _pressedKeys.Select(k => (Keys)k))}'");

            _pressedKeys.Remove(vk);
            return true;
        }

        if (_buffer.Count == 0 && _prefixKeysToSuppressKeyBehaviorInHotKey.Contains(vk))
        {
            Debug.WriteLine($"      DEBUG: Regenerate press key: {(Keys)vk}");

            VirtualKey virtualKey = (VirtualKey)vk;
            var inputs = virtualKey.ToInputsPressKey().ToArray();
            NativeMethods.SendInputAsync(inputs).ConfigureAwait(false);
            return true;
        }
        Debug.WriteLine($"      DEBUG: Clear buffer:'{string.Join(',', _buffer.Select(k => (Keys)k))}' ");
        _buffer.Clear();
        return false;
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
                    && _buffer.SetEquals(rule.InputKeys.Select(x => x.U.ki.wVk != 0 ? x.U.ki.wVk : (ushort)MouseInputToVirtualKey(x.U.mi)))
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