﻿namespace AutoKeyNet.WindowsHooks.WindowsEnums;

/// <summary>
///     Specifies the type of input event to be generated by the SendInput function.
/// </summary>
public enum InputType : uint
{
    /// <summary>
    ///     The event is a mouse event. Use the MOUSEINPUT structure to specify the event details.
    /// </summary>
    INPUT_MOUSE = 0,

    /// <summary>
    ///     The event is a keyboard event. Use the KEYBDINPUT structure to specify the event details.
    /// </summary>
    INPUT_KEYBOARD = 1,

    /// <summary>
    ///     The event is a hardware event. Use the HARDWAREINPUT structure to specify the event details.
    /// </summary>
    INPUT_HARDWARE = 2
}