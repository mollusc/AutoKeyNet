using Outlook = Microsoft.Office.Interop.Outlook;

namespace MolluscOutlookLibrary;

internal class TaskItem : OutlookItem, ITaskItem
{
    private readonly Outlook.TaskItem _taskItem;


    public TaskItem(Outlook.TaskItem taskItem)
    {
        _taskItem = taskItem;
    }

    public override Outlook.OlImportance Importance
    {
        get=> _taskItem.Importance;
        set=> _taskItem.Importance = value;
    }

    protected override string? Categories
    {
        get=> _taskItem.Categories; 
        set => _taskItem.Categories = value;
    }
    public  bool Complete
    {
        get => _taskItem.Complete;
        set => _taskItem.Complete = value;
    }

    public DateTime StartDate
    {
        get => _taskItem.StartDate;
        set => _taskItem.StartDate = value;
    }
    public DateTime DueDate
    {
        get => _taskItem.DueDate;
        set => _taskItem.DueDate = value;
    }

    public DateTime ToDoTaskOrdinal
    {
        get => _taskItem.ToDoTaskOrdinal;
        set => _taskItem.ToDoTaskOrdinal = value;
    }

    public DateTime DateCompleted
    {
        get => _taskItem.DateCompleted;
        set => _taskItem.DateCompleted = value;
    }

    public void MarkInterval(Outlook.OlMarkInterval interval)
    {
        switch (interval)
        {
            case Outlook.OlMarkInterval.olMarkComplete:
                Complete = true;
                break;
            case Outlook.OlMarkInterval.olMarkNoDate:
                DueDate = Constants.NullDate;
                DateCompleted = Constants.NullDate;
                break;
            case Outlook.OlMarkInterval.olMarkToday:
                DueDate = DateTime.Now;
                StartDate = DateTime.Now;
                ToDoTaskOrdinal = DateTime.Now;
                DateCompleted = Constants.NullDate;
                break;
            case Outlook.OlMarkInterval.olMarkTomorrow:
                StartDate = DateTime.Now.AddDays(1);
                DueDate = DateTime.Now.AddDays(1);
                ToDoTaskOrdinal = DateTime.Now;
                DateCompleted = Constants.NullDate;
                break;
            case Outlook.OlMarkInterval.olMarkThisWeek:
                DueDate = GetNextWeekday(DateTime.Now, DayOfWeek.Friday);
                StartDate = DateTime.Now.AddDays(2) > DueDate ? DateTime.Now.AddDays(2) : DueDate;
                ToDoTaskOrdinal = DateTime.Now;
                DateCompleted = Constants.NullDate;
                break;
            case Outlook.OlMarkInterval.olMarkNextWeek:
                StartDate = GetNextWeekday(DateTime.Now.AddDays(1), DayOfWeek.Monday);
                DueDate = GetNextWeekday(StartDate, DayOfWeek.Friday);
                ToDoTaskOrdinal = DateTime.Now;
                DateCompleted = Constants.NullDate;
                break;
        } 
    }

    private static DateTime GetNextWeekday(DateTime start, DayOfWeek day)
    {
        int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
        return start.AddDays(daysToAdd);
    }


    public override void Save() => _taskItem.Save();
}