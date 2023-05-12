namespace AutoKeyNet.WindowsHooks.Hooks.EventArgs;

/// <summary>
/// Интерфейс подразумевающий вызов события с параметром типа BaseHookEventArgs
/// </summary>
/// <typeparam name="TBaseEventArgs">Тип параметра события</typeparam>
internal interface IHookEvent<TBaseEventArgs> where TBaseEventArgs : BaseHookEventArgs
{
    /// <summary>
    /// Событие возникающее при срабатывании хука
    /// </summary>
    public event EventHandler<TBaseEventArgs>? OnHookEvent;
}