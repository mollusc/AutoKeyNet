using System.Diagnostics;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using AutoKeyNet.Helper;
using AutoKeyNet.RuleRecords;
using AutoKeyNet.WindowsHooks.Hooks;
using AutoKeyNet.WindowsHooks.Hooks.EventArgs;
using Microsoft.VisualStudio.Services.Common;

namespace AutoKeyNet.Facades;

/// <summary>
///     Class for emulating Vim commands
/// </summary>
internal class VimKeyHandler : BaseKeyHandler
{
    /// <summary>
    ///     Time in milliseconds to wait for a mapped sequence to complete
    /// </summary>
    private const int TimeoutLen = 500;

    /// <summary>
    ///     Array of virtual keys that trigger the clearing of the buffer
    /// </summary>
    private static readonly HashSet<VIRTUAL_KEY> ClearBufferKey =
    [
        VIRTUAL_KEY.VK_RIGHT,
        VIRTUAL_KEY.VK_LEFT,
        VIRTUAL_KEY.VK_UP,
        VIRTUAL_KEY.VK_DOWN,
        VIRTUAL_KEY.VK_END,
        VIRTUAL_KEY.VK_HOME
    ];

    private static readonly HashSet<MOUSE_EVENT_FLAGS> MouseEventFlagsToClearBuffer =
    [
        MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTDOWN,
        MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTUP,
        MOUSE_EVENT_FLAGS.MOUSEEVENTF_RIGHTDOWN,
        MOUSE_EVENT_FLAGS.MOUSEEVENTF_RIGHTUP,
        MOUSE_EVENT_FLAGS.MOUSEEVENTF_MIDDLEDOWN,
        MOUSE_EVENT_FLAGS.MOUSEEVENTF_MIDDLEUP,
        MOUSE_EVENT_FLAGS.MOUSEEVENTF_XDOWN,
        MOUSE_EVENT_FLAGS.MOUSEEVENTF_XUP
    ];

    /// <summary>
    ///     Buffer for pressed keys
    /// </summary>
    private readonly CircularBuffer<char> _buffer;

    /// <summary>
    ///     Timestamp for when the last key was pressed.
    /// </summary>
    private uint _lastTimeStamp;

    /// <summary>
    ///     Cancellation token source for cancelling the triggering of rules.
    /// </summary>
    private CancellationTokenSource _source = new();

    /// <summary>
    ///     Constructor of objects for emulating Vim commands.
    /// </summary>
    /// <param name="rules">List of rules</param>
    /// <param name="kbdHook">Keyboard hook</param>
    /// <param name="mouseHook">Mouse hook</param>
    /// <param name="winHook">Windows hook</param>
    public VimKeyHandler(IEnumerable<BaseRuleRecord> rules, KeyboardHook kbdHook, MouseHook mouseHook,
        WinHook winHook) : base(rules, kbdHook, mouseHook, winHook)
    {
        var bufferSize = Rules.Max(r => r.KeyChars.Length);
        _buffer = new CircularBuffer<char>(bufferSize);
    }


    /// <summary>
    ///     Method for handling the event when the foreground window changes
    /// </summary>
    /// <param name="sender">Sender of the event</param>
    /// <param name="e">Event arguments</param>
    protected override void OnWinHookEvent(object? sender, WinBaseHookEventArgs e)
    {
        _buffer.Clear();
    }

    /// <summary>
    ///     Method for handling mouse events
    /// </summary>
    /// <param name="sender">Sender of the event</param>
    /// <param name="e">Event arguments</param>
    protected override void OnMouseHookEvent(object? sender, HookEventArgs e)
    {
        if (e.Input.type == INPUT_TYPE.INPUT_MOUSE
            && MouseEventFlagsToClearBuffer.Contains(e.Input.Anonymous.mi.dwFlags))
            _buffer.Clear();
    }

    /// <summary>
    ///     Method for handling keyboard events
    /// </summary>
    /// <param name="sender">Sender of the event</param>
    /// <param name="e">Event arguments</param>
    protected override void OnKeyboardHookEvent(object? sender, HookEventArgs e)
    {
        if (e.Input.type == INPUT_TYPE.INPUT_KEYBOARD)
        {
            KEYBDINPUT ki = e.Input.Anonymous.ki;
            if (ki.dwFlags != KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP)
            {
                // Clear the buffer
                if (ClearBufferKey.Contains(ki.wVk))
                {
                    _buffer.Clear();
                    return;
                }

                // Clear the buffer if the time between pressed keys is longer than TimeoutLen.
                if (ki.time - _lastTimeStamp > TimeoutLen)
                    _buffer.Clear();


                char invariantLetter = ki.wVk.ToUnicode(true);
                if (char.IsLetterOrDigit(invariantLetter))
                {
                    _buffer.Add(invariantLetter);
                    bool isSuppressedKeys = Rules.OfType<VimKeyRuleRecord>().Any(r =>
                        r.KeyChars.Take(_buffer.Count).SequenceEqual(_buffer)
                        && (r.CheckWindowCondition?.Invoke(e.WindowTitle, e.WindowClass, e.WindowModule,
                            e.WindowControl) ?? true));
                    if (isSuppressedKeys)
                    {
                        _lastTimeStamp = ki.time;
                        e.Cancel = true;
                        Debug.WriteLine($"info: {GetType().Name}: {_buffer}");
                        return;
                    }

                    _buffer.Clear();
                }
            }
            else
            {
                if (CheckRules(e.WindowTitle, e.WindowClass, e.WindowModule, e.WindowControl))
                    _buffer.Clear();
            }
        }
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
        VimKeyRuleRecord? foundRule = null;
        var keysStartWithRules = false;
        foreach (var rule in Rules)
            if (rule is VimKeyRuleRecord vkRule
                && (vkRule.CheckWindowCondition?.Invoke(windowTitle, windowClass, windowModule, windowControl) ??
                    true)
                && rule.KeyChars.Take(_buffer.Count).SequenceEqual(_buffer))
            {
                if (rule.KeyChars.Length == _buffer.Count)
                {
                    foundRule = vkRule;
                    _source.Cancel();
                }
                else
                {
                    keysStartWithRules = true;
                }

                if (foundRule is not null && keysStartWithRules)
                {
                    _source = new CancellationTokenSource();
                    Task.Delay(TimeoutLen, _source.Token).ContinueWith(_ =>
                    {
                        foundRule.Run.Invoke();
                        Debug.WriteLine($"rule: {GetType().Name}: {CharExtension.ToString(foundRule.KeyChars)}");
                    }, _source.Token);
                    return false;
                }
            }

        if (foundRule is not null)
        {
            foundRule.Run.Invoke();
            Debug.WriteLine($"rule: {GetType().Name}: {CharExtension.ToString(foundRule.KeyChars)}");
            return true;
        }

        return false;
    }
}