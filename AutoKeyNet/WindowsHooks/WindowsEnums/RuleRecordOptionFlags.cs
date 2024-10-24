namespace AutoKeyNet.WindowsHooks.WindowsEnums;

/// <summary>
///     Option for hot key rule
/// </summary>
[Flags]
public enum HotKeyRuleRecordOptionFlags
{
    None = 0,

    /// <summary>
    ///     Suppress the prefix key down event until the event key up. This is necessary to suppress the native behavior of the
    ///     prefix key, when the prefix key is used in a hotkey combination as the first key.
    /// </summary>
    SuppressNativeBehaviorForPrefixKey = 1 << 0,
    SuppressNativeBehavior = 1 << 1,
}