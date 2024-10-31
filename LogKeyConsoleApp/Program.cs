using System.Diagnostics;
using Application = System.Windows.Forms.Application;

namespace LogKeyConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using var mouseHook = new MouseHook();
                using var kbdHook = new KeyboardHook();
                Application.Run();
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
            }
        }
    }
}
