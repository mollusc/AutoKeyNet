using System;
using System.Runtime.InteropServices;
using System.Text;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Controls; // For HWND, POINT, RECT
using Windows.Win32.UI.WindowsAndMessaging;
using Microsoft.VisualStudio.Services.Organization; // For tooltip-related constants and TOOLINFO
using static Windows.Win32.PInvoke;

namespace AutoKeyNetApp.Helper
{
    public static class TooltipHelper
    {
        //public const int TTS_ALWAYSTIP = 0x01;

        public static unsafe void ShowTooltip(string text)
        {
            // Get the current cursor position
            GetCursorPos(out var cursorPos);
            HWND hTooltipWnd;
            fixed (char* className = TOOLTIPS_CLASS)
            {
                hTooltipWnd = CreateWindowEx(
                    WINDOW_EX_STYLE.WS_EX_TOPMOST,
                    className,
                    null,
                    WINDOW_STYLE.WS_POPUP | (WINDOW_STYLE)TTS_ALWAYSTIP,
                    CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT,
                    (HWND)nint.Zero,
                    (HMENU)nint.Zero,
                    (HINSTANCE)nint.Zero,
                    nint.Zero.ToPointer()
                );

                if (hTooltipWnd.Value == IntPtr.Zero)
                    throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());

            }
            // Create the tooltip window
            // Set up TOOLINFO structure for the tooltip
            TTTOOLINFOW toolInfo;
            fixed (char* pText = text)
            {
                toolInfo = new TTTOOLINFOW
                {
                    cbSize = (uint)Marshal.SizeOf(typeof(TTTOOLINFOW)),
                    uFlags = (TOOLTIP_FLAGS.TTF_SUBCLASS | TOOLTIP_FLAGS.TTF_TRANSPARENT),
                    hwnd = HWND.Null,
                    rect = new RECT { left = cursorPos.X, top = cursorPos.Y, right = cursorPos.X + 1, bottom = cursorPos.Y + 1 },
                    lpszText = pText
                };

            }

            IntPtr lParam = Marshal.AllocHGlobal(Marshal.SizeOf(toolInfo));
            Marshal.StructureToPtr(toolInfo, lParam, false);
            // Add the tool to the tooltip control
            SendMessage(hTooltipWnd, TTM_ADDTOOL, 0, lParam);

            // Position the tooltip at the cursor position
            SendMessage(hTooltipWnd, TTM_TRACKPOSITION, 0, (IntPtr)((cursorPos.Y << 16) | cursorPos.X));

            // Activate the tooltip
            SendMessage(hTooltipWnd, TTM_TRACKACTIVATE, 1, lParam);
            //SendMessage(hTooltipWnd, TTM_ADDTOOL, 0, lParam);
            //SendMessage(hTooltipWnd, TTM_SETMAXTIPWIDTH, 0, new IntPtr(200));
            //SendMessage(hTooltipWnd, TTM_SETTITLE, 1, Marshal.StringToHGlobalUni("Tooltip Title"));
            //SendMessage(hTooltipWnd, TTM_ACTIVATE, 1, IntPtr.Zero);
        }
    }
}
