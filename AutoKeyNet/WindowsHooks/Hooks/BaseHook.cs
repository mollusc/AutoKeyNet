using AutoKeyNet.WindowsHooks.Hooks.EventArgs;
using System.Runtime.InteropServices;

namespace AutoKeyNet.WindowsHooks.Hooks;
/// <summary>
/// Base class for Windows API hooking
/// </summary>
internal abstract class BaseHook : IDisposable
{
    /// <summary>
    /// Identifier for the hook
    /// </summary>
    protected IntPtr HookId;

    /// <summary>
    /// Disposes of the hook
    /// </summary>
    public void Dispose()
    {
        Unhook();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Finalizer
    /// </summary>
    ~BaseHook()
    {
        Dispose();
    }


    /// <summary>
    /// Initialization of the hook.
    /// </summary>
    protected void InitializeHook()
    {
        HookId = SetHook();
    }

    /// <summary>
    /// Set of the hook
    /// </summary>
    /// <returns>Identifier for the hook</returns>
    protected abstract IntPtr SetHook();

    /// <summary>
    /// Remove of the hook
    /// </summary>
    protected abstract void Unhook();

    #region Windows API functions

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    protected static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    protected static extern IntPtr GetModuleHandle(string lpModuleName);

    #endregion
}