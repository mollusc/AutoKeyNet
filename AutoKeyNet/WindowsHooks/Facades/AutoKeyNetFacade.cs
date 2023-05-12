using AutoKeyNet.WindowsHooks.Hooks;
using AutoKeyNet.WindowsHooks.Rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoKeyNet.WindowsHooks.Facades;
public class AutoKeyNetFacade : IDisposable
{
    private HotKeyFacade _hotKey;
    private HotStringFacade _hotString;
    private VimKeyFacade _vimKey;

    private readonly WinHook _winHook;
    private readonly MouseHook _mouseHook;
    private readonly KeyboardHook _kbdHook;

    public AutoKeyNetFacade(IEnumerable<BaseRuleRecord> rules)
    {
        _winHook = new();
        _mouseHook = new();
        _kbdHook = new();

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
    }
}
