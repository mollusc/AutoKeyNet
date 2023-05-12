using System.Diagnostics;
using AutoKeyNet.WindowsHooks.Rule;
using MolluscOutlookLibrary;
using MailItem = MolluscOutlookLibrary.MailItem;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace AutoKeyNetApp.RuleFactory;

/// <summary>
/// Создание горячих клавиш по работе с Outlook
/// </summary>
internal class OutlookRuleFactory : BaseRuleFactory
{
    public override List<BaseRuleRecord> Create()
    {

        return new List<BaseRuleRecord>()
        {
            new HotKeyRuleRecord("1", () => SetImportanceOutlook(Outlook.OlImportance.olImportanceLow), OutlookCheckNotEditor),
            new HotKeyRuleRecord("2", () => SetImportanceOutlook(Outlook.OlImportance.olImportanceNormal), OutlookCheckNotEditor),
            new HotKeyRuleRecord("3", () => SetImportanceOutlook(Outlook.OlImportance.olImportanceHigh), OutlookCheckNotEditor),

            new HotStringRuleRecord("дд", GetGreeting, checkWindowCondition: OutlookCheckEditor ),

            new VimKeyRuleRecord("c", SearchBySubject, OutlookCheckNotEditor),
            new VimKeyRuleRecord("a", SearchBySender, OutlookCheckNotEditor),
            new VimKeyRuleRecord("t", () =>
            {
                MarkOutlookItems(Outlook.OlMarkInterval.olMarkToday);
                RemoveWaitCategory();
            }, OutlookCheckNotEditor),
            new VimKeyRuleRecord("tt", () =>
            {
                MarkOutlookItems(Outlook.OlMarkInterval.olMarkTomorrow);
                RemoveWaitCategory();
            }, OutlookCheckNotEditor),
            new VimKeyRuleRecord("tw", () =>
            {
                MarkOutlookItems(Outlook.OlMarkInterval.olMarkThisWeek);
                RemoveWaitCategory();
            }, OutlookCheckNotEditor),
            new VimKeyRuleRecord("tm", () =>
            {
                SetToNextMonday();
                RemoveWaitCategory();
            }, OutlookCheckNotEditor),
            new VimKeyRuleRecord("tww", () =>
            {
                MarkOutlookItems(Outlook.OlMarkInterval.olMarkNextWeek);
                RemoveWaitCategory();
            }, OutlookCheckNotEditor),
            new VimKeyRuleRecord("w", () =>
            {
                MarkOutlookItems(Outlook.OlMarkInterval.olMarkToday);
                SetWaitCategory();
            }, OutlookCheckNotEditor),
            new VimKeyRuleRecord("wt", () =>
            {
                MarkOutlookItems(Outlook.OlMarkInterval.olMarkTomorrow);
                SetWaitCategory();
            }, OutlookCheckNotEditor),
            new VimKeyRuleRecord("wm", () =>
            {
                SetToNextMonday();
                SetWaitCategory();
            }, OutlookCheckNotEditor),
            new VimKeyRuleRecord("ww", () =>
            {
                MarkOutlookItems(Outlook.OlMarkInterval.olMarkThisWeek);
                SetWaitCategory();
            }, OutlookCheckNotEditor),
            new VimKeyRuleRecord("www", () =>
            {
                MarkOutlookItems(Outlook.OlMarkInterval.olMarkNextWeek);
                SetWaitCategory();
            }, OutlookCheckNotEditor),
            new VimKeyRuleRecord("d", () => MarkOutlookItems(Outlook.OlMarkInterval.olMarkComplete), OutlookCheckNotEditor),
            new VimKeyRuleRecord("r", "{CONTROL DOWN}{KEY_R DOWN}{KEY_R UP}{CONTROL UP}", OutlookCheckNotEditor),
            new VimKeyRuleRecord("rr", "{CONTROL DOWN}{SHIFT DOWN}{KEY_R DOWN}{KEY_R UP}{SHIFT UP}{CONTROL UP}", OutlookCheckNotEditor),
            new VimKeyRuleRecord("f", "{CONTROL DOWN}{KEY_F DOWN}{KEY_F UP}{CONTROL UP}", OutlookCheckNotEditor),
            new VimKeyRuleRecord("ff", "{CONTROL DOWN}{SHIFT DOWN}{KEY_F DOWN}{KEY_F UP}{SHIFT UP}{CONTROL UP}", OutlookCheckNotEditor),
            new VimKeyRuleRecord("j", "{DOWN}", OutlookCheckNotEditor),
            new VimKeyRuleRecord("k", "{UP}", OutlookCheckNotEditor),
            new VimKeyRuleRecord("gg", "{HOME}", OutlookCheckNotEditor),
            new VimKeyRuleRecord("G", "{END}", OutlookCheckNotEditor),
            new VimKeyRuleRecord("o", "{CONTROL DOWN}{KEY_O DOWN}{KEY_O UP}{CONTROL UP}", OutlookCheckNotEditor),

        };

    }
    private static void MarkOutlookItems(Outlook.OlMarkInterval interval)
    {
        Task.Run(() =>
        {
            foreach (OutlookItem item in OutlookApplication.GetSelectedItems())
            {
                (item as ITaskItem)?.MarkInterval(interval);
                item.Save();
            }
        });
    }

