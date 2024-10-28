using System.Diagnostics;
using System.Runtime.InteropServices;
using AutoKeyNet.WindowsHooks.Helper;
using AutoKeyNet.WindowsHooks.Hooks;
using AutoKeyNet.WindowsHooks.Hooks.EventArgs;
using AutoKeyNet.WindowsHooks.Rule;
using AutoKeyNet.WindowsHooks.WindowsEnums;
using AutoKeyNet.WindowsHooks.WindowsStruct;
using Microsoft.VisualStudio.Services.Common;
using static AutoKeyNet.WindowsHooks.WinApi.NativeMethods;

namespace AutoKeyNet.WindowsHooks.Facades;

/// <summary>
///     Class for handling hotkeys
/// </summary>
internal class NewHotKeyHandler : BaseKeyHandler, IDisposable
{
    private const int BufferSize = 1000;
    private readonly Dictionary<(MouseMessage, uint), VirtualKey> _activateMouseKeyEvent = new()
    {
        { (MouseMessage.WM_LBUTTONDOWN, 0), VirtualKey.LBUTTON },
        { (MouseMessage.WM_RBUTTONDOWN, 0), VirtualKey.RBUTTON },
        { (MouseMessage.WM_MBUTTONDOWN, 0), VirtualKey.MBUTTON },
        { (MouseMessage.WM_XBUTTONDOWN, XBUTTON1), VirtualKey.XBUTTON1 },
        { (MouseMessage.WM_XBUTTONDOWN, XBUTTON2), VirtualKey.XBUTTON2 }
    };

    private readonly Buffer<Input> _buffer = new(BufferSize);


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
    private readonly List<List<Input>> _supressedKeys = new();

    /// <summary>
    ///     A list of pressed keys
    /// </summary>
    private readonly List<Input> _pressedKeys = new();

    /// <summary>
    ///     Constructor of the class for handling hotkeys
    /// </summary>
    /// <param name="rules">List of rules</param>
    /// <param name="kbdHook">Keyboard hook</param>
    /// <param name="mouseHook">Mouse hook</param>
    public NewHotKeyHandler(IEnumerable<BaseRuleRecord> rules, KeyboardHook kbdHook, MouseHook mouseHook) : base(rules)
    {
        InitializePrefixKeys();
        _mouseHook = mouseHook;
        _mouseHook.OnHookEvent += OnMouseHookEvent;
        _keyboardHook = kbdHook;
        _keyboardHook.OnHookEvent += OnKeyboardHookEvent;
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
                && hotKeyRule.Options.HasFlag(HotKeyRuleRecordOptionFlags.SuppressNativeBehavior))
            {
                for (int i = 0; i < rule.KeyInputs.Length; i++)
                {
                    List<Input> inputs = new List<Input>();
                    for (int j = 0; j < i; j++)
                    {
                        inputs.Add(rule.KeyInputs[j]);
                    }
                    if (_supressedKeys.All(l => !l.SequenceEqual(inputs)) && inputs.Any())
                        _supressedKeys.Add(inputs);
                }
            }
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
    private void OnMouseHookEvent(object? sender, MouseHookEventArgs e)
    {
        Input input = new Input
        {
            Type = InputType.INPUT_KEYBOARD
        };
        if (_activateMouseKeyEvent.TryGetValue(((MouseMessage)e.WParam, (uint)e.MouseData), out var vkDown))
        {
            input.Data.KeyboardInput.VirtualKey = (ushort)vkDown;
            input.Data.KeyboardInput.Flags = KeyEventFlags.KEYDOWN;
            e.Cancel = ProcessKey(input, e.WindowTitle, e.WindowClass, e.WindowModule, e.WindowControl);
        }

        if (_deactivateMouseKeyEvent.TryGetValue(((MouseMessage)e.WParam, (uint)e.MouseData), out var vkUp))
        {
            input.Data.KeyboardInput.VirtualKey = (ushort)vkUp;
            input.Data.KeyboardInput.Flags = KeyEventFlags.KEYUP;
            e.Cancel = ProcessKey(input, e.WindowTitle, e.WindowClass, e.WindowModule, e.WindowControl);
        }
    }

    private bool ProcessKey(Input input, string? eWindowTitle, string? eWindowClass, string? eWindowModule, string? eWindowControl)
    {
        bool cancelNativeBehavior = true;
        _buffer.Add(input);
        Debug.WriteLine(string.Join(" ", _buffer.TakeLast(5).Select(b => $"[{b}]")));
        var firedRules = CheckRules(eWindowTitle, eWindowClass, eWindowModule, eWindowControl).ToArray();
        bool isSuppressedKeys = _supressedKeys.Any(inputs => _buffer.TakeLast(inputs.Count).SequenceEqual(inputs, new InputComparerByVKeyAndFlag()));
        if (isSuppressedKeys)
        {
            _pressedKeys.Add(input);
            cancelNativeBehavior = true;
        }
        else if (_pressedKeys.Any())
        {
            SendInputAsync(_pressedKeys.ToArray()).ConfigureAwait(false);
            _pressedKeys.Clear();
            cancelNativeBehavior = true;
        }
        else
            cancelNativeBehavior = false;

        if (firedRules.Any())
        {
            //if (!firedRules.Any(r => r.Options.HasFlag(HotKeyRuleRecordOptionFlags.SuppressNativeBehavior)))
            //{
            //    SendInputAsync(_pressedKeys.ToArray()).ConfigureAwait(false);
            //    cancelNativeBehavior = true;
            //}
            firedRules.ForEach(r => r.Run.Invoke());
            Debug.WriteLine("Rule Proceed");

            _pressedKeys.Clear();

        }
        return cancelNativeBehavior;
    }


    private IEnumerable<HotKeyRuleRecord> CheckRules(string? windowTitle, string? windowClass, string? windowModule, string? windowControl)
    {
        if (_buffer.Count > 0)
            foreach (var rule in Rules)
                if (rule is HotKeyRuleRecord hotKeyRuleRecord
                    && _buffer.TakeLast(rule.KeyInputs.Length).SequenceEqual(rule.KeyInputs, new InputComparerByVKeyAndFlag())
                    && (rule.CheckWindowCondition?.Invoke(windowTitle, windowClass, windowModule, windowControl) ??
                        true))
                {
                    yield return hotKeyRuleRecord;
                }
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