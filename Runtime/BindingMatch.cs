using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace TechTalk.SpecFlow
{
    internal class BindingMatch
    {
        public StepBinding StepBinding { get; private set; }
        public Match Match { get; private set; }
        public object[] ExtraArguments { get; private set; }
        public StepArgs StepArgs { get; private set; }

        public BindingMatch(StepBinding stepBinding, Match match, object[] extraArguments, StepArgs stepArgs)
        {
            StepBinding = stepBinding;
            Match = match;
            ExtraArguments = extraArguments;
            StepArgs = stepArgs;
        }
    }
}