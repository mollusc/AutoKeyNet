namespace AutoKeyNet.WindowsHooks;

internal static class Constants
{
    public const uint KEY_IGNORE = 0xFFC3D44F;
    public const int HC_ACTION = 0;
    public const uint WINEVENT_OUTOFCONTEXT = 0;
    public const uint EVENT_SYSTEM_FOREGROUND = 3;
        
    public const uint MAPVK_VK_TO_VSC = 0x00;
    public const uint MAPVK_VSC_TO_VK = 0x01;
    public const uint MAPVK_VK_TO_CHAR = 0x02;
    public const uint MAPVK_VSC_TO_VK_EX = 0x03;
    public const uint MAPVK_VK_TO_VSC_EX = 0x04;
}