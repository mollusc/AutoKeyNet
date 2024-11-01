using System.Diagnostics;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using AutoKeyNet.WindowsHooks.Helper;
using AutoKeyNet.WindowsHooks.Hooks;
using AutoKeyNet.WindowsHooks.Hooks.EventArgs;
using AutoKeyNet.WindowsHooks.Rule;
using Microsoft.VisualStudio.Services.Common;
using static Windows.Win32.PInvoke;

namespace AutoKeyNet.WindowsHooks.Facades;

/// <summary>
///     Class for handling hotkeys
/// </summary>
internal class HotKeyHandler : BaseKeyHandler, IDisposable
{
    /// <summary>
    /// Buffer for inputs from keyboard or mouse
    /// </summary>
    private readonly Buffer<INPUT> _buffer;

    /// <summary>
    ///     Keyboard hook
    /// </summary>
    private readonly KeyboardHook _keyboardHook;

    /// <summary>
    ///     Mouse hook
    /// </summary>
    private readonly MouseHook _mouseHook;

    /// <summary>
    ///     A list of pressed keys
    /// </summary>
    private readonly List<INPUT> _pressedKeys = new();

    /// <summary>
    ///     A list of prefix keys from rules used to suppress key behavior.
    /// </summary>
    private readonly List<List<INPUT>> _suppressedKeys = new();

    /// <summary>
    ///     Constructor of the class for handling hotkeys
    /// </summary>
    /// <param name="rules">List of rules</param>
    /// <param name="kbdHook">Keyboard hook</param>
    /// <param name="mouseHook">Mouse hook</param>
    public HotKeyHandler(IEnumerable<BaseRuleRecord> rules, KeyboardHook kbdHook, MouseHook mouseHook) : base(rules)
    {
        var bufferSize = Rules.Max(r => r.KeyInputs.Length);
        _buffer = new Buffer<INPUT>(bufferSize);

        InitializeListOfSuppressedKeys();

        _mouseHook = mouseHook;
        _mouseHook.HookEvent += OnHookEvent;
        _keyboardHook = kbdHook;
        _keyboardHook.HookEvent += OnHookEvent;
    }

    /// <summary>
    ///     Method for disposing of hooks
    /// </summary>
    public void Dispose()
    {
        _mouseHook.HookEvent -= OnHookEvent;
        _keyboardHook.HookEvent -= OnHookEvent;
    }

    /// <summary>
    ///     Initialize a list of prefix keys.
    /// </summary>
    private void InitializeListOfSuppressedKeys()
    {
        foreach (var rule in Rules)
            if (rule is HotKeyRuleRecord)
            {
                for (var i = 0; i < rule.KeyInputs.Length; i++)
                {
                    if (rule.KeyInputs[i].Anonymous.ki.dwExtraInfo != Constants.KEY_SUPPRESS_NATIVE_BEHAVIOUR)
                        continue;

                    var inputs = new List<INPUT>();
                    for (var j = 0; j <= i; j++)
                        inputs.Add(rule.KeyInputs[j]);

                    if (_suppressedKeys.All(l => !l.SequenceEqual(inputs)) && inputs.Any())
                        _suppressedKeys.Add(inputs);
                }
            }
    }

    /// <summary>
    ///     Method for handling keyboard and mouse events
    /// </summary>
    /// <param name="sender">Sender of the event</param>
    /// <param name="e">Event arguments</param>
    private void OnHookEvent(object? sender, HookEventArgs e)
    {
        e.Cancel = FilterInput(e.Input) 
                   && ProcessKey(e.Input, e.WindowTitle, e.WindowClass, e.WindowModule, e.WindowControl);
    }


    private bool ProcessKey(INPUT input, string? eWindowTitle, string? eWindowClass, string? eWindowModule,
        string? eWindowControl)
    {
        var cancelNativeBehavior = true;
        _buffer.Add(input);
        Debug.WriteLine("info: " + string.Join(" ", _buffer.Where(FilterInput).Select(b => $"[{b}]")));
        var firedRules = CheckRules(eWindowTitle, eWindowClass, eWindowModule, eWindowControl).ToArray();
        var isSuppressedKeys = _suppressedKeys.Any(inputs =>
            _buffer.TakeLast(inputs.Count).SequenceEqual(inputs, new InputComparerByVKeyAndFlag()));
        if (isSuppressedKeys)
        {
            _pressedKeys.Add(input);
            cancelNativeBehavior = true;
        }
        else if (_pressedKeys.Any())
        {
            SendInput(_pressedKeys.ToArray().AsSpan(), _pressedKeys.Count);
            _pressedKeys.Clear();
            cancelNativeBehavior = true;
        }
        else
            cancelNativeBehavior = false;

        if (firedRules.Any())
        {
            firedRules.ForEach(r => r.Run.Invoke());
            Debug.WriteLine("rule: " + string.Join(";\t", firedRules.Select(r => r.KeyText)));
            _pressedKeys.Clear();
        }

        return cancelNativeBehavior;
    }

    /// <summary>
    ///     Filter inputs
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    private bool FilterInput(INPUT i)
    {
        if (i.type == INPUT_TYPE.INPUT_KEYBOARD)
            return true;
        if (i.type == INPUT_TYPE.INPUT_MOUSE)
        {
            var eventsToDisplay = new HashSet<MOUSE_EVENT_FLAGS>
            {
                MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTDOWN,
                MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTUP,
                MOUSE_EVENT_FLAGS.MOUSEEVENTF_RIGHTDOWN,
                MOUSE_EVENT_FLAGS.MOUSEEVENTF_RIGHTUP,
                MOUSE_EVENT_FLAGS.MOUSEEVENTF_MIDDLEDOWN,
                MOUSE_EVENT_FLAGS.MOUSEEVENTF_MIDDLEUP,
                MOUSE_EVENT_FLAGS.MOUSEEVENTF_XDOWN,
                MOUSE_EVENT_FLAGS.MOUSEEVENTF_XUP
            };
            return eventsToDisplay.Contains(i.Anonymous.mi.dwFlags);
        }

        return false;
    }


    private IEnumerable<HotKeyRuleRecord> CheckRules(string? windowTitle, string? windowClass, string? windowModule,
        string? windowControl)
    {
        if (_buffer.Count > 0)
            foreach (var rule in Rules)
                if (rule is HotKeyRuleRecord hotKeyRuleRecord
                    && _buffer.TakeLast(rule.KeyInputs.Length)
                        .SequenceEqual(rule.KeyInputs, new InputComparerByVKeyAndFlag())
                    && (rule.CheckWindowCondition?.Invoke(windowTitle, windowClass, windowModule, windowControl) ??
                        true))
                {
                    yield return hotKeyRuleRecord;
                }
    }
}