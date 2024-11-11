using Windows.Win32.UI.Input.KeyboardAndMouse;
using static Windows.Win32.PInvoke;

namespace AutoKeyNet.Helper;

public class InputComparerByVKeyAndFlag : IEqualityComparer<INPUT>
{
    bool IEqualityComparer<INPUT>.Equals(INPUT x, INPUT y)
    {
        return x.type == y.type
               && (
                   x.type == INPUT_TYPE.INPUT_KEYBOARD && x.Anonymous.ki.wVk == y.Anonymous.ki.wVk
                                                            && x.Anonymous.ki.dwFlags == y.Anonymous.ki.dwFlags
                   || x.type == INPUT_TYPE.INPUT_MOUSE && x.Anonymous.mi.dwFlags == y.Anonymous.mi.dwFlags
                                                            && x.Anonymous.mi.mouseData >> 16 ==
                                                            y.Anonymous.mi.mouseData >> 16);
    }

    int IEqualityComparer<INPUT>.GetHashCode(INPUT obj)
    {
        return HashCode.Combine(obj.type,
            obj.type == INPUT_TYPE.INPUT_KEYBOARD
                ? (uint)obj.Anonymous.ki.wVk
                : (uint)obj.Anonymous.mi.dwFlags,
            obj.type == INPUT_TYPE.INPUT_MOUSE
                ? (uint)obj.Anonymous.ki.dwFlags
                : obj.Anonymous.mi.mouseData >> 16);
    }
}