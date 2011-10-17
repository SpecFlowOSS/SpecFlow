using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace TechTalk.SpecFlow.Bindings
{
    public class BindingMatch
    {
        public IStepDefinitionBinding StepBinding { get; private set; }
        public StepArgs StepArgs { get; private set; }

        public string[] RegexArguments { get; private set; }
        public object[] ExtraArguments { get; private set; }

        public int ScopeMatches { get; set; }
        public bool IsScoped { get { return ScopeMatches > 0; } }

        public object[] Arguments  { get; private set; }

        public BindingMatch(IStepDefinitionBinding stepDefinitionBinding, StepArgs stepArgs, string[] regexArguments, object[] extraArguments, int scopeMatches)
        {
            if (stepDefinitionBinding == null) throw new ArgumentNullException("stepDefinitionBinding");
            if (stepArgs == null) throw new ArgumentNullException("stepArgs");
            if (regexArguments == null) throw new ArgumentNullException("regexArguments");
            if (extraArguments == null) throw new ArgumentNullException("extraArguments");

            StepBinding = stepDefinitionBinding;
            StepArgs = stepArgs;

            RegexArguments = regexArguments;
            ExtraArguments = extraArguments;

            ScopeMatches = scopeMatches;

            Arguments = RegexArguments.Concat(ExtraArguments).ToArray();
        }

        public BindingMatch(IStepDefinitionBinding stepDefinitionBinding, Match match, object[] extraArguments, StepArgs stepArgs, int scopeMatches) :
            this(stepDefinitionBinding, stepArgs, match.Groups.Cast<Group>().Skip(1).Select(g => g.Value).ToArray(), extraArguments, scopeMatches)

        {
        }
    }
}