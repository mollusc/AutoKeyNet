using System.Diagnostics;
using System.Runtime.InteropServices;
using AutoKeyNet.WindowsHooks.Helper;
using AutoKeyNet.WindowsHooks.WindowsEnums;
using AutoKeyNet.WindowsHooks.WindowsStruct;
using static AutoKeyNet.WindowsHooks.WinApi.NativeMethods;

namespace AutoKeyNet.WindowsHooks.Rule;

/// <summary>
/// Базовый класс по проверке срабатывания правил
/// </summary>
public class BaseRuleRecord
{
    /// <summary>
    /// Ключевое слово по которому срабатывает правило
    /// </summary>
    public string KeyWord { get; }

    /// <summary>
    /// Клавиши нажатые одновременно
    /// </summary>
    internal Input[] InputKeys { get; }


    /// <summary>
    /// Действие правила 
    /// </summary>
    public Action Run { get; }

    /// <summary>
    /// Делегат проверяющий условия относящимся к текущему окну
    /// </summary>
    /// <param name="windowTitle">Заголовок текущего окна</param>
    /// <param name="windowClass">Класс текущего окна</param>
    /// <param name="windowModule">Название исполняющего файла относящегося к текущему окну</param>
    /// <param name="windowControl">Название элемента интерфейса текущего окна</param>
    /// <returns></returns>
    public delegate bool WindowCondition(string? windowTitle, string? windowClass, string? windowModule, string? windowControl);

    /// <summary>
    /// Проверка условия относящемся к текущему окну
    /// </summary>
    public WindowCondition? CheckWindowCondition;

    /// <summary>
    /// Конструктор базового класса по проверке срабатывания правил
    /// </summary>
    /// <param name="keyWord">Строка определяющая условия срабатывания правила</param>
    /// <param name="run">Действия выполняемое при соблюдении условий</param>
    /// <param name="checkWindowCondition">Проверка условия относящаяся к текущему окну</param>
    protected BaseRuleRecord(string keyWord, Action run, WindowCondition? checkWindowCondition)
    {
        KeyWord = keyWord;
        InputKeys = keyWord.ToInputs().ToArray();
        //if (releaseKeysBeforeRun)
        //{
        //    Run = () =>
        //    {
        //        List<Input> listInputs = new List<Input>(ReleaseKeys(keyWord));
        //        Input[] inputs = listInputs.ToArray();
        //        SendInput(inputs);
        //        run.Invoke();
        //    };
        //}
        //else
            Run = run;
        CheckWindowCondition = checkWindowCondition;
    }

    protected static Action SendText(string replaceWord)
    {
        var inputs = replaceWord.ToInputs().ToArray();
        return () =>
        {
            SendInput(inputs);
        };
    }
    protected static Action SendText(Func<string> replaceFunc)
    {
        return () =>
        {
            var inputs = replaceFunc.Invoke().ToInputs().ToArray();
            SendInput(inputs);
        };
    }

    protected static Action SendText(Func<string> replaceFunc, string releaseKeys)
    {
        return () =>
        {
            List<Input> listInputs = new List<Input>(ReleaseKeys(releaseKeys));
            listInputs.AddRange(replaceFunc.Invoke().ToInputs().ToArray());
            Input[] inputs = listInputs.ToArray();
            SendInput(inputs);
        };
    }

    protected static Action SendText(string replaceWord, string releaseKeys)
    {
        List<Input> listInputs = new List<Input>(ReleaseKeys(releaseKeys));
        listInputs.AddRange(replaceWord.ToInputs());
        Input[] inputs = listInputs.ToArray();
        return () =>
        {
            SendInput(inputs);
        };
    }

    internal static IEnumerable<Input> ReleaseKeys(string keys)
    {
        var inputs = keys.ToInputs().ToArray();
        for (var i = 0; i < inputs.Length; i++)
        {
            if (inputs[i].type == InputType.INPUT_KEYBOARD)
                inputs[i].U.ki.dwFlags = KeyEventFlags.KEYUP;
            else if (inputs[i].type == InputType.INPUT_MOUSE)
                inputs[i].U.mi.dwFlags = (MouseEvents)((int)inputs[i].U.mi.dwFlags << 1);
        }

        return inputs;
    }
}