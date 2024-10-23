using System.Diagnostics;
using AutoKeyNet.WindowsHooks.Facades;
using AutoKeyNet.WindowsHooks.Rule;
using AutoKeyNetApp.RuleFactory;

namespace AutoKeyNetApp;

internal static class Program
{
    /// <summary>
    ///     List of rules
    /// </summary>
    private static readonly List<BaseRuleRecord> Rules = new();

    /// <summary>
    ///     The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        try
        {
            InitializeRules();
            using var autoKeyNet = new AutoKeyNetFacade(Rules);
            Application.Run();
        }
        catch (Exception e)
        {
            Trace.TraceError(e.Message);
        }
    }

    /// <summary>
    ///     Initialize all rules
    /// </summary>
    private static void InitializeRules()
    {
        BaseRuleFactory[] ruleFactories =
        {
            new HotStringRuleFactory(),
            new HotKeyLButtonRuleFactory(),
            new HotKeyProgramRuleFactory(),
            new OutlookRuleFactory(),
        };

        foreach (var ruleFactory in ruleFactories)
            Rules.AddRange(ruleFactory.Create());
    }
}