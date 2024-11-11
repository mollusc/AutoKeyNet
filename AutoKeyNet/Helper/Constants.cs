using System.Diagnostics.CodeAnalysis;

namespace AutoKeyNet.Helper;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class Constants
{
    public const uint KEY_IGNORE = 0xFFC3D44F;
    public const uint KEY_SUPPRESS_NATIVE_BEHAVIOUR = 0xFFC3D450;
    public const uint XBUTTON1 = 0x0001 << 16;
    public const uint XBUTTON2 = 0x0002 << 16;
}