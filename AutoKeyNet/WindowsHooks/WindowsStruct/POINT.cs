using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AutoKeyNet.WindowsHooks.WindowsStruct;
[StructLayout(LayoutKind.Sequential)]
internal struct Point
{
    public int X;
    public int Y;

    public Point(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public static implicit operator System.Drawing.Point(Point p)
    {
        return new System.Drawing.Point(p.X, p.Y);
    }

    public static implicit operator Point(System.Drawing.Point p)
    {
        return new Point(p.X, p.Y);
    }

    public override string ToString()
    {
        return $"X: {X}, Y: {Y}";
    }
}
