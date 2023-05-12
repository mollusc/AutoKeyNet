using AutoKeyNet.WindowsHooks.Hooks;
using AutoKeyNet.WindowsHooks.Rule;

namespace AutoKeyNet.WindowsHooks.Facades;
public class AutoKeyNetFacade : IDisposable
{
    private readonly HotKeyFacade _hotKey;
    private readonly HotStringFacade _hotString;
    private readonly VimKeyFacade _vimKey;

    private readonly WinHook _winHook;
    private readonly MouseHook _mouseHook;
    private readonly KeyboardHook _kbdHook;

    public AutoKeyNetFacade(IEnumerable<BaseRuleRecord> rules)
    {
        _winHook = new WinHook();
        _mouseHook = new MouseHook();
        _kbdHook = new KeyboardHook();

        var baseRuleRecords = rules as BaseRuleRecord[] ?? rules.ToArray();
        _hotKey = new HotKeyFacade(baseRuleRecords, _kbdHook, _mouseHook);
        _hotString = new HotStringFacade(baseRuleRecords, _kbdHook, _mouseHook, _winHook);
        _vimKey = new VimKeyFacade(baseRuleRecords, _kbdHook, _mouseHook, _winHook);
    }

    public void Dispose()
    {
        _winHook?.Dispose();
        _mouseHook?.Dispose();
        _kbdHook?.Dispose();

        _hotKey?.Dispose();
        _hotString?.Dispose();
        _vimKey?.Dispose();
    }
}
