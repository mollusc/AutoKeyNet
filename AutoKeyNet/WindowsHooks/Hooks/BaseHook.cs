using Windows.Win32.UI.WindowsAndMessaging;
using AutoKeyNet.WindowsHooks.Hooks.EventArgs;

namespace AutoKeyNet.WindowsHooks.Hooks;

/// <summary>
///     Base class for Windows API hooking
/// </summary>
internal abstract class BaseHook<TBaseEventArgs> : IDisposable where TBaseEventArgs : BaseHookEventArgs
{
    public const uint KEY_IGNORE = 0xFFC3D44F;
    public const uint KEY_SUPRESS_NATIVE_BEHAVIOUR = 0xFFC3D450;

    /// <summary>
    ///     Identifier for the hook
    /// </summary>
    protected HHOOK HookId;

    /// <summary>
    ///     Disposes of the hook
    /// </summary>
    public void Dispose()
    {
        Unhook();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Event that is triggered when hook trigger.
    /// </summary>
    public event EventHandler<TBaseEventArgs>? HookEvent;

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
    protected abstract HHOOK SetHook();

    /// <summary>
    ///     Remove of the hook
    /// </summary>
    protected abstract void Unhook();

    /// <summary>
    ///  Invocation of the event handler
    /// </summary>
    protected virtual void OnHookEvent(TBaseEventArgs e)
    {
        HookEvent?.Invoke(this, e);
    }
}