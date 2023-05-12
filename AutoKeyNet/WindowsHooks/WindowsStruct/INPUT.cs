using System.Runtime.InteropServices;
using AutoKeyNet.WindowsHooks.WindowsEnums;

namespace AutoKeyNet.WindowsHooks.WindowsStruct;

[StructLayout(LayoutKind.Sequential)]
public struct Input
{
    public InputType type;
    public InputUnion U;
    public static int Size => Marshal.SizeOf(typeof(Input));
}