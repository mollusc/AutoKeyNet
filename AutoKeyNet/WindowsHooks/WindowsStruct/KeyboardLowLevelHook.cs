using System.Runtime.InteropServices;
using AutoKeyNet.WindowsHooks.WindowsEnums;

namespace AutoKeyNet.WindowsHooks.WindowsStruct;

[StructLayout(LayoutKind.Sequential)]
internal struct KeyboardLowLevelHook
{
    public VirtualKey vkCode;
    public uint scanCode;
    public KeyboardLowLevelHookFlags flags;
    public uint time;
    public UIntPtr dwExtraInfo;
}