namespace AutoKeyNet.WindowsHooks.Hooks.EventArgs;

/// <summary>
///     Interface for calling events with BaseHookEventArgs.
/// </summary>
/// <typeparam name="TBaseEventArgs">Type for event arguments</typeparam>
internal interface IHookEvent<TBaseEventArgs> where TBaseEventArgs : BaseHookEventArgs
{
    /// <summary>
    ///     Event triggered by a Windows hook.
    /// </summary>
    public event EventHandler<TBaseEventArgs>? OnHookEvent;
}