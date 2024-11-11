using AutoKeyNet.RuleRecords;
using AutoKeyNet.WindowsHooks.Hooks;
using AutoKeyNet.WindowsHooks.Hooks.EventArgs;

namespace AutoKeyNet.Facades;

/// <summary>
///     Base class for handling hotkeys
/// </summary>
internal abstract class BaseKeyHandler
{
    /// <summary>
    ///     List of rules
    /// </summary>
    protected IEnumerable<BaseRuleRecord> Rules;

    /// <summary>
    ///     Keyboard hook
    /// </summary>
    protected KeyboardHook KeyboardHook;

    /// <summary>
    ///     Mouse hook
    /// </summary>
    protected MouseHook MouseHook;

    protected WinHook WinHook;

    /// <summary>
    ///     Constructor of base class for handling hotkeys
    /// </summary>
    /// <param name="rules">List of rules for managing hotkeys</param>
    /// <param name="kbdHook"></param>
    /// <param name="mouseHook"></param>
    /// <param name="winHook"></param>
    protected BaseKeyHandler(IEnumerable<BaseRuleRecord> rules, KeyboardHook kbdHook, MouseHook mouseHook, WinHook winHook)
    {
        Rules = rules;
        MouseHook = mouseHook;
        MouseHook.HookEvent += OnMouseHookEvent;
        KeyboardHook = kbdHook;
        KeyboardHook.HookEvent += OnKeyboardHookEvent;
        WinHook = winHook;
        WinHook.HookEvent += OnWinHookEvent;
    }

    protected virtual void OnWinHookEvent(object? sender, WinBaseHookEventArgs e)
    {
    }

    protected virtual void OnKeyboardHookEvent(object? sender, HookEventArgs e)
    {
    }

    protected virtual void OnMouseHookEvent(object? sender, HookEventArgs e)
    {
    }

    /// <summary>
    ///     Method for disposing of hooks
    /// </summary>
    public void Dispose()
    {
        MouseHook.HookEvent -= OnMouseHookEvent;
        KeyboardHook.HookEvent -= OnKeyboardHookEvent;
        WinHook.HookEvent -= OnWinHookEvent;
    }
}