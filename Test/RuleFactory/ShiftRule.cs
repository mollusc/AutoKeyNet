using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AutoKeyNet.WindowsHooks.Rule;
using AutoKeyNet.WindowsHooks.WindowsEnums;
using Microsoft.Office.Interop.Outlook;
using Action = System.Action;

namespace AutoKeyNetApp.RuleFactory
{
    internal class ShiftRule : BaseRuleFactory
    {
        /// <summary>
        ///     Identifier for the hook
        /// </summary>
        protected nint HookId;
        public ShiftRule()
        {
            _hookCallback = LowLevelKeyboardProc;
        }
        [DllImport("user32.dll")]
        private static extern bool PostMessage(int hhwnd, uint msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32.dll")]
        private static extern IntPtr LoadKeyboardLayout(string pwszKLID, uint Flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern nint SetWindowsHookEx(int idHook, HookCallbackDelegate lpfn, nint hMod, uint dwThreadId);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern nint GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern nint CallNextHookEx(nint hhk, int nCode, nint wParam, nint lParam);

        private static uint WM_INPUTLANGCHANGEREQUEST = 0x0050;
        private static int HWND_BROADCAST = 0xffff;
        private static int WH_KEYBOARD_LL = 13;
        internal const uint KEY_IGNORE = 0xFFC3D44F;
        internal const uint HC_ACTION = 0;
        private static string en_US = "00000409";
        private static string ru_RU = "00000419";
        private static int WM_KEYDOWN = 0x0100;
        private static int WM_KEYUP = 0x0101;
        private static ushort LSHIFT = 0xA0;
        private static ushort RSHIFT = 0xA1;

        private static uint KLF_ACTIVATE = 1;
        /// <summary>
        ///     Delegate for the callback function
        /// </summary>
        private readonly HookCallbackDelegate _hookCallback;
        internal delegate nint HookCallbackDelegate(int nCode, nint wParam, nint lParam);

        public override List<BaseRuleRecord> Create()
        {
            SetHook();
            var rules = new List<BaseRuleRecord>
            {
                new HotKeyRuleRecord("{LSHIFT DOWN}", ChangeLanguage(en_US)), // Set text as underscore
                new HotKeyRuleRecord("{RSHIFT DOWN}", ChangeLanguage(ru_RU)) // Set text as underscore
            };
            return rules;
        }
        private static Action ChangeLanguage(string pwszKLID)
        {
            return () =>
            {

                PostMessage(HWND_BROADCAST, WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero,
                    LoadKeyboardLayout(pwszKLID, KLF_ACTIVATE));
            };
        }

        /// <summary>
        ///     Set of the keyboard hook
        /// </summary>
        /// <returns>Identifier for the hook</returns>
        protected nint SetHook()
        {
            using var curProcess = Process.GetCurrentProcess();
            using var curModule = curProcess.MainModule;
            if (curModule != null)
                return SetWindowsHookEx(WH_KEYBOARD_LL, _hookCallback, GetModuleHandle(curModule.ModuleName), 0);

            throw new NullReferenceException();
        }

        internal struct KeyboardLowLevelHook
        {
            /// <summary>
            ///     Specifies a virtual-key code. The code must be a value in the range 1 to 254.
            /// </summary>
            public VirtualKey VirtualKey;

            /// <summary>
            ///     Specifies a hardware scan code for the key.
            /// </summary>
            public uint ScanCode;

            /// <summary>
            ///     Specifies various aspects of the event, such as extended key status, context code, and transition state.
            /// </summary>
            public KeyboardLowLevelHookFlags Flags;

            /// <summary>
            ///     Specifies the time stamp for this message.
            /// </summary>
            public uint Time;

            /// <summary>
            ///     Specifies additional information associated with the message.
            /// </summary>
            public nuint ExtraInfo;
        }

        private nint LowLevelKeyboardProc(int nCode, nint wParam, nint lParam)
        {
            if (nCode >= HC_ACTION)
            {
                var kbd = (KeyboardLowLevelHook)(Marshal.PtrToStructure(lParam, typeof(KeyboardLowLevelHook)) ?? throw new InvalidOperationException());
                if (kbd.ExtraInfo != KEY_IGNORE)
                    OnKeyboardHookEvent(kbd.VirtualKey, wParam);
            }

            return CallNextHookEx(HookId, nCode, wParam, lParam);
        }
        /// <summary>
        ///     Method for handling keyboard events
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private void OnKeyboardHookEvent(VirtualKey key, nint wParam)
        {
            if (wParam == WM_KEYDOWN)
            {
                return;
            }

            if (wParam == WM_KEYUP)
            {
                if(key == VirtualKey.LSHIFT)
                    PostMessage(HWND_BROADCAST, WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero,
                        LoadKeyboardLayout(en_US, KLF_ACTIVATE));

                if(key == VirtualKey.RSHIFT)
                    PostMessage(HWND_BROADCAST, WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero,
                        LoadKeyboardLayout(ru_RU, KLF_ACTIVATE));
            }
        }
    }
}
