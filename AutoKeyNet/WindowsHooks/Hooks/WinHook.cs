using AutoKeyNet.WindowsHooks.Hooks.EventArgs;
using AutoKeyNet.WindowsHooks.WindowsEnums;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using AutoKeyNet.WindowsHooks.Helper;

namespace AutoKeyNet.WindowsHooks.Hooks;
/// <summary>
/// Класс хука окон Windows
/// </summary>
internal class WinHook : BaseHook, IHookEvent<WinBaseHookEventArgs>
{
    /// <summary>
    /// Делегат функции обратного вызова
    /// </summary>
    private readonly WinEventDelegate _hookEvent;

    /// <summary>
    /// Конструктор класса хука окон Widows
    /// </summary>
    public WinHook()
    {
        _hookEvent = WinEventProc;
        InitializeHook();
    }

    /// <summary>
    /// Событие возникающее при срабатывании хука
    /// </summary>
    public event EventHandler<WinBaseHookEventArgs>? OnHookEvent;

    /// <summary>
    /// Установить хук
    /// </summary>
    /// <returns>Возвращает идентификатор хука</returns>
    protected override IntPtr SetHook()
    {
        return SetWinEventHook(Constants.EVENT_SYSTEM_FOREGROUND, Constants.EVENT_SYSTEM_FOREGROUND, IntPtr.Zero,
            _hookEvent, 0, 0, Constants.WINEVENT_OUTOFCONTEXT);
    }

    /// <summary>
    /// Функция обратного вызова возникающего при срабатывании хука.
    /// </summary>
    /// <param name="hWinEventHook">Дескриптор функции обработчика событий.</param>
    /// <param name="eventType">Указывает событие, которое произошло.</param>
    /// <param name="hwnd">Дескриптор окна, создающего событие, или ЗНАЧЕНИЕ NULL , если с событием не связано ни окно.</param>
    /// <param name="idObject">Определяет объект, связанный с событием.</param>
    /// <param name="idChild">Определяет, было ли событие активировано объектом или дочерним элементом объекта.param>
    /// <param name="dwEventThread"></param>
    /// <param name="dwmsEventTime">Указывает время создания события в миллисекундах.</param>
    public void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild,
        uint dwEventThread, uint dwmsEventTime)
    {
        WinBaseHookEventArgs winBaseHookEventArgs =
            new WinBaseHookEventArgs(WindowHelper.GetActiveWindowTitle(), eventType, hwnd);
        OnHookEvent?.Invoke(hWinEventHook, winBaseHookEventArgs);
    }

    /// <summary>
    /// Remove of the windows hook
    /// </summary>
    protected override void Unhook() => UnhookWinEvent(HookId);

    #region Windows API functions
    [DllImport("user32.dll")]
    static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc,
        WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

    [DllImport("user32.dll")]
    static extern bool UnhookWinEvent(IntPtr hWinEventHook);

    delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild,
        uint dwEventThread, uint dwmsEventTime);
    #endregion
}
