namespace AutoKeyNet.WindowsHooks.Hooks.EventArgs;
/// <summary>
/// Параметры события мыши
/// </summary>
internal class MouseHookEventArgs : BaseHookEventArgs
{
    /// <summary>
    /// Конструктор параметра события мыши
    /// </summary>
    /// <param name="wParam">Дополнительный параметр клавиши</param>
    /// <param name="lParam">Дополнительный параметр клавиши</param>
    /// <param name="mouseData">Дополнительный параметр используемый в случае нажатия клавиш XButton1, XButton2 или использования колеса прокрутки</param>
    /// <param name="windowClass">Класс текущего окна</param>
    /// <param name="windowTitle">Заголовок текущего окна</param>
    /// <param name="windowModule">Название файла exe текущей программы</param>
    /// <param name="windowControl">Название выбранного элемента интерфейса</param>
    public MouseHookEventArgs(IntPtr wParam, IntPtr lParam, int mouseData, string? windowClass, string? windowTitle, string? windowModule, string? windowControl)
    {
        WParam = wParam;
        LParam = lParam;
        MouseData = mouseData;
        WindowClass = windowClass;
        WindowTitle = windowTitle;
        WindowModule = windowModule;
        WindowControl = windowControl;
    }
    /// <summary>
    /// Дополнительный параметр клавиши
    /// </summary>
    public IntPtr WParam { get; }
    /// <summary>
    /// Дополнительный параметр клавиши
    /// </summary>
    public IntPtr LParam { get; }
    /// <summary>
    /// Дополнительный параметр. В случаях с кнопками XButton1, XButton2 или использования колеса прокрутки
    /// </summary>
    public int MouseData { get; }
    /// <summary>
    /// Класс текущего окна
    /// </summary>
    public string? WindowClass { get; }
    /// <summary>
    /// Заголовок текущего окна
    /// </summary>
    public string? WindowTitle { get; }
    /// <summary>
    /// Название файла exe текущей программы
    /// </summary>
    public string? WindowModule { get; }
    /// <summary>
    /// Название выбранного элемента интерфейса
    /// </summary>
    public string? WindowControl { get; }
}