    private static void SetToNextMonday()
    {
        Task.Run(() =>
        {
            foreach (OutlookItem item in OutlookApplication.GetSelectedItems())
            {
                DateTime today = DateTime.Today.AddDays(1);
                int daysUntilMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
                var nextMonday = today.AddDays(daysUntilMonday);
                if (item is ITaskItem i)
                {
                    i.DueDate = nextMonday;
                    item.Save();
                }
            }
        });
    }
    /// <summary>
    /// Задать категорию "Wait" для выделенных элементов
    /// </summary>
    private static void SetWaitCategory()
    {
        Task.Run(() =>
        {
            string category = "Wait";
            foreach (OutlookItem item in OutlookApplication.GetSelectedItems())
            {
                item.AddCategory(category);
                item.Save();
            }
        });
    }
    private static void RemoveWaitCategory()
    {
        Task.Run(() =>
        {
            string category = "Wait";
            foreach (OutlookItem item in OutlookApplication.GetSelectedItems())
            {
                item.RemoveCategory(category);
                item.Save();
            }
        });
    }

    private static readonly string[] ControlNames = new string[] { "_WwG", "RICHEDIT60W", "RichEdit20WPT" };
    public static bool OutlookCheckNotEditor(string? windowTitle, string? windowClass, string? windowModuleName, string? controlClassName)
    {
        return windowClass == "rctrl_renwnd32" && controlClassName is not null && !ControlNames.Contains(controlClassName);
    }

    public static bool OutlookCheckEditor(string? windowTitle, string? windowClass, string? windowModuleName, string? controlClassName)
    {
        return windowClass == "rctrl_renwnd32" && controlClassName is not null && ControlNames.Contains(controlClassName);
    }

    public static bool NotInOutlookEditor(string? windowTitle, string? windowClass, string? windowModuleName, string? controlClassName)
    {
        return windowClass != "rctrl_renwnd32" || OutlookCheckNotEditor(windowTitle, windowClass, windowModuleName, controlClassName);

    }

    private static void SetImportanceOutlook(Outlook.OlImportance importance)
    {
        Task.Run(() =>
        {
            foreach (OutlookItem item in OutlookApplication.GetSelectedItems())
            {
                item.Importance = importance;
                item.Save();
            }
        });
    }

    private static void SearchBySubject()
    {
        Task.Run(() =>
        {
            if (OutlookApplication.GetSelectedItems().FirstOrDefault(i => i is MailItem) is MailItem item)
                OutlookApplication.Search($"[Предмет]:=\"{item.ConversationTopic}\"");
        });
    }

    private static void SearchBySender()
    {
        Task.Run(() =>
        {
            if (OutlookApplication.GetSelectedItems().FirstOrDefault(i => i is MailItem) is MailItem item)
                OutlookApplication.Search($"откого:({item.SenderName})");
        });
    }
    private static string GetGreeting()
    {
        return Task.Run(() =>
        {
            if (OutlookApplication.GetRecipientCurrentWindow() is { } item)
            {
                var name = item.Split(' ');
                return $"Добрый день, {name[1]} {name[2]}!";
            }
            return "Добрый день!";
        }).Result;
    }
}