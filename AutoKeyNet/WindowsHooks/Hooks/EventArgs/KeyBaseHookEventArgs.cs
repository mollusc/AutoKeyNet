namespace AutoKeyNet.WindowsHooks.Hooks.EventArgs;
/// <summary>
/// Параметры события вызываемые нажатием клавиши клавиатуры
/// </summary>
internal class KeyBaseHookEventArgs : BaseHookEventArgs
{
    /// <summary>
    /// Конструктор параметра события вызываемого нажатием клавиши клавиатуры
    /// </summary>
    /// <param name="vCode">Виртуальный код клавиши</param>
    /// <param name="letter">Печатный символ клавиши с учетом текущего языка и клавиши Shift</param>
    /// <param name="invariantLetter"></param>
    /// <param name="wParam">Дополнительный параметр клавиши</param>
    /// <param name="lParam">Дополнительный параметр клавиши</param>
    /// <param name="windowClass">Класс текущего окна</param>
    /// <param name="windowTitle">Заголовок текущего окна</param>
    /// <param name="windowModule">Название файла exe текущей программы</param>
    /// <param name="windowControl">Название выбранного элемента интерфейса</param>
    public KeyBaseHookEventArgs(Keys vCode, char letter, char invariantLetter, IntPtr wParam, IntPtr lParam, string? windowClass, string? windowTitle, string? windowModule, string? windowControl)
    {
        VCode = vCode;
        Letter = letter;
        InvariantLetter = invariantLetter;
        WParam = wParam;
        LParam = lParam;
        WindowClass = windowClass;
        WindowTitle = windowTitle;
        WindowModule = windowModule;
        WindowControl = windowControl;
        Cancel = false;

    }

    /// <summary>
    /// Виртуальный код клавиши
    /// </summary>
    public Keys VCode { get; }
    /// <summary>
    /// Печатный символ клавиши с учетом текущего языка и клавиши Shift
    /// </summary>
    public char Letter { get; }
    /// <summary>
    /// Печатный символ клавиши с учетом текущего языка и клавиши Shift
    /// </summary>
    public char InvariantLetter { get; }
    /// <summary>
    /// Дополнительный параметр клавиши
    /// </summary>
    public IntPtr WParam { get; }
    /// <summary>
    /// Дополнительный параметр клавиши
    /// </summary>
    public IntPtr LParam { get; }
    /// <summary>
    /// Параметр позволяющий прекратить передачу клавишу системе
    /// </summary>
    public bool Cancel { get; set; }
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
