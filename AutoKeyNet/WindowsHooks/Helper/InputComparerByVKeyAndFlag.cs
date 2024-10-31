using AutoKeyNet.WindowsHooks.WindowsEnums;
using AutoKeyNet.WindowsHooks.WindowsStruct;

namespace AutoKeyNet.WindowsHooks.Helper;

public class InputComparerByVKeyAndFlag : IEqualityComparer<Input>
{
    public bool Equals(Input x, Input y)
    {
        return x.Type == y.Type
               && (
                   (x.Type == InputType.INPUT_KEYBOARD && x.Data.KeyboardInput.VirtualKey ==
                                                       y.Data.KeyboardInput.VirtualKey
                                                       && x.Data.KeyboardInput.Flags == y.Data.KeyboardInput.Flags)
                   || (x.Type == InputType.INPUT_MOUSE && x.Data.MouseInput.Flags == y.Data.MouseInput.Flags
                                                       && x.Data.MouseInput.MouseData >> 16 ==
                                                       y.Data.MouseInput.MouseData >> 16));
    }

    public int GetHashCode(Input obj)
    {
        return HashCode.Combine((int)obj.Type,
            obj.Type == InputType.INPUT_KEYBOARD
                ? obj.Data.KeyboardInput.VirtualKey
                : (ushort)obj.Data.MouseInput.Flags,
            obj.Type == InputType.INPUT_KEYBOARD
                ? (int)obj.Data.KeyboardInput.Flags
                : obj.Data.MouseInput.MouseData >> 16);
    }
}