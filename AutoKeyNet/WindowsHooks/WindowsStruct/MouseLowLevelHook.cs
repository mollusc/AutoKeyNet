using System.Runtime.InteropServices;

namespace AutoKeyNet.WindowsHooks.WindowsStruct;
[StructLayout(LayoutKind.Sequential)]
internal struct MouseLowLevelHook
{
    public Point pt;
    public int mouseData;
    public int flags;
    public int time;
    public UIntPtr dwExtraInfo;
}
