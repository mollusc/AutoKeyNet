namespace AutoKeyNetApp.RuleFactory;

public struct AutoKeyRule
{
    public string KeyWord { get; set; }
    public string ReplaceText { get; set; }
    public bool TriggerByEndingCharacter { get; set; }
}