using System;
using System.Runtime.InteropServices;
using System.Text;

namespace AutoKeyNetApp.Helpers
{

    public static class ClipboardHelper
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetClipboardData(uint uFormat);

        [DllImport("user32.dll")]
        private static extern bool IsClipboardFormatAvailable(uint format);

        [DllImport("user32.dll")]
        private static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll")]
        private static extern bool CloseClipboard();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern UIntPtr GlobalSize(IntPtr hMem);

        private const uint CF_UNICODETEXT = 13;

        public static string? GetPlainTextFromClipboard()
        {
            if (!IsClipboardFormatAvailable(CF_UNICODETEXT))
            {
                return null;
            }

            if (!OpenClipboard(IntPtr.Zero))
            {
                return null;
            }

            string? text = null;

            try
            {
                IntPtr hClipboardData = GetClipboardData(CF_UNICODETEXT);

                if (hClipboardData != IntPtr.Zero)
                {
                    UIntPtr size = GlobalSize(hClipboardData);
                    IntPtr ptr = GlobalLock(hClipboardData);

                    if (ptr != IntPtr.Zero)
                    {
                        unsafe
                        {
                            ReadOnlySpan<char> span = new ReadOnlySpan<char>(ptr.ToPointer(), (int)size.ToUInt32() / 2);

                            text = span.ToString();
                        }
                    }

                    GlobalUnlock(hClipboardData);
                }
            }
            finally
            {
                CloseClipboard();
            }

            return text;
        }

    }


}
