using System.Runtime.InteropServices;
using AutoKeyNet.WindowsHooks.Helper;
using AutoKeyNet.WindowsHooks.Hooks;
using AutoKeyNet.WindowsHooks.Hooks.EventArgs;
using AutoKeyNet.WindowsHooks.Rule;
using AutoKeyNet.WindowsHooks.WindowsEnums;
using AutoKeyNet.WindowsHooks.WindowsStruct;
using static AutoKeyNet.WindowsHooks.WinApi.NativeMethods;

namespace AutoKeyNet.WindowsHooks.Facades;

/// <summary>
///     Class for handling hotkeys
/// </summary>
internal class HotKeyHandler : BaseKeyHandler, IDisposable
{
    /// <summary>
    ///     Dictionary of Windows mouse events and virtual keys, used for adding virtual mouse keys to the buffer.
    /// </summary>
    private readonly Dictionary<(MouseMessage, uint), VirtualKey> _activateMouseKeyEvent = new()
    {
        { (MouseMessage.WM_LBUTTONDOWN, 0), VirtualKey.LBUTTON },
        { (MouseMessage.WM_RBUTTONDOWN, 0), VirtualKey.RBUTTON },
        { (MouseMessage.WM_MBUTTONDOWN, 0), VirtualKey.MBUTTON },
        { (MouseMessage.WM_XBUTTONDOWN, XBUTTON1), VirtualKey.XBUTTON1 },
        { (MouseMessage.WM_XBUTTONDOWN, XBUTTON2), VirtualKey.XBUTTON2 }
    };

    /// <summary>
    ///     Buffer for pressed keys
    /// </summary>
    private readonly HashSet<ushort> _buffer = new();

    /// <summary>
    ///     Dictionary of Windows mouse events and virtual keys, used for removing virtual mouse keys to the buffer.
    /// </summary>
    private readonly Dictionary<(MouseMessage, uint), VirtualKey> _deactivateMouseKeyEvent = new()
    {
        { (MouseMessage.WM_LBUTTONUP, 0), VirtualKey.LBUTTON },
        { (MouseMessage.WM_RBUTTONUP, 0), VirtualKey.RBUTTON },
        { (MouseMessage.WM_MBUTTONUP, 0), VirtualKey.MBUTTON },
        { (MouseMessage.WM_XBUTTONUP, XBUTTON1), VirtualKey.XBUTTON1 },
        { (MouseMessage.WM_XBUTTONUP, XBUTTON2), VirtualKey.XBUTTON2 }
    };

    /// <summary>
    ///     Keyboard hook
    /// </summary>
    private readonly KeyboardHook _keyboardHook;

    /// <summary>
    ///     Mouse hook
    /// </summary>
    private readonly MouseHook _mouseHook;

    /// <summary>
    ///     A list of prefix keys from rules used to suppress key behavior.
    /// </summary>
    private readonly HashSet<ushort> _prefixKeys = new();

    /// <summary>
    ///     A list of pressed keys
    /// </summary>
    private HashSet<ushort> _pressedKeys = new();

