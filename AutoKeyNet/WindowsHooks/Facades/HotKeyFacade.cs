using AutoKeyNet.WindowsHooks.WindowsEnums;
using AutoKeyNet.WindowsHooks.WindowsStruct;
using System.Runtime.InteropServices;
using AutoKeyNet.WindowsHooks.Rule;
using AutoKeyNet.WindowsHooks.Hooks;
using AutoKeyNet.WindowsHooks.Hooks.EventArgs;

namespace AutoKeyNet.WindowsHooks.Facades;

/// <summary>
/// Отслеживание нажатия горячих клавиш
/// </summary>
internal class HotKeyFacade : BaseKeyFacade, IDisposable
{
    /// <summary>
    /// Словарь перечня событий Windows и соответствующая им виртуальная клавиша. Предназначено для активации
    /// сравнения клавиши с перечнем условий
    /// </summary>
    private readonly Dictionary<(MouseMessage, int), VirtualKey> _activateMouseKeyEvent = new()
    {
        { (MouseMessage.WM_LBUTTONDOWN, 0), VirtualKey.LBUTTON },
        { (MouseMessage.WM_RBUTTONDOWN, 0), VirtualKey.RBUTTON },
        { (MouseMessage.WM_MBUTTONDOWN, 0), VirtualKey.MBUTTON },
        { (MouseMessage.WM_XBUTTONDOWN, 1), VirtualKey.XBUTTON1 },
        { (MouseMessage.WM_XBUTTONDOWN, 2), VirtualKey.XBUTTON2 },
    };
    /// <summary>
    /// Словарь перечня событий Windows и соответствующая ему виртуальная клавиша. Предназначено для ДЕактивации
    /// сравнения клавиши с перечнем условий
    /// </summary>
    private readonly Dictionary<(MouseMessage, int), VirtualKey> _deactivateMouseKeyEvent = new()
    {
        { (MouseMessage.WM_LBUTTONUP, 0), VirtualKey.LBUTTON },
        { (MouseMessage.WM_RBUTTONUP, 0), VirtualKey.RBUTTON },
        { (MouseMessage.WM_MBUTTONUP, 0), VirtualKey.MBUTTON },
        { (MouseMessage.WM_XBUTTONUP, 1), VirtualKey.XBUTTON1 },
        { (MouseMessage.WM_XBUTTONUP, 2), VirtualKey.XBUTTON2 },
    };

    /// <summary>
    /// Буфер нажатых клавиш
    /// </summary>
    private readonly HashSet<ushort> _buffer = new();

    private readonly MouseHook _mouseHook;
    private readonly KeyboardHook _keyboardHook;

    public HotKeyFacade(IEnumerable<BaseRuleRecord> rules, KeyboardHook kbdHook, MouseHook mouseHook) : base(rules)
    {
        _mouseHook = mouseHook;
        _mouseHook.OnHookEvent += OnMouseHookEvent;
        _keyboardHook = kbdHook;
        _keyboardHook.OnHookEvent += OnKeyboardHookEvent;
    }

    /// <summary>
    /// Метод выполняется при возникновении событий связанных с мышью 
    /// </summary>
    /// <param name="sender">Инициатор события</param>
    /// <param name="e">Параметры события</param>
    private void OnMouseHookEvent(object? sender, MouseHookEventArgs e)
    {
        if (_activateMouseKeyEvent.TryGetValue(((MouseMessage)e.WParam, e.MouseData), out var aKey))
            _buffer.Add((ushort)aKey);
        if (_deactivateMouseKeyEvent.TryGetValue(((MouseMessage)e.WParam, e.MouseData), out var dKey))
            _buffer.Remove((ushort)dKey);

        CheckRules(e.WindowTitle, e.WindowTitle, e.WindowModule, e.WindowControl);
    }

    /// <summary>
    /// Метод выполняется при возникновении событий связанных с клавиатурой 
    /// </summary>
    /// <param name="sender">Инициатор события</param>
    /// <param name="e">Параметры события</param>
    private void OnKeyboardHookEvent(object? sender, KeyBaseHookEventArgs e)
    {
        KeyboardLowLevelHook kbd = (KeyboardLowLevelHook)(Marshal.PtrToStructure(e.LParam, typeof(KeyboardLowLevelHook)) ??
                                                throw new InvalidOperationException());
        //if (kbd.dwExtraInfo != (UIntPtr)Constants.KEY_IGNORE)
        //{
            if (e.WParam == (IntPtr)KeyboardMessage.WM_KEYDOWN)
            {
                _buffer.Add((ushort)kbd.vkCode);
                if (CheckRules(e.WindowTitle, e.WindowClass, e.WindowModule, e.WindowControl))
                    e.Cancel = true;
            }

            if (e.WParam == (IntPtr)KeyboardMessage.WM_KEYUP)
            {
                _buffer.Remove((ushort)kbd.vkCode);
            }

            // Panic Button. Возможность отчистить содержимое буфера если он перестанет
            // работать корректно
            if (kbd.vkCode == VirtualKey.ESCAPE)
                _buffer.Clear();
        //}
    }

    /// <summary>
    /// Проверка критериев на соответствие буфера 
    /// </summary>
    private bool CheckRules(string? windowTitle, string? windowClass, string? windowModule, string? windowControl)
    {
        bool result = false;
        if (_buffer.Count > 0)
        {
            foreach (BaseRuleRecord rule in Rules)
            {
                if (rule is HotKeyRuleRecord
                    && _buffer.SetEquals(rule.InputKeys.Select(x => x.U.ki.wVk))
                    && (rule.CheckWindowCondition?.Invoke(windowTitle, windowClass, windowModule, windowControl) ??
                        true))
                {
                    rule.Run.Invoke();
                    result = true;
                }
            }
        }

        return result;
    }

    public void Dispose()
    {
        _mouseHook.OnHookEvent -= OnMouseHookEvent;
        _keyboardHook.OnHookEvent -= OnKeyboardHookEvent;
    }
}