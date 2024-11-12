using Windows.Win32.UI.Input.KeyboardAndMouse;
using AutoKeyNet.Helper;
using AutoKeyNet.RuleRecords;
using Microsoft.Office.Interop.Outlook;
using MolluscOutlookLibrary;
using MailItem = MolluscOutlookLibrary.MailItem;

namespace AutoKeyNetApp.RuleFactory;

/// <summary>
///     This class creates hotkeys to automate common tasks in Outlook
/// </summary>
internal class OutlookRuleFactory : BaseRuleFactory
{
    /// <summary>
    ///     Array of control names used in Outlook for editing text.
    /// </summary>
    private static readonly string[] ControlNames = { "_WwG", "RICHEDIT60W", "RichEdit20WPT" };

    public override List<BaseRuleRecord> Create()
    {
        return new List<BaseRuleRecord>
        {
            new HotKeyRuleRecord(VIRTUAL_KEY.VK_1.ToInputsPressKey().ToArray(), () => SetImportanceOutlook(OlImportance.olImportanceLow), OutlookCheckNotEditor),
            new HotKeyRuleRecord(VIRTUAL_KEY.VK_2.ToInputsPressKey().ToArray(), () => SetImportanceOutlook(OlImportance.olImportanceNormal), OutlookCheckNotEditor),
            new HotKeyRuleRecord(VIRTUAL_KEY.VK_3.ToInputsPressKey().ToArray(), () => SetImportanceOutlook(OlImportance.olImportanceHigh), OutlookCheckNotEditor),

            new HotStringRuleRecord("дд", GetGreeting, checkWindowCondition: OutlookCheckEditor),

            new VimKeyRuleRecord("c", SearchBySubject, OutlookCheckNotEditor),
            new VimKeyRuleRecord("a", SearchBySender, OutlookCheckNotEditor),
            new VimKeyRuleRecord("t", () =>
            {
                MarkOutlookItems(OlMarkInterval.olMarkToday);
                RemoveWaitCategory();
            }, OutlookCheckNotEditor),
            new VimKeyRuleRecord("tt", () =>
            {
                MarkOutlookItems(OlMarkInterval.olMarkTomorrow);
                RemoveWaitCategory();
            }, OutlookCheckNotEditor),
            new VimKeyRuleRecord("tw", () =>
            {
                MarkOutlookItems(OlMarkInterval.olMarkThisWeek);
                RemoveWaitCategory();
            }, OutlookCheckNotEditor),
            new VimKeyRuleRecord("tm", () =>
            {
                SetToNextMonday();
                RemoveWaitCategory();
            }, OutlookCheckNotEditor),
            new VimKeyRuleRecord("tww", () =>
            {
                MarkOutlookItems(OlMarkInterval.olMarkNextWeek);
                RemoveWaitCategory();
            }, OutlookCheckNotEditor),
            new VimKeyRuleRecord("w", () =>
            {
                MarkOutlookItems(OlMarkInterval.olMarkToday);
                SetWaitCategory();
            }, OutlookCheckNotEditor),
            new VimKeyRuleRecord("wt", () =>
            {
                MarkOutlookItems(OlMarkInterval.olMarkTomorrow);
                SetWaitCategory();
            }, OutlookCheckNotEditor),
            new VimKeyRuleRecord("wm", () =>
            {
                SetToNextMonday();
                SetWaitCategory();
            }, OutlookCheckNotEditor),
            new VimKeyRuleRecord("ww", () =>
            {
                MarkOutlookItems(OlMarkInterval.olMarkThisWeek);
                SetWaitCategory();
            }, OutlookCheckNotEditor),
            new VimKeyRuleRecord("www", () =>
            {
                MarkOutlookItems(OlMarkInterval.olMarkNextWeek);
                SetWaitCategory();
            }, OutlookCheckNotEditor),
            new VimKeyRuleRecord("d", () => MarkOutlookItems(OlMarkInterval.olMarkComplete), OutlookCheckNotEditor),
            new VimKeyRuleRecord("r", () =>
            {
                INPUT[] inputs =
                [
                    VIRTUAL_KEY.VK_CONTROL.ToInput(),
                    VIRTUAL_KEY.VK_R.ToInput(),
                    VIRTUAL_KEY.VK_R.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP),
                    VIRTUAL_KEY.VK_CONTROL.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP)
                ];
                inputs.Send();
            }, OutlookCheckNotEditor),
            new VimKeyRuleRecord("rr", () =>
            {
                INPUT[] inputs =
                [
                    VIRTUAL_KEY.VK_CONTROL.ToInput(),
                    VIRTUAL_KEY.VK_SHIFT.ToInput(),
                    VIRTUAL_KEY.VK_R.ToInput(),
                    VIRTUAL_KEY.VK_R.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP),
                    VIRTUAL_KEY.VK_SHIFT.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP),
                    VIRTUAL_KEY.VK_CONTROL.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP),
                ];
                inputs.Send();
            }, OutlookCheckNotEditor),
            new VimKeyRuleRecord("f", () =>
            {
                INPUT[] inputs =
                [
                    VIRTUAL_KEY.VK_CONTROL.ToInput(),
                    VIRTUAL_KEY.VK_F.ToInput(),
                    VIRTUAL_KEY.VK_F.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP),
                    VIRTUAL_KEY.VK_CONTROL.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP)
                ];
                inputs.Send();
            }, OutlookCheckNotEditor),
            new VimKeyRuleRecord("ff", () =>
            {
                INPUT[] inputs =
                [
                    VIRTUAL_KEY.VK_CONTROL.ToInput(),
                    VIRTUAL_KEY.VK_SHIFT.ToInput(),
                    VIRTUAL_KEY.VK_F.ToInput(),
                    VIRTUAL_KEY.VK_F.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP),
                    VIRTUAL_KEY.VK_SHIFT.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP),
                    VIRTUAL_KEY.VK_CONTROL.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP),
                ];
                inputs.Send();
            },
                OutlookCheckNotEditor),
            new VimKeyRuleRecord("j", () => {VIRTUAL_KEY.VK_DOWN.ToInputsPressKey().ToArray().Send(); }, OutlookCheckNotEditor),
            new VimKeyRuleRecord("k", () => {VIRTUAL_KEY.VK_UP.ToInputsPressKey().ToArray().Send(); }, OutlookCheckNotEditor),
            new VimKeyRuleRecord("gg", () => {VIRTUAL_KEY.VK_HOME.ToInputsPressKey().ToArray().Send(); }, OutlookCheckNotEditor),
            new VimKeyRuleRecord("G", () => {VIRTUAL_KEY.VK_END.ToInputsPressKey().ToArray().Send(); }, OutlookCheckNotEditor),
            new VimKeyRuleRecord("o", () =>
            {
                INPUT[] inputs =
                [
                    VIRTUAL_KEY.VK_CONTROL.ToInput(),
                    VIRTUAL_KEY.VK_O.ToInput(),
                    VIRTUAL_KEY.VK_O.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP),
                    VIRTUAL_KEY.VK_CONTROL.ToInput(KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP)
                ];
                inputs.Send();
            }, OutlookCheckNotEditor)
        };
    }

    /// <summary>
    ///     Set interval for selected items
    /// </summary>
    /// <param name="interval">Interval of items</param>
    private static void MarkOutlookItems(OlMarkInterval interval)
    {
        Task.Run(() =>
        {
            foreach (var item in OutlookApplication.GetSelectedItems())
            {
                (item as ITaskItem)?.MarkInterval(interval);
                item.Save();
            }
        });
    }

    /// <summary>
    ///     Update the due date of selected items in Outlook to the upcoming Monday.
    /// </summary>
    private static void SetToNextMonday()
    {
        Task.Run(() =>
        {
            foreach (var item in OutlookApplication.GetSelectedItems())
            {
                var today = DateTime.Today.AddDays(1);
                var daysUntilMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
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
    ///     Set the category of the selected Outlook items to "Wait".
    /// </summary>
    private static void SetWaitCategory()
    {
        Task.Run(() =>
        {
            var category = "Wait";
            foreach (var item in OutlookApplication.GetSelectedItems())
            {
                item.AddCategory(category);
                item.Save();
            }
        });
    }

    /// <summary>
    ///     Remove "Wait" category from the selected Outlook items
    /// </summary>
    private static void RemoveWaitCategory()
    {
        Task.Run(() =>
        {
            var category = "Wait";
            foreach (var item in OutlookApplication.GetSelectedItems())
            {
                item.RemoveCategory(category);
                item.Save();
            }
        });
    }

    /// <summary>
    ///     Check if the current window is Outlook and the current control is not the editor.
    /// </summary>
    /// <param name="windowTitle">Title of the foreground window</param>
    /// <param name="windowClass">Class of the foreground window</param>
    /// <param name="windowModule">Module name (file *.exe) of the foreground window</param>
    /// <param name="windowControl">Name of the focused control</param>
    /// <returns>True if the current window is Outlook and the current control is not the editor, otherwise false</returns>
    public static bool OutlookCheckNotEditor(string? windowTitle, string? windowClass, string? windowModule,
        string? windowControl)
    {
        return windowClass == "rctrl_renwnd32" && windowControl is not null && !ControlNames.Contains(windowControl);
    }

    /// <summary>
    ///     Check if the current window is Outlook and the current control is the editor.
    /// </summary>
    /// <param name="windowTitle">Title of the foreground window</param>
    /// <param name="windowClass">Class of the foreground window</param>
    /// <param name="windowModule">Module name (file *.exe) of the foreground window</param>
    /// <param name="windowControl">Name of the focused control</param>
    /// <returns>True if the current window is Outlook and the current control is not the editor, otherwise false</returns>
    public static bool OutlookCheckEditor(string? windowTitle, string? windowClass, string? windowModule,
        string? windowControl)
    {
        return windowClass == "rctrl_renwnd32" && windowControl is not null && ControlNames.Contains(windowControl);
    }

    /// <summary>
    ///     Set the importance of the selected Outlook items.
    /// </summary>
    /// <param name="importance">Importance of selected items</param>
    private static void SetImportanceOutlook(OlImportance importance)
    {
        Task.Run(() =>
        {
            foreach (var item in OutlookApplication.GetSelectedItems())
            {
                item.Importance = importance;
                item.Save();
            }
        });
    }

    /// <summary>
    ///     Set the filter of the selected Outlook items by subject.
    /// </summary>
    private static void SearchBySubject()
    {
        Task.Run(() =>
        {
            if (OutlookApplication.GetSelectedItems().FirstOrDefault(i => i is MailItem) is MailItem item)
                OutlookApplication.Search($"[Предмет]:=\"{item.ConversationTopic}\"");
        });
    }

    /// <summary>
    ///     Set the filter of the selected Outlook items by sender.
    /// </summary>
    private static void SearchBySender()
    {
        Task.Run(() =>
        {
            if (OutlookApplication.GetSelectedItems().FirstOrDefault(i => i is MailItem) is MailItem item)
                OutlookApplication.Search($"откого:({item.SenderName})");
        });
    }

    /// <summary>
    ///     Get greeting text including the name of the recipient.
    /// </summary>
    /// <returns>Greeting text</returns>
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