    /// <summary>
    ///     Constructor of the class for handling hotkeys
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
        InitializePrefixKeys();
    }

    /// <summary>
    ///     Method for disposing of hooks
    /// </summary>
    public void Dispose()
    {
        _mouseHook.OnHookEvent -= OnMouseHookEvent;
        _keyboardHook.OnHookEvent -= OnKeyboardHookEvent;
    }

    /// <summary>
    ///     Initialize a list of prefix keys.
    /// </summary>
    private void InitializePrefixKeys()
    {
        foreach (var rule in Rules)
            if (rule is HotKeyRuleRecord hotKeyRule
                && hotKeyRule.Options.HasFlag(HotKeyRuleRecordOptionFlags.SuppressNativeBehaviorForPrefixKey))
                if (hotKeyRule.KeyInputs.ToVirtualKeys().FirstOrDefault() is var virtualKey)
                    _prefixKeys.Add((ushort)virtualKey);
    }

    /// <summary>
    ///     Method for handling mouse events
    /// </summary>
    /// <param name="sender">Sender of the event</param>
    /// <param name="e">Event arguments</param>
    private void OnMouseHookEvent(object? sender, MouseHookEventArgs e)
    {
        if (_activateMouseKeyEvent.TryGetValue(((MouseMessage)e.WParam, (uint)e.MouseData), out var vkDown))
            e.Cancel = ProcessKeyDown((ushort)vkDown, e.WindowTitle, e.WindowClass, e.WindowModule, e.WindowControl);

        if (_deactivateMouseKeyEvent.TryGetValue(((MouseMessage)e.WParam, (uint)e.MouseData), out var vkUp))
            e.Cancel = ProcessKeyUp((ushort)vkUp);
    }

    /// <summary>
    ///     Method for handling keyboard events
    /// </summary>
    /// <param name="sender">Sender of the event</param>
    /// <param name="e">Event arguments</param>
    private void OnKeyboardHookEvent(object? sender, KeyboardHookEventArgs e)
    {
        var kbd = (KeyboardLowLevelHook)(Marshal.PtrToStructure(e.LParam, typeof(KeyboardLowLevelHook)) ??
                                         throw new InvalidOperationException());
        var vk = kbd.VirtualKey;

        if (e.WParam == (nint)KeyboardMessage.WM_KEYDOWN)
            e.Cancel = ProcessKeyDown((ushort)vk, e.WindowTitle, e.WindowClass, e.WindowModule, e.WindowControl);

        if (e.WParam == (nint)KeyboardMessage.WM_KEYUP)
            e.Cancel = ProcessKeyUp((ushort)vk);
    }


    /// <summary>
    ///     Method that processes a key down event.
    /// </summary>
    /// <param name="vk">A virtual key</param>
    /// <param name="windowTitle">
    ///     Title of the foreground window for filtering rules. If the variable is null, the filter is
    ///     not applied.
    /// </param>
    /// <param name="windowClass">
    ///     Class of the foreground window for filtering rules. If the variable is null, the filter is
    ///     not applied.
    /// </param>
    /// <param name="windowModule">
    ///     Module name (file *.exe) of the foreground window for filtering rules. If the variable is
    ///     null, the filter is not applied.
    /// </param>
    /// <param name="windowControl">
    ///     Name of the focused control for filtering rules. If the variable is null, the filter is not
    ///     applied.
    /// </param>
    /// <returns>A boolean value indicating whether the key event should not be sent to the system (true) or not (false).</returns>
    private bool ProcessKeyDown(ushort vk, string? windowTitle, string? windowClass, string? windowModule,
        string? windowControl)
    {
        _pressedKeys.Clear();

        _buffer.Add(vk);
        if (_buffer.Count == 1 && _prefixKeys.Contains(vk))
            return true;

        if (CheckRules(windowTitle, windowClass, windowModule, windowControl))
        {
            _pressedKeys = new HashSet<ushort>(_buffer);
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Method that processes a key up event.
    /// </summary>
    /// <param name="vk">A virtual key</param>
    /// <returns>A boolean value indicating whether the key event should not be sent to the system (true) or not (false).</returns>
    private bool ProcessKeyUp(ushort vk)
    {
        _buffer.Remove(vk);

        if (_pressedKeys.Count > 0)
        {
            _pressedKeys.Remove(vk);
            return true;
        }

        if (_buffer.Count == 0 && _prefixKeys.Contains(vk))
        {
            var virtualKey = (VirtualKey)vk;
            var inputs = virtualKey.ToInputsPressKey().ToArray();
            SendInputAsync(inputs).ConfigureAwait(false);
            return true;
        }

        _buffer.Clear();
        return false;
    }

    /// <summary>
    ///     Method for checking rules
    /// </summary>
    /// <param name="windowTitle">
    ///     Title of the foreground window for filtering rules. If the variable is null, the filter is
    ///     not applied.
    /// </param>
    /// <param name="windowClass">
    ///     Class of the foreground window for filtering rules. If the variable is null, the filter is
    ///     not applied.
    /// </param>
    /// <param name="windowModule">
    ///     Module name (file *.exe) of the foreground window for filtering rules. If the variable is
    ///     null, the filter is not applied.
    /// </param>
    /// <param name="windowControl">
    ///     Name of the focused control for filtering rules. If the variable is null, the filter is not
    ///     applied.
    /// </param>
    /// <returns>True if a rule was triggered; otherwise, false.</returns>
    private bool CheckRules(string? windowTitle, string? windowClass, string? windowModule, string? windowControl)
    {
        var result = false;
        if (_buffer.Count > 0)
            foreach (var rule in Rules)
                if (rule is HotKeyRuleRecord
                    && _buffer.SetEquals(rule.KeyInputs.ToVirtualKeys().Cast<ushort>())
                    && (rule.CheckWindowCondition?.Invoke(windowTitle, windowClass, windowModule, windowControl) ??
                        true))
                {
                    rule.Run.Invoke();
                    result = true;
                }

        return result;
    }
}