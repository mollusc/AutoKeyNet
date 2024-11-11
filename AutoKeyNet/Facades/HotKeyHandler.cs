using System.Diagnostics;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using AutoKeyNet.WindowsHooks.Hooks;
using AutoKeyNet.WindowsHooks.Hooks.EventArgs;
using Microsoft.VisualStudio.Services.Common;
using static Windows.Win32.PInvoke;
using System.Runtime.InteropServices;
using AutoKeyNet.Helper;
using AutoKeyNet.RuleRecords;

namespace AutoKeyNet.Facades;

/// <summary>
///     Class for handling hotkeys
/// </summary>
internal class HotKeyHandler : BaseKeyHandler, IDisposable
{
    /// <summary>
    /// Buffer for inputs from keyboard or mouse
    /// </summary>
    private readonly CircularBuffer<INPUT>? _buffer;

    /// <summary>
    ///     A list of pressed keys
    /// </summary>
    private readonly List<INPUT> _pressedKeys = [];

    /// <summary>
    ///     A list of prefix keys from rules used to suppress key behavior.
    /// </summary>
    private readonly List<List<INPUT>> _suppressedKeys = [];

    /// <summary>
    ///     Constructor of the class for handling hotkeys
    /// </summary>
    /// <param name="rules">List of rules</param>
    /// <param name="kbdHook">Keyboard hook</param>
    /// <param name="mouseHook">Mouse hook</param>
    /// <param name="winHook"></param>
    public HotKeyHandler(IEnumerable<HotKeyRuleRecord> rules, KeyboardHook kbdHook, MouseHook mouseHook, WinHook winHook) :
        base(rules, kbdHook, mouseHook, winHook)
    {
        if (Rules.Any())
        {
            var bufferSize = Rules.Max(r => r.KeyInputs?.Length ?? 0);
            _buffer = new CircularBuffer<INPUT>(bufferSize);
        }

        InitializeListOfSuppressedKeys();

    }


    /// <summary>
    ///     Initialize a list of prefix keys.
    /// </summary>
    private void InitializeListOfSuppressedKeys()
    {
        foreach (var rule in Rules)
            if (rule is HotKeyRuleRecord)
            {
                if (rule.KeyInputs != null)
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

    protected override void OnKeyboardHookEvent(object? sender, HookEventArgs e) => OnHookEvent(sender, e);

    protected override void OnMouseHookEvent(object? sender, HookEventArgs e) => OnHookEvent(sender, e);


    protected virtual bool ProcessKey(INPUT input, string? eWindowTitle, string? eWindowClass, string? eWindowModule,
        string? eWindowControl)
    {
        var cancelNativeBehavior = false;
        if (_buffer is not null)
        {
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
                SendInput(_pressedKeys.ToArray().AsSpan(), Marshal.SizeOf(typeof(INPUT)));
                _pressedKeys.Clear();
                cancelNativeBehavior = true;
            }
            else
                cancelNativeBehavior = false;

            if (firedRules.Any())
            {
                firedRules.ForEach(r => r.Run.Invoke());
                Debug.WriteLine("rule: " + string.Join(";\t", firedRules.Select(r => r.KeyChars)));
                _pressedKeys.Clear();
            }

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
        if (_buffer is { Count: > 0 })
            foreach (var rule in Rules)
                if (rule.KeyInputs != null
                    && rule is HotKeyRuleRecord hotKeyRuleRecord
                    && _buffer.TakeLast(rule.KeyInputs.Length)
                        .SequenceEqual(rule.KeyInputs, new InputComparerByVKeyAndFlag())
                    && (rule.CheckWindowCondition?.Invoke(windowTitle, windowClass, windowModule, windowControl) ??
                        true))
                {
                    yield return hotKeyRuleRecord;
                }
    }
}