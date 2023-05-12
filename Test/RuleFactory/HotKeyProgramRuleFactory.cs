using System.Diagnostics;
using AutoKeyNet.WindowsHooks.Rule;

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

            new HotKeyRuleRecord("{XBUTTON1}X",
                () => RunProgram("iexplore", "C:\\Program Files\\Internet Explorer\\iexplore.exe")),
            new HotKeyRuleRecord("{XBUTTON1}E",
                () => RunProgram("EXCEL.EXE", "c:\\Program Files\\Microsoft Office\\Office15\\EXCEL.EXE")),
            new HotKeyRuleRecord("{XBUTTON1}W",
                () => RunProgram("WINWORD.EXE", "c:\\Program Files\\Microsoft Office\\Office15\\WINWORD.EXE")),
            new HotKeyRuleRecord("{XBUTTON1}T",
                () => RunProgram("TOTALCMD64.EXE", "C:\\Program Files (x86)\\Total Commander\\TOTALCMD64.EXE")),
            //new HotKeyRuleRecord("{LSHIFT}", )
        };
    }
    private static void RunProgram(string processName, string pathToProgram)
    {
        var runningProcessByName = Process.GetProcessesByName(processName);
        if (runningProcessByName.Length == 0)
        {
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