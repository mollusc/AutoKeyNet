using AutoKeyNet.WindowsHooks.Hooks;
using AutoKeyNet.WindowsHooks.Rule;

namespace AutoKeyNet.WindowsHooks.Facades;

/// <summary>
///     Class for combining all key handlers
/// </summary>
public class AutoKeyNetFacade : IDisposable
{
    private readonly NewHotKeyHandler _hotKeyHandler;
    private readonly HotStringHandler _hotStringHandler;
    private readonly KeyboardHook _kbdHook;
    private readonly MouseHook _mouseHook;
    private readonly VimKeyHandler _vimKeyHandler;

    private readonly WinHook _winHook;

    /// <summary>
    ///     Constructor for initializing all key handlers
    /// </summary>
    /// <param name="rules">List of rules</param>
    public AutoKeyNetFacade(IEnumerable<BaseRuleRecord> rules)
    {
        _winHook = new WinHook();
        _mouseHook = new MouseHook();
        _kbdHook = new KeyboardHook();

        var baseRuleRecords = rules as BaseRuleRecord[] ?? rules.ToArray();
        _hotKeyHandler = new NewHotKeyHandler(baseRuleRecords, _kbdHook, _mouseHook);
        _hotStringHandler = new HotStringHandler(baseRuleRecords, _kbdHook, _mouseHook, _winHook);
        _vimKeyHandler = new VimKeyHandler(baseRuleRecords, _kbdHook, _mouseHook, _winHook);
    }

    /// <summary>
    ///     Method for disposing of objects
    /// </summary>
    public void Dispose()
    {
        _winHook?.Dispose();
        _mouseHook?.Dispose();
        _kbdHook?.Dispose();

        _hotKeyHandler?.Dispose();
        _hotStringHandler?.Dispose();
        _vimKeyHandler?.Dispose();
    }
}