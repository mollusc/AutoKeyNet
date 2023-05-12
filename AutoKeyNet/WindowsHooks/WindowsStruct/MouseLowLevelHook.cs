using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
