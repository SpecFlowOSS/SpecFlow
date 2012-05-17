using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow.Bindings
{
    public class BindingMatchService : IStepDefinitionMatchService
    {
        private readonly IBindingRegistry bindingRegistry;

        public bool Ready { get { return bindingRegistry.Ready; } }

        public BindingMatchService(IBindingRegistry bindingRegistry)
        {
            this.bindingRegistry = bindingRegistry;
        }

        public BindingMatch Match(IStepDefinitionBinding stepDefinitionBinding, StepInstance stepInstance, bool useRegexMatching = true, bool useParamMatching = true, bool useScopeMatching = true)
        {
            if (useParamMatching)
                useRegexMatching = true;

            Match match = null;
            if (useRegexMatching && stepDefinitionBinding.Regex != null && !(match = stepDefinitionBinding.Regex.Match(stepInstance.Text)).Success)
                return BindingMatch.NonMatching;

            if (stepDefinitionBinding.StepDefinitionType != stepInstance.StepDefinitionType)
                return BindingMatch.NonMatching;

            int scopeMatches = 0;
            if (useScopeMatching && stepDefinitionBinding.IsScoped && stepInstance.StepContext != null && !stepDefinitionBinding.BindingScope.Match(stepInstance.StepContext, out scopeMatches))
                return BindingMatch.NonMatching;

            Debug.Assert(match != null);
            var arguments = CalculateArguments(match, stepInstance);

            if (useParamMatching)
            {
                Debug.Assert(match != null);
                // check if the regex + extra arguments match to the binding method parameters
                if (arguments.Count() != stepDefinitionBinding.Method.Parameters.Count())
                    return BindingMatch.NonMatching;

                // Check if regex & extra arguments can be converted to the method parameters
                if (arguments.Zip(stepDefinitionBinding.Method.Parameters, (arg, parameter) => CanConvertArg(arg, parameter.Type)).Any(canConvert => !canConvert))
                    return BindingMatch.NonMatching;
            }

            return new BindingMatch(stepDefinitionBinding, scopeMatches, arguments, stepInstance.StepContext);
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

        private List<BindingMatch> GetCandidatingBindings(StepInstance stepInstance, bool useParamMatching = true)
        {
            var matches = bindingRegistry.GetConsideredStepDefinitions(stepInstance.StepDefinitionType, stepInstance.Text).Select(b => Match(b, stepInstance, useParamMatching: useParamMatching)).Where(b => b.Success);
            // we remove duplicate maches for the same method (take the highest scope matches from each)
            matches = matches.GroupBy(m => m.StepBinding.Method, (methodInfo, methodMatches) => methodMatches.OrderByDescending(m => m.ScopeMatches).First(), BindingMethodComparer.Instance);
            return matches.ToList();
        }

        public BindingMatch GetBestMatch(StepInstance stepInstance, out StepDefinitionAmbiguityReason ambiguityReason, out List<BindingMatch> candidatingBindings)
        {
            var matches = GetCandidatingBindings(stepInstance, useParamMatching: true).ToList();
            candidatingBindings = matches;
            ambiguityReason = StepDefinitionAmbiguityReason.None;
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
                return BindingMatch.NonMatching;
            }
            if (matches.Count > 1)
            {
                candidatingBindings = matches;
                return BindingMatch.NonMatching;
            }
            return matches[0];
        }
    }
}
