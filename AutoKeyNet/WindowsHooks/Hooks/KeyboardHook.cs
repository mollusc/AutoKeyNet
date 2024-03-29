﻿using System.Diagnostics;
using System.Runtime.InteropServices;
using AutoKeyNet.WindowsHooks.Helper;
using AutoKeyNet.WindowsHooks.Hooks.EventArgs;
using AutoKeyNet.WindowsHooks.WindowsEnums;
using AutoKeyNet.WindowsHooks.WindowsStruct;
using static AutoKeyNet.WindowsHooks.WinApi.NativeMethods;

namespace AutoKeyNet.WindowsHooks.Hooks;
/// <summary>
///     Class for keyboard hooking
/// </summary>
internal class KeyboardHook : BaseHook, IHookEvent<KeyboardHookEventArgs>
{
    /// <summary>
    ///     Delegate for the callback function
    /// </summary>
    private readonly HookCallbackDelegate _hookCallback;

    /// <summary>
    ///     Constructor of the class for keyboard hooking
    /// </summary>
    public KeyboardHook()
    {
        _hookCallback = LowLevelKeyboardProc;
        InitializeHook();
    }

    /// <summary>
    ///     Event that occurs when a key is pressed or released.
    /// </summary>
    public event EventHandler<KeyboardHookEventArgs>? OnHookEvent;

    /// <summary>
    ///     Set of the keyboard hook
    /// </summary>
    /// <returns>Identifier for the hook</returns>
    protected override nint SetHook()
    {
        using var curProcess = Process.GetCurrentProcess();
        using var curModule = curProcess.MainModule;
        if (curModule != null)
            return SetWindowsHookEx((int)HookType.WH_KEYBOARD_LL, _hookCallback, GetModuleHandle(curModule.ModuleName),
                0);

        throw new NullReferenceException();
    }

    /// <summary>
    ///     Callback function that is called when a Windows hook is executed.
    ///     To prevent sending a pressed key to the system, you need to set KeyboardHookEventArgs.Cancel to true
    /// </summary>
    /// <param name="nCode">A code the hook procedure uses to determine how to process the message</param>
    /// <param name="wParam">The identifier of the keyboard message</param>
    /// <param name="lParam">A pointer to a Windows API KBDLLHOOKSTRUCT structure</param>
    /// <returns>A code the hook procedure uses to determine how to process the message</returns>
    /// <exception cref="InvalidOperationException">
    ///     An exception occurs when there is an error in retrieving
    ///     the KeyboardLowLevelHook struct from the lParam parameter.
    /// </exception>
    private nint LowLevelKeyboardProc(int nCode, nint wParam, nint lParam)
    {
        if (nCode >= HC_ACTION)
        {
            var kbd = (KeyboardLowLevelHook)(Marshal.PtrToStructure(lParam, typeof(KeyboardLowLevelHook)) ??
                                             throw new InvalidOperationException());
            if (kbd.ExtraInfo != KEY_IGNORE)
            {
                var keyboardHookEventArgs = new KeyboardHookEventArgs((Keys)kbd.VirtualKey,
                    kbd.VirtualKey.ToUnicode(), kbd.VirtualKey.ToUnicode(true), wParam, lParam,
                    WindowHelper.GetActiveWindowTitle(),
                    WindowHelper.GetActiveWindowClass(), WindowHelper.GetActiveWindowModuleFileName(),
                    WindowHelper.GetActiveWindowFocusControlName());

                OnHookEvent?.Invoke(kbd.VirtualKey, keyboardHookEventArgs);
                if (keyboardHookEventArgs.Cancel)
                    return 1;
            }
        }

        return CallNextHookEx(HookId, nCode, wParam, lParam);
    }


    /// <summary>
    ///     Remove of the keyboard hook
    /// </summary>
    protected override void Unhook() => UnhookWindowsHookEx(HookId);
}