using AutoKeyNet.WindowsHooks.Rule;
using Microsoft.Extensions.Configuration;

namespace AutoKeyNetApp.RuleFactory;

/// <summary>
///     Base class for creating rules.
/// </summary>
internal abstract class BaseRuleFactory
{
    /// <summary>
    ///     Configuration of rules
    /// </summary>
    protected IConfiguration Configuration;

    /// <summary>
    ///     Constructor of base class for creating rules
    /// </summary>
    protected BaseRuleFactory()
    {
        Configuration = new ConfigurationBuilder()
            .AddJsonFile("AutoKeySettings.json", true).Build();
    }

    /// <summary>
    ///     Create rules
    /// </summary>
    /// <returns>List of rules</returns>
    public abstract List<BaseRuleRecord> Create();
}