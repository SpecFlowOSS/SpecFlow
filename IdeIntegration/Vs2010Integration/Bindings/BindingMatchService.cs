using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Vs2010Integration.Bindings.Reflection;

namespace TechTalk.SpecFlow.Vs2010Integration.Bindings
{
    public interface IBindingMatchService
    {
        bool Ready { get; }
        StepBinding GetBestMatchingBinding(StepInstance stepInstance, out IEnumerable<StepBinding> candidatingBindings);
    }

    public class BindingMatchService : IBindingMatchService
    {
        private readonly IBindingRegistry bindingRegistry;

        public bool Ready { get { return bindingRegistry.Ready; } }

        public BindingMatchService(IBindingRegistry bindingRegistry)
        {
            this.bindingRegistry = bindingRegistry;
        }

        public BindingMatch Match(StepBinding stepBinding, StepInstance stepInstance, bool useRegexMatching = true, bool useParamMatching = false, bool useScopeMatching = true)
        {
            if (useParamMatching)
                useRegexMatching = true;

            Match match = null;
            if (useRegexMatching && stepBinding.Regex != null && !(match = stepBinding.Regex.Match(stepInstance.Text)).Success)
                return BindingMatch.NonMatching;

            if (stepBinding.BindingType != stepInstance.BindingType)
                return BindingMatch.NonMatching;

            int scopeMatches = 0;
            if (useScopeMatching && stepBinding.IsScoped && stepInstance.StepScope != null && !stepBinding.BindingScope.Match(stepInstance.StepScope, out scopeMatches))
                return BindingMatch.NonMatching;

            if (useParamMatching)
            {
                Debug.Assert(match != null);
                var regexArgs = match.Groups.Cast<Group>().Skip(1).Select(g => g.Value).ToArray();
                var arguments = regexArgs /*+ extraArgs*/;
                // check if the regex + extra arguments match to the binding method parameters
                if (arguments.Count() != stepBinding.Method.Parameters.Count())
                    return BindingMatch.NonMatching;

                //TODO: proper param matching

                // Check if regex & extra arguments can be converted to the method parameters
                //                if (bindingMatch.Arguments.Where(
                //                    (arg, argIndex) => !CanConvertArg(arg, stepBinding.ParameterTypes[argIndex])).Any())
                //                    return null;
            }

            return new BindingMatch(stepBinding, scopeMatches);
        }

        private IEnumerable<BindingMatch> GetCandidatingBindings(StepInstance stepInstance)
        {
            var matches = bindingRegistry.GetConsideredBindings(stepInstance.Text).Select(b => Match(b, stepInstance)).Where(b => b.Success);
            // we remove duplicate maches for the same method (take the highest scope matches from each)
            matches = matches.GroupBy(m => m.StepBinding.Method, (methodInfo, methodMatches) => methodMatches.OrderByDescending(m => m.ScopeMatches).First(), BindingMethodComparer.Instance);
            return matches;
        }

        public StepBinding GetBestMatchingBinding(StepInstance stepInstance, out IEnumerable<StepBinding> candidatingBindings)
        {
            candidatingBindings = Enumerable.Empty<StepBinding>();
            var matches = GetCandidatingBindings(stepInstance).ToList();

            if (matches.Count > 1)
            {
                // if there are both scoped and non-scoped matches, we take the ones with the higher degree of scope matches
                int maxScopeMatches = matches.Max(m => m.ScopeMatches);
                matches.RemoveAll(m => m.ScopeMatches < maxScopeMatches);
            }

            if (matches.Count == 0)
            {
                return null;
            }
            if (matches.Count > 1)
            {
                candidatingBindings = matches.Select(m => m.StepBinding);
                return null;
            }
            return matches[0].StepBinding;
        }
    }
}
