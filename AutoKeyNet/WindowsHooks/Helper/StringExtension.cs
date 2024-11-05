namespace AutoKeyNet.WindowsHooks.Helper;

/// <summary>
///     Extension methods for the string data type.
/// </summary>
internal static class StringExtension
{
    ///// <summary>
    /////     Dictionary with virtual keys
    ///// </summary>
    //private static readonly Dictionary<string, VIRTUAL_KEY> VkDictionary =
    //    new(
    //        Enum.GetNames<VIRTUAL_KEY>().Select(x =>
    //            new KeyValuePair<string, VIRTUAL_KEY>(x, (VIRTUAL_KEY)Enum.Parse(typeof(VIRTUAL_KEY), x))),
    //        StringComparer.CurrentCultureIgnoreCase);

    ///// <summary>
    /////     Dictionary for specifying keys that require additional flags.
    ///// </summary>
    //private static readonly Dictionary<string, KEYBD_EVENT_FLAGS> FlagsDictionary = new()
    //{
    //    { Enum.GetName(VIRTUAL_KEY.SHIFT)!, KEYBD_EVENT_FLAGS.EXTENDEDKEY },
    //    { Enum.GetName(VIRTUAL_KEY.CONTROL)!, KEYBD_EVENT_FLAGS.EXTENDEDKEY },
    //    { Enum.GetName(VIRTUAL_KEY.MENU)!, KEYBD_EVENT_FLAGS.EXTENDEDKEY }
    //};


    ///// <summary>
    /////     Dictionary for storing flags associated with key events
    ///// </summary>
    //private static readonly Dictionary<string, KEYBD_EVENT_FLAGS> ActionFlagDictionary =
    //    new(StringComparer.CurrentCultureIgnoreCase)
    //    {
    //        { "UP", KEYBD_EVENT_FLAGS.KEYUP },
    //        { "DOWN", KEYBD_EVENT_FLAGS.KEYDOWN }
    //    };

    ///// <summary>
    /////     A method that converts a string object to Input structures.
    /////     This method allows for the use of tags to represent special keys such as "{LEFT}" and "{RIGHT}"
    ///// </summary>
    ///// <param name="text">Text</param>
    ///// <param name="extraInfo">An additional value associated with the keystroke</param>
    ///// <returns>Input structures that represent the text</returns>
    //internal static IEnumerable<Input> ToInputs(this string text, nuint extraInfo = NativeMethods.KEY_IGNORE)
    //{
    //    for (var i = 0; i < text.Length; i++)
    //    {
    //        if (text[i] == '{' && text.IndexOf('}', i) is var k and > 0)
    //        {
    //            var key = text.AsSpan(i, k - i + 1);
    //            ReadOnlySpan<char> keyName;
    //            var keyAction = ReadOnlySpan<char>.Empty;
    //            var indexSeparator = key.IndexOf(' ');
    //            if (indexSeparator > 0)
    //            {
    //                keyName = key[1..indexSeparator];
    //                keyAction = key[(indexSeparator + 1)..^1];
    //            }
    //            else
    //            {
    //                keyName = key[1..^1];
    //            }

    //            if (VkDictionary.ContainsKey(keyName.ToString()))
    //            {
    //                if (keyAction.IsEmpty || !ActionFlagDictionary.ContainsKey(keyAction.ToString()))
    //                {
    //                    foreach (var input in VkDictionary[keyName.ToString()].ToInputsPressKey())
    //                        yield return input;
    //                }
    //                else
    //                {
    //                    KEYBD_EVENT_FLAGS extraFlags = 0;
    //                    if (ActionFlagDictionary.ContainsKey(keyAction.ToString()))
    //                        extraFlags |= ActionFlagDictionary[keyAction.ToString()];
    //                    if (FlagsDictionary.ContainsKey(keyName.ToString()))
    //                        extraFlags |= FlagsDictionary[keyName.ToString()];

    //                    yield return VkDictionary[keyName.ToString()].ToInput(extraFlags);
    //                }

    //                i = k;
    //            }

    //            continue;
    //        }

    //        foreach (var input in text[i].ToInputsPressKey(extraInfo: extraInfo))
    //            yield return input;
    //    }
    //}
}