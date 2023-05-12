namespace AutoKeyNet.WindowsHooks.Hooks.EventArgs;

/// <summary>
///     Event arguments for a keyboard hook
/// </summary>
internal class KeyboardHookEventArgs : BaseHookEventArgs
{
    /// <summary>
    ///     Constructor for keyboard hook event arguments.
    /// </summary>
    /// <param name="vkCode">Virtual key code</param>
    /// <param name="letter">Unicode character</param>
    /// <param name="invariantLetter">Unicode character independent of current language layout</param>
    /// <param name="wParam">The identifier of the keyboard message</param>
    /// <param name="lParam">A pointer to a Windows API KBDLLHOOKSTRUCT structure</param>
    /// <param name="windowTitle">Title of the foreground window</param>
    /// <param name="windowClass">Class of the foreground window</param>
    /// <param name="windowModule">Module name (file *.exe) of the foreground window</param>
    /// <param name="windowControl">Name of the focused control</param>
    public KeyboardHookEventArgs(Keys vkCode, char letter, char invariantLetter, nint wParam, nint lParam,
        string? windowTitle, string? windowClass, string? windowModule, string? windowControl)
    {
        VkCode = vkCode;
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
    ///     Virtual key code
    /// </summary>
    public Keys VkCode { get; }

    /// <summary>
    ///     Unicode character
    /// </summary>
    public char Letter { get; }

    /// <summary>
    ///     Unicode character independent of current language layout
    /// </summary>
    public char InvariantLetter { get; }

    /// <summary>
    ///     The identifier of the keyboard message
    /// </summary>
    public nint WParam { get; }

    /// <summary>
    ///     A pointer to a Windows API KBDLLHOOKSTRUCT structure
    /// </summary>
    public nint LParam { get; }

    /// <summary>
    ///     Property to prevent sending pressed key to the system.
    /// </summary>
    public bool Cancel { get; set; }

    /// <summary>
    ///     Class of the foreground window
    /// </summary>
    public string? WindowClass { get; }

    /// <summary>
    ///     Title of the foreground window
    /// </summary>
    public string? WindowTitle { get; }

    /// <summary>
    ///     Module name (file *.exe) of the foreground window
    /// </summary>
    public string? WindowModule { get; }

    /// <summary>
    ///     Name of the focused control
    /// </summary>
    public string? WindowControl { get; }
}