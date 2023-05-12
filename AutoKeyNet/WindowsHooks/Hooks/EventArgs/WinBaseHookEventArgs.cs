namespace AutoKeyNet.WindowsHooks.Hooks.EventArgs;

/// <summary>
///     Event arguments for a Windows hook
/// </summary>
internal class WinBaseHookEventArgs : BaseHookEventArgs
{
    /// <summary>
    ///     Constructor for Windows hook event arguments.
    /// </summary>
    /// <param name="windowTitle">Title of the foreground window</param>
    /// <param name="eventType">Event type</param>
    /// <param name="handle">Identifier of a window</param>
    public WinBaseHookEventArgs(string? windowTitle, uint eventType, nint handle)
    {
        WindowWindowTitle = windowTitle;
        EventType = eventType;
        Handle = handle;
    }

    /// <summary>
    ///     Title of the foreground window
    /// </summary>
    public string? WindowWindowTitle { get; }

    /// <summary>
    ///     Event type
    /// </summary>
    public uint EventType { get; }

    /// <summary>
    ///     Identifier of a window
    /// </summary>
    public nint Handle { get; }
}