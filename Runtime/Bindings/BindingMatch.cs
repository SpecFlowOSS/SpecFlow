using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace TechTalk.SpecFlow.Bindings
{
    public class BindingMatch
    {
        public StepBinding StepBinding { get; private set; }
        public Match Match { get; private set; }
        public object[] ExtraArguments { get; private set; }
        public StepArgs StepArgs { get; private set; }
        public int ScopeMatches { get; set; }
        public bool IsScoped { get { return ScopeMatches > 0; } }

        public BindingMatch(StepBinding stepBinding, Match match, object[] extraArguments, StepArgs stepArgs, int scopeMatches)
        {
            StepBinding = stepBinding;
            Match = match;
            ExtraArguments = extraArguments;
            StepArgs = stepArgs;
            ScopeMatches = scopeMatches;
        }
    }
}