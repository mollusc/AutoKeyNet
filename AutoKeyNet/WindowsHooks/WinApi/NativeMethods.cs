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
        public static async Task SendInputAsync(Input[] inputs) => await Task.Run(() => SendInput(inputs));
        public static void SendInput(Input[] inputs) => SendInput((uint)inputs.Length, inputs, Input.Size);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint SendInput(uint nInputs, Input[] pInputs, int cbSize);
    }
}
