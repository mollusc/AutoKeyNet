using System.Diagnostics;
using System.Runtime.InteropServices;
using AutoKeyNet.WindowsHooks.Hooks;
using AutoKeyNet.WindowsHooks.Hooks.EventArgs;
using AutoKeyNet.WindowsHooks.Rule;
using AutoKeyNet.WindowsHooks.WindowsEnums;
using AutoKeyNet.WindowsHooks.WindowsStruct;

namespace AutoKeyNet.WindowsHooks.Facades;
internal class HotStringFacade : BaseKeyFacade, IDisposable
{
    /// <summary>
    /// Перечень клавиш которые вызывают отчистку буфера
    /// </summary>
    private readonly ushort[] _clearBufferKey = {
            (ushort)VirtualKey.RIGHT,
            (ushort)VirtualKey.LEFT,
            (ushort)VirtualKey.UP,
            (ushort)VirtualKey.DOWN,
            (ushort)VirtualKey.END,
            (ushort)VirtualKey.HOME
        };
    /// <summary>
    /// Перечень символов, которые являются окончанием слова
    /// </summary>
    private readonly char[] _endWordCharacters = { ' ', '-', '(', ')', '[', ']', '{', '}', ':', ';', '"', '/', '\\', ',', '.', '?', '!', '\t', '\n', '\r' };

    /// <summary>
    /// Буфер нажатых клавиш
    /// </summary>
    private string _buffer;

    private readonly WinHook _winHook;
    private readonly MouseHook _mouseHook;
    private readonly KeyboardHook _keyboardHook;

    public HotStringFacade(IEnumerable<BaseRuleRecord> rules, KeyboardHook kbdHook, MouseHook mouseHook, WinHook winHook) : base(rules)
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
    /// Метод выполняется при возникновении событий связанных с мышью 
    /// </summary>
    /// <param name="sender">Инициатор события</param>
    /// <param name="e">Параметры события</param>
    private void OnMouseHookEvent(object? sender, MouseHookEventArgs e)
    {
        if ((MouseMessage)e.WParam is MouseMessage.WM_LBUTTONDOWN or MouseMessage.WM_LBUTTONUP
            or MouseMessage.WM_LBUTTONDBLCLK or
            MouseMessage.WM_RBUTTONDOWN or MouseMessage.WM_RBUTTONUP or MouseMessage.WM_RBUTTONDBLCLK or
            MouseMessage.WM_MBUTTONDOWN or MouseMessage.WM_MBUTTONUP or MouseMessage.WM_MBUTTONDBLCLK)
            _buffer = string.Empty;
    }

    /// <summary>
    /// Метод выполняется при возникновении событий связанных со сменой текущего окна Windows 
    /// </summary>
    /// <param name="sender">Инициатор события</param>
    /// <param name="e">Параметры события</param>
    private void OnWinHookEvent(object? sender, WinBaseHookEventArgs e)
    {
        _buffer = string.Empty;
    }

    /// <summary>
    /// Метод выполняется при возникновении событий связанных с клавиатурой 
    /// </summary>
    /// <param name="sender">Инициатор события</param>
    /// <param name="e">Параметры события</param>
    private void OnKeyboardHookEvent(object? sender, KeyBaseHookEventArgs e)
    {
        if (e.WParam == (IntPtr)KeyboardMessage.WM_KEYDOWN)
        {
            KeyboardLowLevelHook kbd = (KeyboardLowLevelHook)(Marshal.PtrToStructure(e.LParam, typeof(KeyboardLowLevelHook)) ??
                                                    throw new InvalidOperationException());
            //if (kbd.dwExtraInfo != (UIntPtr)Constants.KEY_IGNORE)
            //{
            // Отчищаем буфер в случае специальных клавиш
            if (_clearBufferKey.Contains((ushort)kbd.vkCode))
            {
                _buffer = string.Empty;
                return;
            }

            // Удаляем последний символ из буфера если пользователь нажимает на BackSpace
            if (kbd.vkCode == VirtualKey.BACK)
            {
                if (_buffer.Length > 0)
                    _buffer = _buffer.Remove(_buffer.Length - 1);
                return;
            }

            if (char.IsLetterOrDigit(e.Letter))
            {
                _buffer += e.Letter;
                Debug.WriteLine($"{e.Letter} --> {_buffer}");
            }
            //}
        }

        if (e.WParam == (IntPtr)KeyboardMessage.WM_KEYUP)
        {
            //KeyboardLowLevelHook kbd = (KeyboardLowLevelHook)(Marshal.PtrToStructure(e.LParam, typeof(KeyboardLowLevelHook)) ??
            //throw new InvalidOperationException());
            //if (kbd.dwExtraInfo != (UIntPtr)Constants.KEY_IGNORE)
            //{
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
            //}
        }
    }

    /// <summary>
    /// Проверка критериев на соответствие буфера 
    /// </summary>
    private bool CheckRules(bool triggerByEndCharacter, string? windowTitle, string? windowClass,
        string? windowModule, string? windowControl)
    {
        foreach (BaseRuleRecord rule in Rules)
        {
            if (rule is HotStringRuleRecord hsRule
                && _buffer.Equals(hsRule.KeyWord)
                && triggerByEndCharacter == hsRule.TriggerByEndingCharacter
                && (hsRule.CheckWindowCondition?.Invoke(windowTitle, windowClass, windowModule, windowControl) ??
                    true))
            {
                hsRule.Run.Invoke();
                return true;
            }
        }
        return false;
    }

    public void Dispose()
    {
        _winHook.OnHookEvent -= OnWinHookEvent;
        _mouseHook.OnHookEvent -= OnMouseHookEvent;
        _keyboardHook.OnHookEvent -= OnKeyboardHookEvent;
    }
}