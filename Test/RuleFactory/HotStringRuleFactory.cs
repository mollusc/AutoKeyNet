using AutoKeyNet.WindowsHooks.Rule;
using Microsoft.Extensions.Configuration;

namespace AutoKeyNetApp.RuleFactory;

internal class HotStringRuleFactory : BaseRuleFactory
{
    private readonly IEnumerable<string>? _names;
    private readonly IEnumerable<AutoKeyRule>? _hotStrings;

    public HotStringRuleFactory()
    {
        _names = Configuration.GetSection("Names").Get<string[]>();
        _hotStrings = Configuration.GetSection("HotStrings").Get<AutoKeyRule[]>();
    }
    public override List<BaseRuleRecord> Create()
    {
        var rules = new List<BaseRuleRecord>()
        {
            new HotStringRuleRecord("дд", "Добрый день!"),
            new HotStringRuleRecord("сс", "Согласовано"),
            new HotStringRuleRecord("пжст", "{Left}, пожалуйста,{Right}", false),
            new HotStringRuleRecord("ээ", "«»{Left}", false),
            new HotStringRuleRecord("жж", "↓", false),
            new HotStringRuleRecord("зз", "→", false),
            new HotStringRuleRecord("ыа", "откого:(){Left}", false),
            new HotStringRuleRecord("ые", "имяполучателя:(){Left}", false),
            new HotStringRuleRecord("htd",
                () => DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")), // Вставка даты и время
            new HotStringRuleRecord("hd", () => DateTime.Now.ToString("yyyy-MM-dd")), // Вставка даты
        };
        rules.AddRange(GetNameRulesFromConfiguration());
        rules.AddRange(GetHotStringRulesFromConfiguration());
        return rules;
    }

    private List<BaseRuleRecord> GetHotStringRulesFromConfiguration()
    {
        List<BaseRuleRecord> rules = new();
        if (_hotStrings == null)
            return rules;
        foreach (AutoKeyRule s in _hotStrings)
            rules.Add(new HotStringRuleRecord(s.KeyWord, s.ReplaceText, s.TriggerByEndingCharacter));
        return rules;
    }

    private List<BaseRuleRecord> GetNameRulesFromConfiguration()
    {
        List<BaseRuleRecord> rules = new();
        if (_names == null)
            return rules;
        foreach (string s in _names)
        {
            for (int i = 1; i < s.Length; i++)
            {
                if (_names.Count(x => x.StartsWith(s[..i])) == 1)
                {
                    var names = s.Split(' ');
                    rules.Add(new HotStringRuleRecord("фио" + s[..i], s, false));
                    rules.Add(new HotStringRuleRecord("ф" + s[..i], $"{names[0]} {names[1].FirstOrDefault()}.{names[2].FirstOrDefault()}.", false));
                    break;
                }
            }
        }
        return rules;
    }
}