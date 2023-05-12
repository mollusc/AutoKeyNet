namespace AutoKeyNet.WindowsHooks.Hooks;

/// <summary>
///     Base class for Windows API hooking
/// </summary>
internal abstract class BaseHook : IDisposable
{
    /// <summary>
    ///     Identifier for the hook
    /// </summary>
    protected nint HookId;

    /// <summary>
    ///     Disposes of the hook
    /// </summary>
    public void Dispose()
    {
        Unhook();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Finalizer
    /// </summary>
    ~BaseHook()
    {
        Dispose();
    }


    /// <summary>
    ///     Initialization of the hook.
    /// </summary>
    protected void InitializeHook()
    {
        HookId = SetHook();
    }

    /// <summary>
    ///     Set of the hook
    /// </summary>
    /// <returns>Identifier for the hook</returns>
    protected abstract nint SetHook();

    /// <summary>
    ///     Remove of the hook
    /// </summary>
    protected abstract void Unhook();
}