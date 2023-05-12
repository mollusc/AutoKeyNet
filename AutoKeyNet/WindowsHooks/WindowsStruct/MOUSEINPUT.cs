using System.Runtime.InteropServices;
using AutoKeyNet.WindowsHooks.WindowsEnums;

namespace AutoKeyNet.WindowsHooks.WindowsStruct;

[StructLayout(LayoutKind.Sequential)]
internal struct MouseInput
{
    internal int dx;
    internal int dy;
    internal int mouseData;
    internal MouseEvents dwFlags;
    internal uint time;
    internal nuint dwExtraInfo;
}