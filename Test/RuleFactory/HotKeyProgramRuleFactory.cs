using System.Diagnostics;
using System.Runtime.InteropServices;
using AutoKeyNet.WindowsHooks.Rule;
using AutoKeyNet.WindowsHooks.WinApi;
using AutoKeyNet.WindowsHooks.WindowsStruct;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace AutoKeyNetApp.RuleFactory;

/// <summary>
/// Создание горячих клавиш по запуску программ
/// </summary>
internal class HotKeyProgramRuleFactory : BaseRuleFactory
{
    public override List<BaseRuleRecord> Create()
    {
        return new List<BaseRuleRecord>()
        {

            new HotKeyRuleRecord("{XBUTTON1 DOWN}{KEY_X DOWN}", () => RunProgram("iexplore", "C:\\Program Files\\Internet Explorer\\iexplore.exe"),null, HotKeyRuleRecordOptionFlags.DelayKeyDownToKyeUpForPrefixKey),
            new HotKeyRuleRecord("{XBUTTON1 DOWN}{KEY_E DOWN}", () => RunProgram("EXCEL", "c:\\Program Files\\Microsoft Office\\Office15\\EXCEL.EXE")),
            new HotKeyRuleRecord("{XBUTTON1 DOWN}{KEY_W DOWN}", () => RunProgram("WINWORD", "c:\\Program Files\\Microsoft Office\\Office15\\WINWORD.EXE")),
            new HotKeyRuleRecord("{XBUTTON1 DOWN}{KEY_T DOWN}", () => RunProgram("TOTALCMD64", "C:\\Program Files (x86)\\Total Commander\\TOTALCMD64.EXE"),null, HotKeyRuleRecordOptionFlags.DelayKeyDownToKyeUpForPrefixKey),
            new HotKeyRuleRecord("{XBUTTON1 DOWN}{KEY_S DOWN}", () => RunProgram("OUTLOOK", "C:\\Program Files\\Microsoft Office\\Office15\\OUTLOOK.EXE")),
        };
    }

    private static void RunProgram(string processName, string pathToProgram)
    {
        bool result = false;
        var processes = Process.GetProcessesByName(processName);
        var process = processes.FirstOrDefault();
        if (process is not null) 
        {
            Debug.WriteLine($"    DEBUG: Set as foreground:'{process.ProcessName}'");

            result |= NativeMethods.ShowWindow(process.MainWindowHandle, NativeMethods.SW_MAXIMIZE);
            result |= NativeMethods.SetForegroundWindow(process.MainWindowHandle);
        }

        if (processes.Length == 0 || !result)
        {
            Debug.WriteLine($"    DEBUG: Start program:'{pathToProgram}'");
            Process.Start(pathToProgram);
        }
    }

    //[DllImport("user32.dll")]
    //public static extern IntPtr GetForegroundWindow();
    //[DllImport("user32.dll", CharSet = CharSet.Auto)]
    //public static extern bool PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
    //public void ChangeLanguage(string language)
    //{
    //    switch (language)
    //    {
    //        case "RU":
    //            PostMessage(GetForegroundWindow(), 0x0050, 2, 0);
    //    }
    //}
}