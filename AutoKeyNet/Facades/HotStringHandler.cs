using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using AutoKeyNet.WindowsHooks.Hooks;
using AutoKeyNet.WindowsHooks.Hooks.EventArgs;
using Microsoft.VisualStudio.Services.Common;
using AutoKeyNet.Helper;
using AutoKeyNet.RuleRecords;

namespace AutoKeyNet.Facades;

/// <summary>
///     Class for handling HotStrings
/// </summary>
internal class HotStringHandler : BaseKeyHandler
{
    private readonly CircularBuffer<char> _buffer;

    /// <summary>
    ///     Array of letters that represent the end of a word
    /// </summary>
    private static readonly HashSet<char> EndWordCharacters =
        [' ', '-', '(', ')', '[', ']', '{', '}', ':', ';', '"', '/', '\\', ',', '.', '?', '!', '\t', '\n', '\r'];

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
    ///     Array of virtual keys that trigger the clearing of the buffer
    /// </summary>
    private static readonly HashSet<VIRTUAL_KEY> ClearBufferKey =
    [
        VIRTUAL_KEY.VK_UP,
        VIRTUAL_KEY.VK_DOWN,
        VIRTUAL_KEY.VK_END,
        VIRTUAL_KEY.VK_HOME
    ];

    public HotStringHandler(IEnumerable<HotStringRuleRecord> rules, KeyboardHook kbdHook, MouseHook mouseHook,
        WinHook winHook)
        : base(rules, kbdHook, mouseHook, winHook)
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

                // Remove the last character when the Backspace key is pressed
                if (ki.wVk == VIRTUAL_KEY.VK_BACK && _buffer.Count > 0)
                {
                    _buffer.PopBack();
                    return;
                }

                char letter = ki.wVk.ToUnicode();
                if (char.IsLetterOrDigit(letter))
                {
                    _buffer.Add(letter);
                    Debug.WriteLine($"info: {GetType().Name}: {_buffer}");
                }
            }
            else
            {
                char letter = ki.wVk.ToUnicode();
                if (EndWordCharacters.Contains(letter))
                {
                    var firedRules = CheckRules(true, e.WindowTitle, e.WindowClass, e.WindowModule, e.WindowControl);
                    firedRules.ForEach(r =>
                    {
                        r.Run.Invoke();
                        Debug.WriteLine($"rule: {GetType().Name}: {CharExtension.ToString(r.KeyChars)}");
                    });
                    _buffer.Clear();
                }
                else if (_buffer.Count > 0 && char.IsLetterOrDigit(letter))
                {
                    var firedRules = CheckRules(false, e.WindowTitle, e.WindowClass, e.WindowModule, e.WindowControl).ToArray();
                    if (firedRules.Any())
                    {
                        firedRules.ForEach(r =>
                        {
                            r.Run.Invoke();
                            Debug.WriteLine($"rule: {GetType().Name}: {CharExtension.ToString(r.KeyChars)}");
                        });
                        _buffer.Clear();
                    }
                }
            }
        }
    }

    private IEnumerable<HotStringRuleRecord> CheckRules(bool isEndOfWord, string? windowTitle, string? windowClass, string? windowModule,
        string? windowControl)
    {
        if (_buffer.Count > 0)
            foreach (var rule in Rules)
                if (rule is HotStringRuleRecord hotStringRuleRecord
                    && _buffer.SequenceEqual(hotStringRuleRecord.KeyChars)
                    && isEndOfWord == hotStringRuleRecord.TriggerByEndingCharacter
                    && (rule.CheckWindowCondition?.Invoke(windowTitle, windowClass, windowModule, windowControl) ??
                        true))
                {
                    yield return hotStringRuleRecord;
                }
    }
}
