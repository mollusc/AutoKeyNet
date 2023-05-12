using AutoKeyNet.WindowsHooks.Rule;

namespace AutoKeyNetApp.RuleFactory;

/// <summary>
/// Создание комбинаций с левой клавишей мыши
/// </summary>
internal class HotKeyLButtonRuleFactory : BaseRuleFactory
{
    public override List<BaseRuleRecord> Create()
    {
        var rules = new List<BaseRuleRecord>()
        {
            new HotKeyRuleRecord("{LBUTTON DOWN}{KEY_C DOWN}", "{CONTROL DOWN}{KEY_C DOWN}{KEY_C UP}{CONTROL UP}"), // Копирование текста
            new HotKeyRuleRecord("{LBUTTON DOWN}{KEY_V DOWN}", "{CONTROL DOWN}{KEY_V DOWN}{KEY_V UP}{CONTROL UP}"), // Вставка текста
            new HotKeyRuleRecord("{LBUTTON DOWN}{KEY_B DOWN}", PasteWithoutFormat), // Удаление форматирования текста в буфере и вставка текста
            new HotKeyRuleRecord("{LBUTTON DOWN}{KEY_X DOWN}", "{CONTROL DOWN}{KEY_X DOWN}{KEY_X UP}{CONTROL UP}"), // Вырезать текст
            new HotKeyRuleRecord("{LBUTTON DOWN}{KEY_F DOWN}", "{CONTROL DOWN}{KEY_B DOWN}{KEY_B UP}{CONTROL UP}"), // Сделать текст жирным
            new HotKeyRuleRecord("{LBUTTON DOWN}{KEY_D DOWN}", "{CONTROL DOWN}{KEY_I DOWN}{KEY_I UP}{CONTROL UP}"), // Сделать текст курсивом
            new HotKeyRuleRecord("{LBUTTON DOWN}{KEY_G DOWN}", "{CONTROL DOWN}{KEY_U DOWN}{KEY_U UP}{CONTROL UP}"), // Сделать текст подчеркнутым
        };
        return rules;
    }

    private static string PasteWithoutFormat()
    {
        Clipboard.SetText(Clipboard.GetText(TextDataFormat.UnicodeText));
        return "{CONTROL DOWN}{KEY_V DOWN}{KEY_V UP}{CONTROL UP}";
    }
}