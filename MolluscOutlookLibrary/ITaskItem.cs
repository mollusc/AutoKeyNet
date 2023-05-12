using Microsoft.Office.Interop.Outlook;

namespace MolluscOutlookLibrary
{
    public interface ITaskItem
    {
        public  bool Complete { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime ToDoTaskOrdinal { get; set; }
        public DateTime DateCompleted { get; set; }
        public void MarkInterval(OlMarkInterval interval);

    }
}