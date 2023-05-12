//using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AutoKeyNet.WindowsHooks.Hooks;
using AutoKeyNet.WindowsHooks.Hooks.EventArgs;
using AutoKeyNet.WindowsHooks.Rule;
using AutoKeyNet.WindowsHooks.WindowsEnums;
using AutoKeyNet.WindowsHooks.WindowsStruct;
using Timer = System.Threading.Timer;

namespace AutoKeyNet.WindowsHooks.Facades;
internal class VimKeyFacade : BaseKeyFacade
{
    /// <summary>
    /// Промежуток времени между нажатием клавиш, которые должны считаться одной командой
    /// </summary>
    private const int TimeoutLen = 500;

    /// <summary>
    /// Перечень клавиш которые вызывают отчистку буфера
    /// </summary>
    private readonly ushort[] _clearBufferKey = {
            (ushort)VirtualKey.RIGHT,
            (ushort) VirtualKey.LEFT,
            (ushort) VirtualKey.UP,
            (ushort) VirtualKey.DOWN,
            (ushort) VirtualKey.END,
            (ushort) VirtualKey.HOME
        };

    /// <summary>
    /// Буфер нажатых клавиш
    /// </summary>
    private string _buffer;

    /// <summary>
    /// Временная отметка последней нажатой клавиши
    /// </summary>
    private uint _lastTimeStamp;

    /// <summary>
    /// Генератор токенов для отмены выполнения правил
    /// </summary>
    private CancellationTokenSource _source = new();

    public VimKeyFacade(IEnumerable<BaseRuleRecord> rules, KeyboardHook kbdHook, MouseHook mouseHook, WinHook winHook) : base(rules)
    {
        _buffer = string.Empty;

        winHook.OnHookEvent += OnWinHookEvent;
        mouseHook.OnHookEvent += OnMouseHookEvent;
        kbdHook.OnHookEvent += OnKeyboardHookEvent;
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

            // Если после нажатия последней клавиши прошло больше секунды то сбросить буфер
            if ((kbd.time - _lastTimeStamp) > TimeoutLen)
                _buffer = string.Empty;


            bool isFound = false;
            if (char.IsLetterOrDigit(e.InvariantLetter))
            {
                string newBuffer = _buffer + e.InvariantLetter;
                foreach (BaseRuleRecord rule in Rules)
                {
                    if (rule is VimKeyRuleRecord vkRule
                        && (vkRule.CheckWindowCondition?.Invoke(e.WindowTitle, e.WindowClass, e.WindowModule,
                            e.WindowControl) ?? true)
                        && rule.KeyWord.StartsWith(newBuffer))
                    {
                        isFound = true;
                        break;
                    }
                }

                if (isFound)
                {
                    _buffer = newBuffer;
                    _lastTimeStamp = kbd.time;
                    e.Cancel = true;
                    Debug.WriteLine($"{e.InvariantLetter} --> {_buffer}");
                    return;
                }

                _buffer = string.Empty;
            }
            //}
        }

        if (e.WParam == (IntPtr)KeyboardMessage.WM_KEYUP)
        {
            //KeyboardLowLevelHook kbd = (KeyboardLowLevelHook)(Marshal.PtrToStructure(e.LParam, typeof(KeyboardLowLevelHook)) ??
            //throw new InvalidOperationException());
            //if (kbd.dwExtraInfo != (UIntPtr)Constants.KEY_IGNORE)
            //{
            if (CheckRules(e.WindowTitle, e.WindowClass, e.WindowModule, e.WindowControl))
                _buffer = string.Empty;
            //}
        }
    }

    /// <summary>
    /// Проверка критериев на соответствие буфера 
    /// </summary>
    private bool CheckRules(string? windowTitle, string? windowClass, string? windowModule, string? windowControl)
    {
        VimKeyRuleRecord? foundRule = null;
        bool keysStartWithRules = false;
        foreach (BaseRuleRecord rule in Rules)
        {
            if (rule is VimKeyRuleRecord vkRule
                && (vkRule.CheckWindowCondition?.Invoke(windowTitle, windowClass, windowModule, windowControl) ?? true)
                && rule.KeyWord.StartsWith(_buffer))
            {
                if (rule.KeyWord.Length == _buffer.Length)
                {
                    foundRule = vkRule;
                    _source.Cancel();
                }
                else
                    keysStartWithRules = true;

                if (foundRule is not null && keysStartWithRules)
                {
                    _source = new CancellationTokenSource();
                    Task.Delay(TimeoutLen, _source.Token).ContinueWith(_ => foundRule.Run.Invoke(), _source.Token);
                    return false;
                }
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