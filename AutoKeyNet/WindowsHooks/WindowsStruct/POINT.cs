using System.Runtime.InteropServices;

namespace AutoKeyNet.WindowsHooks.WindowsStruct;
/// <summary>
/// Represents an ordered pair of integer x and y coordinates that define a point in a two-dimensional plane.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct Point
{
    /// <summary>
    /// Gets or sets the x-coordinate of this <see cref="Point"/>.
    /// </summary>
    public int X;

    /// <summary>
    /// Gets or sets the y-coordinate of this <see cref="Point"/>.
    /// </summary>
    public int Y;

    /// <summary>
    /// Initializes a new instance of the <see cref="Point"/> structure with the specified x and y coordinates.
    /// </summary>
    /// <param name="x">The x-coordinate of the new <see cref="Point"/>.</param>
    /// <param name="y">The y-coordinate of the new <see cref="Point"/>.</param>
    public Point(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    /// <summary>
    /// Implicitly converts the specified <see cref="Point"/> to a <see cref="System.Drawing.Point"/>.
    /// </summary>
    /// <param name="p">The <see cref="Point"/> to convert.</param>
    /// <returns>The equivalent <see cref="System.Drawing.Point"/>.</returns>
    public static implicit operator System.Drawing.Point(Point p)
    {
        return new System.Drawing.Point(p.X, p.Y);
    }

    /// <summary>
    /// Implicitly converts the specified <see cref="System.Drawing.Point"/> to a <see cref="Point"/>.
    /// </summary>
    /// <param name="p">The <see cref="System.Drawing.Point"/> to convert.</param>
    /// <returns>The equivalent <see cref="Point"/>.</returns>
    public static implicit operator Point(System.Drawing.Point p)
    {
        return new Point(p.X, p.Y);
    }

    /// <summary>
    /// Returns a string that represents the current <see cref="Point"/> structure.
    /// </summary>
    /// <returns>A string that represents the current <see cref="Point"/> structure.</returns>
    public override string ToString()
    {
        return $"X: {X}, Y: {Y}";
    }
}

