using AutoKeyNet.WindowsHooks.WindowsStruct;

namespace AutoKeyNet.WindowsHooks.Hooks.EventArgs;

/// <summary>
///     Event arguments for a  hook
/// </summary>
internal class HookEventArgs : BaseHookEventArgs
{
    /// <summary>
    ///     Constructor for  hook event arguments.
    /// </summary>
    /// <param name="input">Input from keyboard or mouse</param>
    /// <param name="windowTitle">Title of the foreground window</param>
    /// <param name="windowClass">Class of the foreground window</param>
    /// <param name="windowModule">Module name (file *.exe) of the foreground window</param>
    /// <param name="windowControl">Name of the focused control</param>
    public HookEventArgs(Input input,
        string? windowTitle, string? windowClass, string? windowModule, string? windowControl)
    {
        Input = input;
        WindowClass = windowClass;
        WindowTitle = windowTitle;
        WindowModule = windowModule;
        WindowControl = windowControl;
        Cancel = false;
    }

    /// <summary>
    /// Input from keyboard or mouse
    /// </summary>
    public Input Input { get; set; }

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