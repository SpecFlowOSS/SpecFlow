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
        public bool IsScoped { get; private set; }

        public BindingMatch(StepBinding stepBinding, Match match, object[] extraArguments, StepArgs stepArgs, bool isScoped)
        {
            StepBinding = stepBinding;
            Match = match;
            ExtraArguments = extraArguments;
            StepArgs = stepArgs;
            IsScoped = isScoped;
        }
    }
}