using System.Runtime.InteropServices;
using AutoKeyNet.WindowsHooks.Helper;

namespace AutoKeyNet.WindowsHooks.WindowsStruct;

[StructLayout(LayoutKind.Sequential)]
internal struct GUIThreadInfo
{
    public int cbSize;
    public GuiThreadInfoFlags flags;
    public nint hwndActive;
    public nint hwndFocus;
    public nint hwndCapture;
    public nint hwndMenuOwner;
    public nint hwndMoveSize;
    public nint hwndCaret;
    public System.Drawing.Rectangle rcCaret;
}