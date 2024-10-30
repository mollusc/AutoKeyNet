using System.Diagnostics;
using AutoKeyNet.WindowsHooks.Helper;
using AutoKeyNet.WindowsHooks.Rule;
using AutoKeyNet.WindowsHooks.WinApi;
using AutoKeyNet.WindowsHooks.WindowsEnums;

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
                    VirtualKey.XBUTTON1.ToInput(KeyEventFlags.KEYDOWN,
                        AutoKeyNet.WindowsHooks.WinApi.NativeMethods.KEY_SUPRESS_NATIVE_BEHAVIOUR),
                    VirtualKey.XBUTTON1.ToInput(KeyEventFlags.KEYUP,
                        AutoKeyNet.WindowsHooks.WinApi.NativeMethods.KEY_SUPRESS_NATIVE_BEHAVIOUR)
                ],
                [
                    MouseEvents.XDOWN.ToInput(AutoKeyNet.WindowsHooks.WinApi.NativeMethods.XBUTTON1),
                    MouseEvents.XUP.ToInput(AutoKeyNet.WindowsHooks.WinApi.NativeMethods.XBUTTON1)
                ]),


            //new HotKeyRuleRecord([
            //        VirtualKey.XBUTTON1.ToInput(KeyEventFlags.KEYDOWN,
            //            AutoKeyNet.WindowsHooks.WinApi.NativeMethods.KEY_SUPRESS_NATIVE_BEHAVIOUR),
            //        VirtualKey.KEY_T.ToInput(KeyEventFlags.KEYDOWN,
            //            AutoKeyNet.WindowsHooks.WinApi.NativeMethods.KEY_SUPRESS_NATIVE_BEHAVIOUR),
            //        VirtualKey.KEY_T.ToInput(KeyEventFlags.KEYUP,
            //            AutoKeyNet.WindowsHooks.WinApi.NativeMethods.KEY_SUPRESS_NATIVE_BEHAVIOUR),
            //        VirtualKey.XBUTTON1.ToInput(KeyEventFlags.KEYUP,
            //            AutoKeyNet.WindowsHooks.WinApi.NativeMethods.KEY_SUPRESS_NATIVE_BEHAVIOUR),
            //    ],
            //    () => { }),

            new HotKeyRuleRecord([
                    VirtualKey.XBUTTON1.ToInput(KeyEventFlags.KEYDOWN,
                        AutoKeyNet.WindowsHooks.WinApi.NativeMethods.KEY_SUPRESS_NATIVE_BEHAVIOUR),
                    VirtualKey.KEY_T.ToInput(KeyEventFlags.KEYDOWN,
                        AutoKeyNet.WindowsHooks.WinApi.NativeMethods.KEY_SUPRESS_NATIVE_BEHAVIOUR)
                ],
                () => RunProgram("TOTALCMD64", "C:\\Program Files (x86)\\Total Commander\\TOTALCMD64.EXE")),

            new HotKeyRuleRecord([
                    VirtualKey.XBUTTON1.ToInput(KeyEventFlags.KEYDOWN,
                        AutoKeyNet.WindowsHooks.WinApi.NativeMethods.KEY_SUPRESS_NATIVE_BEHAVIOUR),
                    VirtualKey.KEY_X.ToInput(KeyEventFlags.KEYDOWN,
                        AutoKeyNet.WindowsHooks.WinApi.NativeMethods.KEY_SUPRESS_NATIVE_BEHAVIOUR)
                ],
                () => RunProgram("iexplore", "C:\\Program Files\\Internet Explorer\\iexplore.exe")),
            new HotKeyRuleRecord([
                    VirtualKey.XBUTTON1.ToInput(KeyEventFlags.KEYDOWN,
                        AutoKeyNet.WindowsHooks.WinApi.NativeMethods.KEY_SUPRESS_NATIVE_BEHAVIOUR),
                    VirtualKey.KEY_E.ToInput(KeyEventFlags.KEYDOWN,
                        AutoKeyNet.WindowsHooks.WinApi.NativeMethods.KEY_SUPRESS_NATIVE_BEHAVIOUR)
                ],
            () => RunProgram("EXCEL", "c:\\Program Files\\Microsoft Office\\Office15\\EXCEL.EXE")),
            new HotKeyRuleRecord([
                    VirtualKey.XBUTTON1.ToInput(KeyEventFlags.KEYDOWN,
                        AutoKeyNet.WindowsHooks.WinApi.NativeMethods.KEY_SUPRESS_NATIVE_BEHAVIOUR),
                    VirtualKey.KEY_W.ToInput(KeyEventFlags.KEYDOWN,
                        AutoKeyNet.WindowsHooks.WinApi.NativeMethods.KEY_SUPRESS_NATIVE_BEHAVIOUR)
                ],
            () => RunProgram("WINWORD", "c:\\Program Files\\Microsoft Office\\Office15\\WINWORD.EXE")),
            new HotKeyRuleRecord([
                    VirtualKey.XBUTTON1.ToInput(KeyEventFlags.KEYDOWN,
                        AutoKeyNet.WindowsHooks.WinApi.NativeMethods.KEY_SUPRESS_NATIVE_BEHAVIOUR),
                    VirtualKey.KEY_S.ToInput(KeyEventFlags.KEYDOWN,
                        AutoKeyNet.WindowsHooks.WinApi.NativeMethods.KEY_SUPRESS_NATIVE_BEHAVIOUR)
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

            result |= AutoKeyNetApp.WinApi.NativeMethods.ShowWindow(process.MainWindowHandle, AutoKeyNetApp.WinApi.NativeMethods.SW_MAXIMIZE);
            result |= AutoKeyNetApp.WinApi.NativeMethods.SetForegroundWindow(process.MainWindowHandle);
        }

        if (processes.Length == 0 || !result)
        {
            Debug.WriteLine($"    DEBUG: Start program:'{pathToProgram}'");
            Process.Start(pathToProgram);
        }
    }
}