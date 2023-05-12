using System.Runtime.InteropServices;
using AutoKeyNet.WindowsHooks.WindowsEnums;

namespace AutoKeyNet.WindowsHooks.WindowsStruct;

/// <summary>
///     Contains information about the GUI thread.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct GuiThreadInfo
{
    /// <summary>
    ///     The size of the structure, in bytes.
    /// </summary>
    public int Size;

    /// <summary>
    ///     Flags that indicate which members of this structure are valid.
    /// </summary>
    public GuiThreadInfoFlags Flags;

    /// <summary>
    ///     A handle to the active window on the thread.
    /// </summary>
    public nint ActiveWindowHandle;

    /// <summary>
    ///     A handle to the window that has the keyboard focus.
    /// </summary>
    public nint FocusedWindowHandle;

    /// <summary>
    ///     A handle to the window that has captured the mouse.
    /// </summary>
    public nint CapturedWindowHandle;

    /// <summary>
    ///     A handle to the window that owns any active menus.
    /// </summary>
    public nint MenuWindowHandle;

    /// <summary>
    ///     A handle to the window that is currently being moved or sized.
    /// </summary>
    public nint MovingWindowHandle;

    /// <summary>
    ///     A handle to the window that has the caret.
    /// </summary>
    public nint CaretWindowHandle;

    /// <summary>
    ///     A rectangle that specifies the caret's position and dimensions, in screen coordinates.
    /// </summary>
    public Rectangle CaretRect;
}