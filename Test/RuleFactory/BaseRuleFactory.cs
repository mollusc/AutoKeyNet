using AutoKeyNet.WindowsHooks.Rule;
using Microsoft.Extensions.Configuration;

namespace Test.RuleFactory
{
    internal abstract class BaseRuleFactory
    {
        protected IConfiguration Configuration;

        protected BaseRuleFactory()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("AutoKeySettings.json", true).Build();
        }
        public abstract List<BaseRuleRecord> Create();
    }
}
