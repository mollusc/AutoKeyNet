using Microsoft.Office.Interop.Outlook;

namespace MolluscOutlookLibrary;

public abstract class  OutlookItem
{
    public abstract OlImportance Importance { get; set; }
    protected abstract string? Categories { get; set; }
    public IReadOnlyList<string>? GetCategories() => Categories?.Split(';').AsReadOnly();
    public void AddCategory(string category)
    {
        HashSet<string> categories = new HashSet<string>(Categories?.Split(';').Select(c => c.Trim()) ?? Array.Empty<string>(), StringComparer.CurrentCultureIgnoreCase);
        categories.Add(category);
        Categories = string.Join(';', categories);
    }
    public void RemoveCategory(string category)
    {
        HashSet<string> categories = new HashSet<string>(Categories?.Split(';').Select(c => c.Trim()) ?? Array.Empty<string>(), StringComparer.CurrentCultureIgnoreCase);
        categories.Remove(category);
        Categories = string.Join(';', categories);
    }

    public void Clear() => Categories = null;
    public abstract void Save();
}