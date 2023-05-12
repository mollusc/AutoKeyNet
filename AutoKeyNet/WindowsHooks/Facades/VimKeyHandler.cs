using System.Diagnostics;
using System.Runtime.InteropServices;
using AutoKeyNet.WindowsHooks.Hooks;
using AutoKeyNet.WindowsHooks.Hooks.EventArgs;
using AutoKeyNet.WindowsHooks.Rule;
using AutoKeyNet.WindowsHooks.WindowsEnums;
using AutoKeyNet.WindowsHooks.WindowsStruct;

namespace AutoKeyNet.WindowsHooks.Facades;

/// <summary>
///     Class for emulating Vim commands
/// </summary>
internal class VimKeyHandler : BaseKeyHandler, IDisposable
{
    /// <summary>
    ///     Time in milliseconds to wait for a mapped sequence to complete
    /// </summary>
    private const int TimeoutLen = 500;

    /// <summary>
    ///     Array of virtual keys that trigger the clearing of the buffer
    /// </summary>
    private readonly HashSet<ushort> _clearBufferKey = new()
    {
        (ushort)VirtualKey.RIGHT,
        (ushort)VirtualKey.LEFT,
        (ushort)VirtualKey.UP,
        (ushort)VirtualKey.DOWN,
        (ushort)VirtualKey.END,
        (ushort)VirtualKey.HOME
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
    ///     Windows hook
    /// </summary>
    private readonly WinHook _winHook;

    /// <summary>
    ///     Buffer for pressed keys
    /// </summary>
    private string _buffer;

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
        WinHook winHook) : base(rules)
    {
        _buffer = string.Empty;

        _winHook = winHook;
        _winHook.OnHookEvent += OnWinHookEvent;
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
        _winHook.OnHookEvent -= OnWinHookEvent;
        _mouseHook.OnHookEvent -= OnMouseHookEvent;
        _keyboardHook.OnHookEvent -= OnKeyboardHookEvent;
    }

    /// <summary>
    ///     Method for handling mouse events
    /// </summary>
    /// <param name="sender">Sender of the event</param>
    /// <param name="e">Event arguments</param>
    private void OnMouseHookEvent(object? sender, MouseHookEventArgs e)
    {
        if ((MouseMessage)e.WParam is MouseMessage.WM_LBUTTONDOWN or MouseMessage.WM_LBUTTONUP
            or MouseMessage.WM_LBUTTONDBLCLK or
            MouseMessage.WM_RBUTTONDOWN or MouseMessage.WM_RBUTTONUP or MouseMessage.WM_RBUTTONDBLCLK or
            MouseMessage.WM_MBUTTONDOWN or MouseMessage.WM_MBUTTONUP or MouseMessage.WM_MBUTTONDBLCLK)
            _buffer = string.Empty;
    }

    /// <summary>
    ///     Method for handling the event when the foreground window changes
    /// </summary>
    /// <param name="sender">Sender of the event</param>
    /// <param name="e">Event arguments</param>
    private void OnWinHookEvent(object? sender, WinBaseHookEventArgs e)
    {
        _buffer = string.Empty;
    }

    /// <summary>
    ///     Method for handling keyboard events
    /// </summary>
    /// <param name="sender">Sender of the event</param>
    /// <param name="e">Event arguments</param>
    private void OnKeyboardHookEvent(object? sender, KeyboardHookEventArgs e)
    {
        if (e.WParam == (nint)KeyboardMessage.WM_KEYDOWN)
        {
            var kbd = (KeyboardLowLevelHook)(Marshal.PtrToStructure(e.LParam, typeof(KeyboardLowLevelHook)) ??
                                             throw new InvalidOperationException());
            // Clear the buffer
            if (_clearBufferKey.Contains((ushort)kbd.VirtualKey))
            {
                _buffer = string.Empty;
                return;
            }

            // Clear the buffer if the time between pressed keys is longer than TimeoutLen.
            if (kbd.Time - _lastTimeStamp > TimeoutLen)
                _buffer = string.Empty;


            var isFound = false;
            if (char.IsLetterOrDigit(e.InvariantLetter))
            {
                var newBuffer = _buffer + e.InvariantLetter;
                foreach (var rule in Rules)
                    if (rule is VimKeyRuleRecord vkRule
                        && (vkRule.CheckWindowCondition?.Invoke(e.WindowTitle, e.WindowClass, e.WindowModule,
                            e.WindowControl) ?? true)
                        && rule.KeyText.StartsWith(newBuffer))
                    {
                        isFound = true;
                        break;
                    }

                if (isFound)
                {
                    _buffer = newBuffer;
                    _lastTimeStamp = kbd.Time;
                    e.Cancel = true;
                    Debug.WriteLine($"{e.InvariantLetter} --> {_buffer}");
                    return;
                }

                _buffer = string.Empty;
            }
        }

        if (e.WParam == (nint)KeyboardMessage.WM_KEYUP)
            if (CheckRules(e.WindowTitle, e.WindowClass, e.WindowModule, e.WindowControl))
                _buffer = string.Empty;
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
                && (vkRule.CheckWindowCondition?.Invoke(windowTitle, windowClass, windowModule, windowControl) ?? true)
                && rule.KeyText.StartsWith(_buffer))
            {
                if (rule.KeyText.Length == _buffer.Length)
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
                    Task.Delay(TimeoutLen, _source.Token).ContinueWith(_ => foundRule.Run.Invoke(), _source.Token);
                    return false;
                }
            }

        if (foundRule is not null)
        {
            foundRule.Run();
            return true;
        }

        return false;
    }
}