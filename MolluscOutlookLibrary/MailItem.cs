using Outlook = Microsoft.Office.Interop.Outlook;

namespace MolluscOutlookLibrary;

public class MailItem : OutlookItem, ITaskItem
{
    private readonly Outlook.MailItem _mailItem;

    public MailItem(Outlook.MailItem mailItem)
    {
        _mailItem = mailItem;
    }

    public override Outlook.OlImportance Importance
    {
        get => _mailItem.Importance;
        set => _mailItem.Importance = value;

    }

    public string ConversationTopic => _mailItem.ConversationTopic;
    public string SenderName => _mailItem.SenderName;

    protected override string? Categories
    {
        get=> _mailItem.Categories; 
        set => _mailItem.Categories = value;
    }

    public bool Complete
    {
        get => _mailItem.IsMarkedAsTask && _mailItem.TaskCompletedDate != Constants.NullDate;
        set => _mailItem.MarkAsTask(value ? Outlook.OlMarkInterval.olMarkComplete : Outlook.OlMarkInterval.olMarkNoDate);
    }

    public DateTime StartDate
    {
        get => _mailItem.TaskStartDate;
        set
        {
            if(!_mailItem.IsMarkedAsTask)
                _mailItem.MarkAsTask(Outlook.OlMarkInterval.olMarkNoDate);
            _mailItem.TaskStartDate = value;
        }
    }

    public DateTime DueDate
    {
        get => _mailItem.TaskDueDate;
        set
        {
            if(!_mailItem.IsMarkedAsTask)
                _mailItem.MarkAsTask(Outlook.OlMarkInterval.olMarkNoDate);
            _mailItem.TaskDueDate = value;
        }
    }

    public DateTime ToDoTaskOrdinal
    {
        get => _mailItem.ToDoTaskOrdinal;
        set
        {
            if(!_mailItem.IsMarkedAsTask)
                _mailItem.MarkAsTask(Outlook.OlMarkInterval.olMarkNoDate);
            _mailItem.ToDoTaskOrdinal = value;
        } 
    }
    public DateTime DateCompleted {
        get=> _mailItem.TaskCompletedDate;
        set
        {
            if(!_mailItem.IsMarkedAsTask)
                _mailItem.MarkAsTask(Outlook.OlMarkInterval.olMarkNoDate);
            _mailItem.TaskCompletedDate = value;
        }
    }

    public void MarkInterval(Outlook.OlMarkInterval interval) => _mailItem.MarkAsTask(interval);

    public override void Save() => _mailItem.Save();
}