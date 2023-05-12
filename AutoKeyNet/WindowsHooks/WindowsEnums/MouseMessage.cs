﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoKeyNet.WindowsHooks.WindowsEnums;
internal enum MouseMessage
{
    WM_MOUSEMOVE = 0x200,
    WM_LBUTTONDOWN = 0x201,
    WM_LBUTTONUP = 0x202,
    WM_LBUTTONDBLCLK = 0x203,
    WM_RBUTTONDOWN = 0x204,
    WM_RBUTTONUP = 0x205,
    WM_RBUTTONDBLCLK = 0x206,
    WM_MBUTTONDOWN = 0x207,
    WM_MBUTTONUP = 0x208,
    WM_MBUTTONDBLCLK = 0x209,
    WM_MOUSEWHEEL = 0x20A,
    WM_MOUSEHWHEEL = 0x20E,
    WM_XBUTTONDOWN = 0x020B,
    WM_XBUTTONUP = 0x020C
}
