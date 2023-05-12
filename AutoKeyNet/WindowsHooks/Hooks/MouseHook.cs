using System.Diagnostics;
using System.Runtime.InteropServices;
using AutoKeyNet.WindowsHooks.Helper;
using AutoKeyNet.WindowsHooks.Hooks.EventArgs;
using AutoKeyNet.WindowsHooks.WindowsEnums;
using AutoKeyNet.WindowsHooks.WindowsStruct;

namespace AutoKeyNet.WindowsHooks.Hooks;
/// <summary>
/// Класс хука мыши
/// </summary>
internal class MouseHook : BaseHook, IHookEvent<MouseHookEventArgs>
{
    /// <summary>
    /// Делегат функции обратного вызова
    /// </summary>
    private readonly HookCallbackDelegate _hookCallback;

    /// <summary>
    /// Конструктор класса хука мыши
    /// </summary>
    public MouseHook()
    {
        _hookCallback = LowLevelMouseProc;
        InitializeHook();
    }

    /// <summary>
    /// Событие возникающее при срабатывании хука
    /// </summary>
    public event EventHandler<MouseHookEventArgs>? OnHookEvent;

    /// <summary>
    /// Установить хук
    /// </summary>
    /// <returns>Возвращает идентификатор хука</returns>
    protected override IntPtr SetHook()
    {
        using Process curProcess = Process.GetCurrentProcess();
        using ProcessModule? curModule = curProcess.MainModule;
        if (curModule != null)
        {
            return SetWindowsHookEx((int)HookType.WH_MOUSE_LL, _hookCallback, GetModuleHandle(curModule.ModuleName),
                0);
        }

        throw new NullReferenceException();
    }

    /// <summary>
    /// Функция обратного вызова возникающего при срабатывании хука.
    /// </summary>
    /// <param name="nCode">Код, который используется для определения способа обработки сообщения</param>
    /// <param name="wParam">Устанавливает идентификатор сообщения мыши</param>
    /// <param name="lParam">Указатель на структуру</param>
    /// <returns>Код, который используется для определения способа обработки сообщения</returns>
    /// <exception cref="InvalidOperationException"></exception>
    private IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= Constants.HC_ACTION)
        {
            MouseLowLevelHook hookStruct = (MouseLowLevelHook)(Marshal.PtrToStructure(lParam, typeof(MouseLowLevelHook)) ??
                                                         throw new InvalidOperationException());
            MouseHookEventArgs mouseHookEventArgs = new MouseHookEventArgs(wParam, lParam,
                hookStruct.mouseData >> 16,
                WindowHelper.GetActiveWindowClass(), WindowHelper.GetActiveWindowTitle(),
                WindowHelper.GetActiveWindowModuleFileName(),
                WindowHelper.GetActiveWindowFocusControlName());
            OnHookEvent?.Invoke(wParam, mouseHookEventArgs);
        }

        return CallNextHookEx(HookId, nCode, wParam, lParam);
    }

    /// <summary>
    /// Remove of the mouse hook
    /// </summary>
    protected override void Unhook() => UnhookWindowsHookEx(HookId);

    #region Windows API functions
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, HookCallbackDelegate lpfn, IntPtr hMod,
        uint dwThreadId);

    private delegate IntPtr HookCallbackDelegate(int nCode, IntPtr wParam, IntPtr lParam);
    #endregion
}