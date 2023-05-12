namespace AutoKeyNet.WindowsHooks.Hooks.EventArgs;

/// <summary>
///     Event arguments for a mouse hook
/// </summary>
internal class MouseHookEventArgs : BaseHookEventArgs
{
    /// <summary>
    ///     Constructor for mouse hook event arguments.
    /// </summary>
    /// <param name="wParam">The identifier of the mouse message</param>
    /// <param name="lParam">A pointer to an Windows API MSLLHOOKSTRUCT structure</param>
    /// <param name="mouseData">Additional parameter for a mouse message used to detect the XBUTTON1 or XBUTTON2 keys.</param>
    /// <param name="windowTitle">Title of the foreground window</param>
    /// <param name="windowClass">Class of the foreground window</param>
    /// <param name="windowModule">Module name (file *.exe) of the foreground window</param>
    /// <param name="windowControl">Name of the focused control</param>
    public MouseHookEventArgs(nint wParam, nint lParam, int mouseData, string? windowTitle, string? windowClass,
        string? windowModule, string? windowControl)
    {
        WParam = wParam;
        LParam = lParam;
        MouseData = mouseData;
        WindowTitle = windowTitle;
        WindowClass = windowClass;
        WindowModule = windowModule;
        WindowControl = windowControl;
    }

    /// <summary>
    ///     The identifier of the mouse message
    /// </summary>
    public nint WParam { get; }

    /// <summary>
    ///     A pointer to an Windows API MSLLHOOKSTRUCT structure
    /// </summary>
    public nint LParam { get; }

    /// <summary>
    ///     Additional parameter for a mouse message used to detect the XBUTTON1 or XBUTTON2 keys.
    /// </summary>
    public int MouseData { get; }

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

    /// <summary>
    ///     Property to prevent sending mouse event to the system.
    /// </summary>
    public bool Cancel { get; set; }
}