using System.Runtime.InteropServices;
using AutoKeyNet.WindowsHooks.Hooks;
using AutoKeyNet.WindowsHooks.Hooks.EventArgs;
using AutoKeyNet.WindowsHooks.Rule;
using AutoKeyNet.WindowsHooks.WindowsEnums;
using AutoKeyNet.WindowsHooks.WindowsStruct;

namespace AutoKeyNet.WindowsHooks.Facades;

/// <summary>
///     Class for handling HotStrings
/// </summary>
internal class HotStringHandler : BaseKeyHandler, IDisposable
{
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
    ///     Array of letters that represent the end of a word
    /// </summary>
    private readonly HashSet<char> _endWordCharacters = new()
        { ' ', '-', '(', ')', '[', ']', '{', '}', ':', ';', '"', '/', '\\', ',', '.', '?', '!', '\t', '\n', '\r' };

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
    ///     Constructor of the class for handling HotStrings
    /// </summary>
    /// <param name="rules">List of rules</param>
    /// <param name="kbdHook">Keyboard hook</param>
    /// <param name="mouseHook">Mouse hook</param>
    /// <param name="winHook">Windows hook</param>
    public HotStringHandler(IEnumerable<BaseRuleRecord> rules, KeyboardHook kbdHook, MouseHook mouseHook,
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

            // Remove the last character when the Backspace key is pressed
            if (kbd.VirtualKey == VirtualKey.BACK)
            {
                if (_buffer.Length > 0)
                    _buffer = _buffer.Remove(_buffer.Length - 1);
                return;
            }

            if (char.IsLetterOrDigit(e.Letter)) _buffer += e.Letter;
        }

        if (e.WParam == (nint)KeyboardMessage.WM_KEYUP)
        {
            if (_endWordCharacters.Contains(e.Letter))
            {
                CheckRules(true, e.WindowTitle, e.WindowClass, e.WindowModule, e.WindowControl);
                _buffer = string.Empty;
            }
            else if (_buffer.Length > 0 && char.IsLetterOrDigit(e.Letter))
            {
                if (CheckRules(false, e.WindowTitle, e.WindowClass, e.WindowModule, e.WindowControl))
                    _buffer = string.Empty;
            }
        }
    }


    /// <summary>
    ///     Method for checking rules
    /// </summary>
    /// <param name="isEndOfWord">True if the buffer contains a complete word; otherwise, false.</param>
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
    private bool CheckRules(bool isEndOfWord, string? windowTitle, string? windowClass,
        string? windowModule, string? windowControl)
    {
        foreach (var rule in Rules)
            if (rule is HotStringRuleRecord hsRule
                && _buffer.Equals(hsRule.KeyText)
                && isEndOfWord == hsRule.TriggerByEndingCharacter
                && (hsRule.CheckWindowCondition?.Invoke(windowTitle, windowClass, windowModule, windowControl) ??
                    true))
            {
                hsRule.Run.Invoke();
                return true;
            }

        return false;
    }
}