using Windows.Win32.UI.Input.KeyboardAndMouse;
using Accessibility;
using static Windows.Win32.PInvoke;
using AutoKeyNet.Helper;

namespace Windows.Win32
{
    namespace UI.Input.KeyboardAndMouse
    {
        public partial struct INPUT
        {
            public override string ToString()
            {
                switch (type)
                {
                    case INPUT_TYPE.INPUT_MOUSE:
                        return $"{Anonymous.mi.dwFlags} {Anonymous.mi.mouseData:x8}";
                    case INPUT_TYPE.INPUT_KEYBOARD:
                        var vk = Anonymous.ki.wVk;
                        string? eventFlag = Anonymous.ki.dwFlags switch
                        {
                            KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP => "↑",
                            _ => "↓",
                        };
                        return $"{vk.GetDisplayName() ?? vk.ToString()}({(char.IsLetterOrDigit(vk.ToUnicode())?vk.ToUnicode():"")}){eventFlag}";
                    case INPUT_TYPE.INPUT_HARDWARE:
                        return "HardWare: " + Anonymous.hi.uMsg;
                    default:
                        return "Undefined type";
                }
            }
        }
    }
}
