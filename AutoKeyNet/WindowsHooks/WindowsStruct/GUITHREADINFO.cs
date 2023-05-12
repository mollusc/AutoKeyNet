using System.Runtime.InteropServices;
using AutoKeyNet.WindowsHooks.Helper;
using static AutoKeyNet.WindowsHooks.Helper.WindowHelper;

namespace AutoKeyNet.WindowsHooks.WindowsStruct;

[StructLayout(LayoutKind.Sequential)]
internal struct GUIThreadInfo
{
    public int cbSize;
    public GuiThreadInfoFlags flags;
    public IntPtr hwndActive;
    public IntPtr hwndFocus;
    public IntPtr hwndCapture;
    public IntPtr hwndMenuOwner;
    public IntPtr hwndMoveSize;
    public IntPtr hwndCaret;
    public System.Drawing.Rectangle rcCaret;
}