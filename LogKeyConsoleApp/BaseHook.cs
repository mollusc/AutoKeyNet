using LogKeyConsoleApp.WindowsEnums;
using LogKeyConsoleApp.WindowsStruct;

namespace LogKeyConsoleApp;

internal abstract class BaseHook : IDisposable
{
    protected nint HookId;
    public void Dispose()
    {
        Unhook();
        GC.SuppressFinalize(this);
    }
    ~BaseHook()
    {
        Dispose();
    }
    protected void InitializeHook()
    {
        HookId = SetHook();
    }
    protected abstract nint SetHook();
    protected abstract void Unhook();
    protected bool FilterInput(Input i)
    {
        if (i.Type == InputType.INPUT_KEYBOARD)
            return true;
        if (i.Type == InputType.INPUT_MOUSE)
        {
            var eventsToDisplay = new HashSet<MouseEvents>
            {
                MouseEvents.LEFTUP,
                MouseEvents.LEFTDOWN,
                MouseEvents.RIGHTUP,
                MouseEvents.RIGHTDOWN,
                MouseEvents.MIDDLEDOWN,
                MouseEvents.MIDDLEUP,
                MouseEvents.XDOWN,
                MouseEvents.XUP
            };
            return eventsToDisplay.Contains(i.Data.MouseInput.Flags);
        }

        return false;
    }
}