using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoKeyNet.WindowsHooks.Hooks;
using AutoKeyNet.WindowsHooks.Rule;

namespace AutoKeyNet.WindowsHooks.Facades;
/// <summary>
/// Базовый класс по обработке горячих клавиш
/// </summary>
internal abstract class BaseKeyFacade
{

    /// <summary>
    /// Перечень критериев по срабатыванию горячих клавиш
    /// </summary>
    protected IEnumerable<BaseRuleRecord> Rules;

    /// <summary>
    /// Конструктор класса по обработке горячих клавиш
    /// </summary>
    /// <param name="rules">Перечень критериев по срабатыванию горячих клавиш</param>
    protected BaseKeyFacade(IEnumerable<BaseRuleRecord> rules)
    {
        Rules = rules;
    }
}
