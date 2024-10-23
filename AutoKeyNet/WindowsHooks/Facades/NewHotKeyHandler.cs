using System.Diagnostics;
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
internal class NewHotKeyHandler : BaseKeyHandler, IDisposable
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
    private readonly HashSet<Input> _buffer = new(new InputComparerFlagIgnore());


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
    public NewHotKeyHandler(IEnumerable<BaseRuleRecord> rules, KeyboardHook kbdHook, MouseHook mouseHook) : base(rules)
    {
        //_mouseHook = mouseHook;
        //_mouseHook.OnHookEvent += OnMouseHookEvent;
        _keyboardHook = kbdHook;
        _keyboardHook.OnHookEvent += OnKeyboardHookEvent;
        InitializePrefixKeys();
    }

    /// <summary>
    ///     Method for disposing of hooks
    /// </summary>
    public void Dispose()
    {
        //_mouseHook.OnHookEvent -= OnMouseHookEvent;
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
    ///     Method for handling keyboard events
    /// </summary>
    /// <param name="sender">Sender of the event</param>
    /// <param name="e">Event arguments</param>
    private void OnKeyboardHookEvent(object? sender, KeyboardHookEventArgs e)
    {
        var kbd = (KeyboardLowLevelHook)(Marshal.PtrToStructure(e.LParam, typeof(KeyboardLowLevelHook)) ??
                                         throw new InvalidOperationException());
        var keyFlag = e.WParam == (nint)KeyboardMessage.WM_KEYDOWN ? KeyEventFlags.KEYDOWN : KeyEventFlags.KEYUP;
        var input = kbd.VirtualKey.ToInput(keyFlag);
        e.Cancel = ProcessKey(input, e.WindowTitle, e.WindowClass, e.WindowModule, e.WindowControl);
    }

    private bool ProcessKey(Input input, string? eWindowTitle, string? eWindowClass, string? eWindowModule, string? eWindowControl)
    {
        if (input.Data.KeyboardInput.Flags == KeyEventFlags.KEYUP)
            _buffer.Remove(input);
        _buffer.Add(input);
        Debug.WriteLine(_buffer.Count);
        bool result = CheckRules(eWindowTitle, eWindowClass, eWindowModule, eWindowControl);
        if (input.Data.KeyboardInput.Flags == KeyEventFlags.KEYUP)
            _buffer.Remove(input);
        return result;
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
        var testBuffer = new HashSet<Input>(_buffer, new InputComparerByVKeyAndFlag());
        var result = false;
        if (testBuffer.Count > 0)
            foreach (var rule in Rules)
                if (rule is HotKeyRuleRecord
                    && testBuffer.SetEquals(rule.KeyInputs)
                    && (rule.CheckWindowCondition?.Invoke(windowTitle, windowClass, windowModule, windowControl) ??
                        true))
                {
                    rule.Run.Invoke();
                    result = true;
                }

        return result;
    }

}

internal class InputComparerByVKeyAndFlag : IEqualityComparer<Input>
{
    public bool Equals(Input x, Input y)
    {
        return x.Type == y.Type && x.Data.KeyboardInput.VirtualKey == y.Data.KeyboardInput.VirtualKey && x.Data.KeyboardInput.Flags == y.Data.KeyboardInput.Flags;
    }

    public int GetHashCode(Input obj)
    {
        return HashCode.Combine((int)obj.Type, obj.Data.KeyboardInput.VirtualKey, obj.Data.KeyboardInput.Flags);
    }
}

internal class InputComparerFlagIgnore : IEqualityComparer<Input>
{
    public bool Equals(Input x, Input y)
    {
        return x.Type == y.Type && x.Data.KeyboardInput.VirtualKey == y.Data.KeyboardInput.VirtualKey;
    }

    public int GetHashCode(Input obj)
    {
        return HashCode.Combine((int)obj.Type, obj.Data.KeyboardInput.VirtualKey);
    }
}