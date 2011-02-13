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

        public string[] RegexArguments { get; private set; }
        public object[] Arguments  { get; private set; }

        public BindingMatch(StepBinding stepBinding, Match match, object[] extraArguments, StepArgs stepArgs, int scopeMatches)
        {
            if (stepBinding == null) throw new ArgumentNullException("stepBinding");
            if (match == null) throw new ArgumentNullException("match");
            if (extraArguments == null) throw new ArgumentNullException("extraArguments");
            if (stepArgs == null) throw new ArgumentNullException("stepArgs");

            StepBinding = stepBinding;
            Match = match;
            ExtraArguments = extraArguments;
            StepArgs = stepArgs;
            ScopeMatches = scopeMatches;

            RegexArguments = Match.Groups.Cast<Group>().Skip(1).Select(g => g.Value).ToArray();
            Arguments = RegexArguments.Concat(ExtraArguments).ToArray();
        }
    }
}