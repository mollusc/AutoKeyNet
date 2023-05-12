namespace AutoKeyNet.WindowsHooks.WindowsEnums;

[Flags]
public enum HotKeyRuleRecordOptionFlags
{
    None = 0,
    /// <summary>
    /// Подавление prefix key до момента key up.Требуется для того что бы устранить native behavior of the prefix key
    /// </summary>
    DelayKeyDownToKyeUpForPrefixKey = 1 << 0,
}