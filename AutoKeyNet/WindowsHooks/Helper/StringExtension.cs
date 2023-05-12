using AutoKeyNet.WindowsHooks.WindowsEnums;
using AutoKeyNet.WindowsHooks.WindowsStruct;

namespace AutoKeyNet.WindowsHooks.Helper;
/// <summary>
/// Вспомогательные функции для работы со строками
/// </summary>
internal static class StringExtension
{
    /// <summary>
    /// Словарь с перечнем виртуальных клавиш
    /// </summary>
    private static readonly Dictionary<string, VirtualKey> VkDictionary =
        new(Enum.GetNames<VirtualKey>().Select(x => new KeyValuePair<string, VirtualKey>(x, (VirtualKey)Enum.Parse(typeof(VirtualKey), x))), StringComparer.CurrentCultureIgnoreCase);

    private static readonly Dictionary<string, KeyEventFlags> FlagsDictionary = new()
        {
            {Enum.GetName(VirtualKey.SHIFT)!, KeyEventFlags.EXTENDEDKEY},
            {Enum.GetName(VirtualKey.CONTROL)!, KeyEventFlags.EXTENDEDKEY},
            {Enum.GetName(VirtualKey.MENU)!, KeyEventFlags.EXTENDEDKEY},
        };

    private static readonly Dictionary<string, KeyEventFlags> ActionFlagDictionary =
        new(StringComparer.CurrentCultureIgnoreCase)
        {
                { "UP", KeyEventFlags.KEYUP },
                { "DOWN", KeyEventFlags.KEYDOWN }
        };

    /// <summary>
    /// Преобразует строку символов в конкретные клавиши на клавиатуре.
    /// Кроме того допускается использование тегов для специальных клавиш такие как {LEFT}, {RIGHT} и т.д.
    /// Все значения специальных клавиш находятся в переменной SpecialKeys
    /// </summary>
    /// <param name="text">Исходный текст для анализа</param>
    /// <param name="extraInfo"></param>
    /// <returns>Возвращает картеж с значениями виртуальной клавиши и scan кода</returns>
    internal static IEnumerable<Input> ToInputs(this string text, nuint extraInfo = Constants.KEY_IGNORE)
    {
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '{' && text.IndexOf('}', i) is var k and > 0)
            {
                var key = text.AsSpan(i, k - i + 1);
                ReadOnlySpan<char> keyName;
                ReadOnlySpan<char> keyAction = ReadOnlySpan<char>.Empty;
                int indexSeparator = key.IndexOf(' ');
                if (indexSeparator > 0)
                {
                    keyName = key[1..indexSeparator];
                    keyAction = key[(indexSeparator + 1)..^1];
                }
                else
                    keyName = key[1..^1];

                if (VkDictionary.ContainsKey(keyName.ToString()))
                {
                    if (keyAction.IsEmpty || !ActionFlagDictionary.ContainsKey(keyAction.ToString()))
                        foreach (Input input in VkDictionary[keyName.ToString()].ToInputsPressKey())
                            yield return input;
                    else
                    {
                        KeyEventFlags extraFlags = 0;
                        if (ActionFlagDictionary.ContainsKey(keyAction.ToString()))
                            extraFlags |= ActionFlagDictionary[keyAction.ToString()];
                        if (FlagsDictionary.ContainsKey(keyName.ToString()))
                            extraFlags |= FlagsDictionary[keyName.ToString()];

                        yield return VkDictionary[keyName.ToString()].ToInput(extraFlags);
                    }

                    i = k;
                }
                continue;
            }

            foreach (Input input in text[i].ToInputsPressKey(extraInfo: extraInfo))
                yield return input;
        }
    }
}
