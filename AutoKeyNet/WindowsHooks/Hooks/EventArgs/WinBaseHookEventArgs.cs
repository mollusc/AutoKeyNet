namespace AutoKeyNet.WindowsHooks.Hooks.EventArgs;
/// <summary>
/// Параметр события окна Windows
/// </summary>
internal class WinBaseHookEventArgs : BaseHookEventArgs
{
    /// <summary>
    /// Конструктор параметра события окна Windows
    /// </summary>
    /// <param name="windowTitle">Заголовок текущего окна</param>
    /// <param name="eventType">Тип события окна</param>
    /// <param name="hwnd">Идентификатор окна</param>
    public WinBaseHookEventArgs(string? windowTitle, uint eventType, IntPtr hwnd)
    {
        WindowWindowTitle = windowTitle;
        EventType = eventType;
        HWND = hwnd;
    }

    /// <summary>
    /// Заголовок текущего окна
    /// </summary>
    public string? WindowWindowTitle { get; }
    /// <summary>
    /// Тип события окна
    /// </summary>
    public uint EventType { get; }
    /// <summary>
    /// Идентификатор окна
    /// </summary>
    public IntPtr HWND { get; }
}
