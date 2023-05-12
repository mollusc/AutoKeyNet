namespace AutoKeyNet.WindowsHooks.WindowsEnums;

/// <summary>
///     Specifies the flags that can be used with the GuiThreadInfo function.
/// </summary>
[Flags]
internal enum GuiThreadInfoFlags
{
    /// <summary>
    ///     The caret is blinking.
    /// </summary>
    GUI_CARETBLINKING = 0x00000001,

    /// <summary>
    ///     The thread is in menu mode.
    /// </summary>
    GUI_INMENUMODE = 0x00000004,

    /// <summary>
    ///     The thread is in move or size mode.
    /// </summary>
    GUI_INMOVESIZE = 0x00000002,

    /// <summary>
    ///     The thread is in popup menu mode.
    /// </summary>
    GUI_POPUPMENUMODE = 0x00000010,

    /// <summary>
    ///     The thread is in system menu mode.
    /// </summary>
    GUI_SYSTEMMENUMODE = 0x00000008
}