using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AutoKeyNet.WindowsHooks.WindowsStruct;

namespace AutoKeyNet.WindowsHooks.WinApi
{
    public static class NativeMethods
    {
        public static Task SendInputAsync(Input[] inputs) => Task.Run(() => SendInput(inputs));
        public static void SendInput(Input[] inputs) => SendInput((uint)inputs.Length, inputs, Input.Size);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint SendInput(uint nInputs, Input[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public const int SW_MAXIMIZE = 3;
    }
}
