using AutoKeyNet.WindowsHooks.Rule;
using Microsoft.Extensions.Configuration;

namespace AutoKeyNetApp.RuleFactory;

/// <summary>
///     Class for creation HotStrings
/// </summary>
internal class HotStringRuleFactory : BaseRuleFactory
{
    /// <summary>
    ///     List of HotStrings loaded from configuration
    /// </summary>
    private readonly IEnumerable<HotStringRule>? _hotStrings;

    /// <summary>
    ///     List of employees name
    /// </summary>
    private readonly IEnumerable<string>? _names;

    /// <summary>
    ///     Constructor for HotString rules that loads information about employees from configuration.
    /// </summary>
    public HotStringRuleFactory()
    {
        _names = Configuration.GetSection("Names").Get<string[]>();
        _hotStrings = Configuration.GetSection("HotStrings").Get<HotStringRule[]>();
    }

    public override List<BaseRuleRecord> Create()
    {
        var rules = new List<BaseRuleRecord>
        {
            new HotStringRuleRecord("ддд", "Добрый день!"),
            new HotStringRuleRecord("сс", "Согласовано"),
            new HotStringRuleRecord("пжст", "{Left}, пожалуйста,{Right}", false),
            new HotStringRuleRecord("ээ", "«»{Left}", false),
            new HotStringRuleRecord("жж", "↓", false),
            new HotStringRuleRecord("зз", "→", false),
            new HotStringRuleRecord("ыа", "откого:(){Left}", false),
            new HotStringRuleRecord("ые", "имяполучателя:(){Left}", false),
            new HotStringRuleRecord("htd",
                () => DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")), // Insert the current date and time
            new HotStringRuleRecord("hd", () => DateTime.Now.ToString("yyyy-MM-dd")) // Insert the current date
        };
        rules.AddRange(GetNameRulesFromConfiguration());
        rules.AddRange(GetHotStringRulesFromConfiguration());
        return rules;
    }

    /// <summary>
    ///     Get HotStrings from configuration
    /// </summary>
    /// <returns>List of HotStrings</returns>
    private List<BaseRuleRecord> GetHotStringRulesFromConfiguration()
    {
        List<BaseRuleRecord> rules = new();
        if (_hotStrings == null)
            return rules;
        foreach (var s in _hotStrings)
            rules.Add(new HotStringRuleRecord(s.KeyText, s.ReplaceText, s.TriggerByEndingCharacter));
        return rules;
    }

    /// <summary>
    /// Create HotStrings from the list of employees
    /// </summary>
    /// <returns>List of HotStrings</returns>
    private List<BaseRuleRecord> GetNameRulesFromConfiguration()
    {
        List<BaseRuleRecord> rules = new();
        if (_names == null)
            return rules;
        foreach (var s in _names)
            for (var i = 1; i < s.Length; i++)
                if (_names.Count(x => x.StartsWith(s[..i])) == 1)
                {
                    var names = s.Split(' ');
                    rules.Add(new HotStringRuleRecord("фио" + s[..i], s, false));
                    rules.Add(new HotStringRuleRecord("ф" + s[..i],
                        $"{names[0]} {names[1].FirstOrDefault()}.{names[2].FirstOrDefault()}.", false));
                    rules.Add(new HotStringRuleRecord("дд" + s[..i], $"Добрый день, {names[1]} {names[2]}!", false));
                    break;
                }

        return rules;
    }
}