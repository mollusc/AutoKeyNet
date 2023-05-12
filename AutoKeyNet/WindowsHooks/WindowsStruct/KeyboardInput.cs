using System.Runtime.InteropServices;
using AutoKeyNet.WindowsHooks.WindowsEnums;

namespace AutoKeyNet.WindowsHooks.WindowsStruct;

[StructLayout(LayoutKind.Sequential)]
public struct KeyboardInput
{
    public ushort wVk;
    public ushort wScan;
    public KeyEventFlags dwFlags;
    public int time;
    public nuint dwExtraInfo;
}