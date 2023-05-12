using System.Diagnostics;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace MolluscOutlookLibrary;

public static class OutlookApplication
{
    public static IEnumerable<OutlookItem> GetSelectedItems()
    {
        if (Process.GetProcessesByName("OUTLOOK").Any())
        {
            var application = new Outlook.Application();
            var selectedItems = application?.ActiveExplorer().Selection;
            if (selectedItems != null)
                foreach (var item in selectedItems)
                {
                    OutlookItem? outlookItem = item switch
                    {
                        Outlook.MailItem mailItem => new MailItem(mailItem),
                        Outlook.TaskItem taskItem => new TaskItem(taskItem),
                        _ => null
                    };
                    if (outlookItem is not null)
                        yield return outlookItem;
                }
        }
    }
    public static void Search(string text)
    {
        if (Process.GetProcessesByName("OUTLOOK").Any())
            new Outlook.Application()?.ActiveExplorer()
                .Search(text, Outlook.OlSearchScope.olSearchScopeCurrentFolder);
    }
}