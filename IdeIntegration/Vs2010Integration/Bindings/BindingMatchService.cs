using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings
{
    public interface IBindingMatchService
    {
        bool Ready { get; }
        IStepDefinitionBinding GetBestMatchingBinding(StepInstance stepInstance, out IEnumerable<IStepDefinitionBinding> candidatingBindings);
        BindingMatch Match(IStepDefinitionBinding stepBinding, StepInstance stepInstance, bool useRegexMatching = true, bool useParamMatching = true, bool useScopeMatching = true);
    }

    public class BindingMatchService : IBindingMatchService
    {
        private readonly IBindingRegistry bindingRegistry;

        public bool Ready { get { return bindingRegistry.Ready; } }

        public BindingMatchService(IBindingRegistry bindingRegistry)
        {
            this.bindingRegistry = bindingRegistry;
        }

        public BindingMatch Match(IStepDefinitionBinding stepBinding, StepInstance stepInstance, bool useRegexMatching = true, bool useParamMatching = true, bool useScopeMatching = true)
        {
            if (useParamMatching)
                useRegexMatching = true;

            Match match = null;
            if (useRegexMatching && stepBinding.Regex != null && !(match = stepBinding.Regex.Match(stepInstance.Text)).Success)
                return BindingMatch.NonMatching;

            if (stepBinding.StepDefinitionType != stepInstance.StepDefinitionType)
                return BindingMatch.NonMatching;

            int scopeMatches = 0;
            if (useScopeMatching && stepBinding.IsScoped && stepInstance.StepContext != null && !stepBinding.BindingScope.Match(stepInstance.StepContext, out scopeMatches))
                return BindingMatch.NonMatching;

            Debug.Assert(match != null);
            var arguments = CalculateArguments(match, stepInstance);

            if (useParamMatching)
            {
                Debug.Assert(match != null);
                // check if the regex + extra arguments match to the binding method parameters
                if (arguments.Count() != stepBinding.Method.Parameters.Count())
                    return BindingMatch.NonMatching;

                // Check if regex & extra arguments can be converted to the method parameters
                if (arguments.Zip(stepBinding.Method.Parameters, (arg, parameter) => CanConvertArg(arg, parameter.Type)).Any(canConvert => !canConvert))
                    return BindingMatch.NonMatching;
            }

            return new BindingMatch(stepBinding, scopeMatches, arguments, stepInstance.StepContext);
        }

        private object[] CalculateArguments(Match match, StepInstance stepInstance)
        {
            var regexArgs = match.Groups.Cast<Group>().Skip(1).Select(g => g.Value);
            var arguments = regexArgs.Cast<object>().ToList();
            if (stepInstance.MultilineTextArgument != null)
                arguments.Add(stepInstance.MultilineTextArgument);
            if (stepInstance.TableArgument != null)
                arguments.Add(stepInstance.TableArgument);

            return arguments.ToArray();
        }

        private bool CanConvertArg(object value, IBindingType typeToConvertTo)
        {
            Debug.Assert(value != null);
            Debug.Assert(typeToConvertTo != null);

            if (typeToConvertTo.IsAssignableTo(value.GetType()))
                return true;

            //TODO: proper param matching
            //return stepArgumentTypeConverter.CanConvert(value, typeToConvertTo, FeatureContext.Current.BindingCulture);
            return false;
        }

        private IEnumerable<BindingMatch> GetCandidatingBindings(StepInstance stepInstance, bool useParamMatching = true)
        {
            var matches = bindingRegistry.GetConsideredStepDefinitions(stepInstance.StepDefinitionType, stepInstance.Text).Select(b => Match(b, stepInstance, useParamMatching: useParamMatching)).Where(b => b.Success);
            // we remove duplicate maches for the same method (take the highest scope matches from each)
            matches = matches.GroupBy(m => m.StepBinding.Method, (methodInfo, methodMatches) => methodMatches.OrderByDescending(m => m.ScopeMatches).First(), BindingMethodComparer.Instance);
            return matches;
        }

        public IStepDefinitionBinding GetBestMatchingBinding(StepInstance stepInstance, out IEnumerable<IStepDefinitionBinding> candidatingBindings)
        {
            candidatingBindings = Enumerable.Empty<IStepDefinitionBinding>();
            var matches = GetCandidatingBindings(stepInstance, useParamMatching: true).ToList();
            if (matches.Count == 0)
            {
                //HACK: since out param matching does not support agrument converters yet, we rather show more results than "no match"
                matches = GetCandidatingBindings(stepInstance, useParamMatching: false).ToList();
            }

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
