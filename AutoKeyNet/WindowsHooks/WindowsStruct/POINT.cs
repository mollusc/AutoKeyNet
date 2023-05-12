using System.Runtime.InteropServices;

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
