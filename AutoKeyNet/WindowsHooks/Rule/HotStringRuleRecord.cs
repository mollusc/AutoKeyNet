using System.Linq.Expressions;
using AutoKeyNet.WindowsHooks.WindowsEnums;
using System.Runtime.InteropServices;
using AutoKeyNet.WindowsHooks.WindowsStruct;
using AutoKeyNet.WindowsHooks.Helper;

namespace AutoKeyNet.WindowsHooks.Rule;

/// <summary>
/// Класс определяющий условия при вводе определенного набора символов (по типу автозамены)
/// </summary>
public sealed class HotStringRuleRecord : BaseRuleRecord
{
    public bool TriggerByEndingCharacter { get; }
    public HotStringRuleRecord(string keyWord, Action hotKeyRun, bool triggerByEndingCharacter = true, WindowCondition? checkWindowCondition = null)
        : base(keyWord, hotKeyRun, checkWindowCondition)
    {
        TriggerByEndingCharacter = triggerByEndingCharacter;
    }
    public HotStringRuleRecord(string keyWord, Func<string> replaceFunc, bool triggerByEndingCharacter = true, WindowCondition? checkWindowCondition = null)
        : base(keyWord, SendText(keyWord, replaceFunc, triggerByEndingCharacter), checkWindowCondition)
    {
        TriggerByEndingCharacter = triggerByEndingCharacter;
    }
    public HotStringRuleRecord(string keyWord, string replaceText, bool triggerByEndingCharacter = true, WindowCondition? checkWindowCondition = null)
        : base(keyWord, SendText(keyWord, replaceText, triggerByEndingCharacter), checkWindowCondition)
    {
        TriggerByEndingCharacter = triggerByEndingCharacter;
    }

    private static Action SendText(string keyWord, string replaceWord, bool triggerByEndingCharacter)
    {
        var inputs = GetInputs(keyWord, replaceWord, triggerByEndingCharacter);
        return () =>
        {
            SendInput(inputs);
        };
    }
    private static Action SendText(string keyWord, Func<string> replaceFunc, bool triggerByEndingCharacter)
    {
        return () =>
        {
            var inputs = GetInputs(keyWord, replaceFunc.Invoke(), triggerByEndingCharacter);
            SendInput(inputs);
        };
    }

    private static Input[] GetInputs(string keyWord, string replaceWord, bool triggerByEndingCharacter)
    {
        List<Input> inputList = new();
        if (triggerByEndingCharacter)
            inputList.AddRange(VirtualKey.LEFT.ToInputsPressKey());
        inputList.AddRange(SendBackspaces(keyWord.Length));
        inputList.AddRange(replaceWord.ToInputs());
        if (triggerByEndingCharacter)
            inputList.AddRange(VirtualKey.RIGHT.ToInputsPressKey());
        Input[] inputs = inputList.ToArray();
        return inputs;
    }

    /// <summary>
    /// Отправка заданного количества нажатия клавиши Backspace
    /// </summary>
    /// <param name="count">Количество нажатий клавиши Backspace</param>
    /// <param name="extraInfo">Отправка дополнительных параметров</param>
    private static Input[] SendBackspaces(int count) => Enumerable.Repeat(VirtualKey.BACK.ToInputsPressKey(), count).SelectMany(x => x).ToArray();
}