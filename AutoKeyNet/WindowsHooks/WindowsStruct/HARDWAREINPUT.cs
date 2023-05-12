using System.Runtime.InteropServices;

namespace AutoKeyNet.WindowsHooks.WindowsStruct;

[StructLayout(LayoutKind.Sequential)]
public struct HardwareInput
{
    public uint uMsg;
    public ushort wParamL;
    public ushort wParamH;
}