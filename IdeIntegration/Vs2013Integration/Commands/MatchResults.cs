using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;

namespace TechTalk.SpecFlow.Vs2010Integration.Commands
{
    public enum ExitReason
    {
        NoCurrentStep,
        BindingServiceNotReady,
        NoMatchFound,
        Success
    }

    public class MatchResults
    {
        public readonly ExitReason ExitReason;
        public IEnumerable<BindingMatch> CandidatingMatches;
        public readonly CultureInfo BindingCulture;
        public readonly BindingMatch BindingMatch;
        public readonly GherkinStep Step;

        public MatchResults(ExitReason exitReason)
        {
            this.ExitReason = exitReason;
        }

        public MatchResults(List<BindingMatch> candidatingMatches, BindingMatch bindingMatch)
            : this(ExitReason.Success)
        {
            this.CandidatingMatches = candidatingMatches;
            this.BindingMatch = bindingMatch;
        }

        public MatchResults(CultureInfo bindingCulture, GherkinStep step)
            : this(ExitReason.NoMatchFound)
        {
            BindingCulture = bindingCulture;
            Step = step;
        }
    }
}
