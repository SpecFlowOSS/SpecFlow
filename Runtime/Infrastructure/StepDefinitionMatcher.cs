using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.Infrastructure
{
    public interface IStepDefinitionMatcher
    {
        List<BindingMatch> GetMatches(StepArgs stepArgs);
        List<BindingMatch> GetMatchesWithoutScopeCheck(StepArgs stepArgs);
        List<BindingMatch> GetMatchesWithoutParamCheck(StepArgs stepArgs);
    }

    public class StepDefinitionMatcher : IStepDefinitionMatcher
    {
        private readonly IBindingRegistry bindingRegistry;
        private readonly IStepArgumentTypeConverter stepArgumentTypeConverter;
        private readonly IContextManager contextManager;

        public StepDefinitionMatcher(IBindingRegistry bindingRegistry, IStepArgumentTypeConverter stepArgumentTypeConverter, IContextManager contextManager)
        {
            this.bindingRegistry = bindingRegistry;
            this.contextManager = contextManager;
            this.stepArgumentTypeConverter = stepArgumentTypeConverter;
        }

        public List<BindingMatch> GetMatches(StepArgs stepArgs)
        {
            var matches = bindingRegistry
                .Where(b => b.Type == stepArgs.Type)
                .Select(binding => Match(binding, stepArgs, true, true))
                .Where(match => match != null)
                .ToList();

            if (matches.Count > 1)
            {
                // if there are both scoped and non-scoped matches, we take the ones with the higher degree of scope matches
                int maxScopeMatches = matches.Max(m => m.ScopeMatches);
                matches.RemoveAll(m => m.ScopeMatches < maxScopeMatches);
            }

            if (matches.Count > 1)
            {
                // we remove duplicate maches for the same method (take the first from each)
                matches = matches.GroupBy(m => m.StepBinding.MethodInfo, (methodInfo, methodMatches) => methodMatches.First()).ToList();
            }

            return matches;
        }

        public List<BindingMatch> GetMatchesWithoutParamCheck(StepArgs stepArgs)
        {
            return bindingRegistry.Where(b => b.Type == stepArgs.Type).Select(binding => Match(binding, stepArgs, false, true)).Where(match => match != null).ToList();
        }

        public List<BindingMatch> GetMatchesWithoutScopeCheck(StepArgs stepArgs)
        {
            return bindingRegistry.Where(b => b.Type == stepArgs.Type).Select(binding => Match(binding, stepArgs, true, false)).Where(match => match != null).ToList();
        }

        private static readonly object[] emptyExtraArgs = new object[0];
        private object[] CalculateExtraArgs(StepArgs stepArgs)
        {
            if (stepArgs.MultilineTextArgument == null && stepArgs.TableArgument == null)
                return emptyExtraArgs;

            var extraArgsList = new List<object>();
            if (stepArgs.MultilineTextArgument != null)
                extraArgsList.Add(stepArgs.MultilineTextArgument);
            if (stepArgs.TableArgument != null)
                extraArgsList.Add(stepArgs.TableArgument);
            return extraArgsList.ToArray();
        }

        private bool CanConvertArg(object value, Type typeToConvertTo)
        {
            Debug.Assert(value != null);
            Debug.Assert(typeToConvertTo != null);

            if (value.GetType().IsAssignableFrom(typeToConvertTo))
                return true;

            return stepArgumentTypeConverter.CanConvert(value, typeToConvertTo, contextManager.FeatureContext.BindingCulture);
        }

        private BindingMatch Match(StepDefinitionBinding stepDefinitionBinding, StepArgs stepArgs, bool useParamMatching, bool useScopeMatching)
        {
            Match match = stepDefinitionBinding.Regex.Match(stepArgs.Text);

            // Check if regexp is a match
            if (!match.Success)
                return null;

            int scopeMatches = 0;
            if (useScopeMatching && stepDefinitionBinding.IsScoped)
            {
                if (!stepDefinitionBinding.BindingScope.Match(stepArgs.StepContext, out scopeMatches))
                    return null;
            }

            var bindingMatch = new BindingMatch(stepDefinitionBinding, match, CalculateExtraArgs(stepArgs), stepArgs, scopeMatches);

            if (useParamMatching)
            {
                // check if the regex + extra arguments match to the binding method parameters
                if (bindingMatch.Arguments.Length != stepDefinitionBinding.ParameterTypes.Length)
                    return null;

                // Check if regex & extra arguments can be converted to the method parameters
                if (bindingMatch.Arguments.Where(
                    (arg, argIndex) => !CanConvertArg(arg, stepDefinitionBinding.ParameterTypes[argIndex])).Any())
                    return null;
            }
            return bindingMatch;
        }
    }
}
