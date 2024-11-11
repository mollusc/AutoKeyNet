using System.Diagnostics;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.WindowsAndMessaging;
using static Windows.Win32.PInvoke;
using AutoKeyNet.Helper;
using AutoKeyNet.RuleRecords;

namespace AutoKeyNetApp.RuleFactory;

/// <summary>
/// Create hot keys for start programs
/// </summary>
internal class HotKeyProgramRuleFactory : BaseRuleFactory
{
    public override List<BaseRuleRecord> Create()
    {
        return
        [
            new HotKeyRuleRecord([
                    VIRTUAL_KEY.VK_XBUTTON1.ToInput(0, Constants.KEY_SUPPRESS_NATIVE_BEHAVIOUR),
                    VIRTUAL_KEY.VK_XBUTTON1.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP,
                        Constants.KEY_SUPPRESS_NATIVE_BEHAVIOUR)
                ],
                [
                    MOUSE_EVENT_FLAGS.MOUSEEVENTF_XDOWN.ToInput(Constants.XBUTTON1),
                    MOUSE_EVENT_FLAGS.MOUSEEVENTF_XUP.ToInput(Constants.XBUTTON1)
                ]),


            //new HotKeyRuleRecord([
            //        VIRTUAL_KEY.VK_XBUTTON1.ToInput(0, Constants.KEY_SUPPRESS_NATIVE_BEHAVIOUR),
            //        VIRTUAL_KEY.VK_X.ToInput(0, Constants.KEY_SUPPRESS_NATIVE_BEHAVIOUR),
            //        VIRTUAL_KEY.VK_X.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP,
            //            Constants.KEY_SUPPRESS_NATIVE_BEHAVIOUR),
            //        VIRTUAL_KEY.VK_XBUTTON1.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP,
            //            Constants.KEY_SUPPRESS_NATIVE_BEHAVIOUR),
            //    ],
            //    () => { }),

            new HotKeyRuleRecord([
                    VIRTUAL_KEY.VK_XBUTTON1.ToInput(0, Constants.KEY_SUPPRESS_NATIVE_BEHAVIOUR),
                    VIRTUAL_KEY.VK_T.ToInput(0, Constants.KEY_SUPPRESS_NATIVE_BEHAVIOUR)
                ],
                () => RunProgram("TOTALCMD64", "C:\\Program Files (x86)\\Total Commander\\TOTALCMD64.EXE")),

            new HotKeyRuleRecord([
                    VIRTUAL_KEY.VK_XBUTTON1.ToInput(0,
                        Constants.KEY_SUPPRESS_NATIVE_BEHAVIOUR),
                    VIRTUAL_KEY.VK_X.ToInput(0,
                        Constants.KEY_SUPPRESS_NATIVE_BEHAVIOUR)
                ],
                () => RunProgram("iexplore", "C:\\Program Files\\Internet Explorer\\iexplore.exe")),
            new HotKeyRuleRecord([
                    VIRTUAL_KEY.VK_XBUTTON1.ToInput(0,
                        Constants.KEY_SUPPRESS_NATIVE_BEHAVIOUR),
                    VIRTUAL_KEY.VK_E.ToInput(0,
                        Constants.KEY_SUPPRESS_NATIVE_BEHAVIOUR)
                ],
                () => RunProgram("EXCEL", "c:\\Program Files\\Microsoft Office\\Office15\\EXCEL.EXE")),
            new HotKeyRuleRecord([
                    VIRTUAL_KEY.VK_XBUTTON1.ToInput(0,
                        Constants.KEY_SUPPRESS_NATIVE_BEHAVIOUR),
                    VIRTUAL_KEY.VK_W.ToInput(0,
                        Constants.KEY_SUPPRESS_NATIVE_BEHAVIOUR)
                ],
                () => RunProgram("WINWORD", "c:\\Program Files\\Microsoft Office\\Office15\\WINWORD.EXE")),
            new HotKeyRuleRecord([
                    VIRTUAL_KEY.VK_XBUTTON1.ToInput(0,
                        Constants.KEY_SUPPRESS_NATIVE_BEHAVIOUR),
                    VIRTUAL_KEY.VK_S.ToInput(0,
                        Constants.KEY_SUPPRESS_NATIVE_BEHAVIOUR)
                ],
                () => RunProgram("OUTLOOK", "C:\\Program Files\\Microsoft Office\\Office15\\OUTLOOK.EXE")),
        ];
    }

    /// <summary>
    /// Run program
    /// </summary>
    /// <param name="processName">Process name of the program</param>
    /// <param name="pathToProgram">The absolute path to the executable file of the program</param>
    private static void RunProgram(string processName, string pathToProgram)
    {
        bool result = false;
        var processes = Process.GetProcessesByName(processName);
        var process = processes.FirstOrDefault();
        if (process is not null)
        {
            Debug.WriteLine($"    DEBUG: Set as foreground:'{process.ProcessName}'");

            result |= ShowWindow((HWND)process.MainWindowHandle, SHOW_WINDOW_CMD.SW_MAXIMIZE);
            result |= SetForegroundWindow((HWND)process.MainWindowHandle);
        }

        if (processes.Length == 0 || !result)
        {
            Debug.WriteLine($"    DEBUG: Start program:'{pathToProgram}'");
            Process.Start(pathToProgram);
        }
    }
}
