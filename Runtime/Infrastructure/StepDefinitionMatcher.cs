using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;

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
            var matches = bindingRegistry.GetConsideredStepDefinitions(stepArgs.StepDefinitionType, stepArgs.Text)
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
                matches = matches.GroupBy(m => m.StepBinding.Method, (methodInfo, methodMatches) => methodMatches.OrderByDescending(m => m.ScopeMatches).First(), BindingMethodComparer.Instance).ToList();
            }

            return matches;
        }

        public List<BindingMatch> GetMatchesWithoutParamCheck(StepArgs stepArgs)
        {
            return bindingRegistry.GetConsideredStepDefinitions(stepArgs.StepDefinitionType, stepArgs.Text).Select(binding => Match(binding, stepArgs, false, true)).Where(match => match != null).ToList();
        }

        public List<BindingMatch> GetMatchesWithoutScopeCheck(StepArgs stepArgs)
        {
            return bindingRegistry.GetConsideredStepDefinitions(stepArgs.StepDefinitionType, stepArgs.Text).Select(binding => Match(binding, stepArgs, true, false)).Where(match => match != null).ToList();
        }

        private object[] CalculateArguments(Match match, StepArgs stepArgs)
        {
            var regexArgs = match.Groups.Cast<Group>().Skip(1).Select(g => g.Value);
            var arguments = regexArgs.Cast<object>().ToList();
            if (stepArgs.MultilineTextArgument != null)
                arguments.Add(stepArgs.MultilineTextArgument);
            if (stepArgs.TableArgument != null)
                arguments.Add(stepArgs.TableArgument);

            return arguments.ToArray();
        }

        private bool CanConvertArg(object value, IBindingType typeToConvertTo)
        {
            if (typeToConvertTo.IsAssignableTo(value.GetType()))
                return true;

            return stepArgumentTypeConverter.CanConvert(value, typeToConvertTo, contextManager.FeatureContext.BindingCulture);
        }

        private BindingMatch Match(IStepDefinitionBinding stepDefinitionBinding, StepArgs stepArgs, bool useParamMatching, bool useScopeMatching)
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

            var bindingMatch = new BindingMatch(stepDefinitionBinding, scopeMatches, CalculateArguments(match, stepArgs), stepArgs.StepContext);

            if (useParamMatching)
            {
                var arguments = bindingMatch.Arguments;
                var bindingParameters = stepDefinitionBinding.Method.Parameters.ToArray();

                // check if the regex + extra arguments match to the binding method parameters
                if (arguments.Length != bindingParameters.Length)
                    return null;//BindingMatch.NonMatching;

                // Check if regex & extra arguments can be converted to the method parameters
                //if (arguments.Zip(bindingParameters, (arg, parameter) => CanConvertArg(arg, parameter.Type)).Any(canConvert => !canConvert))
                if (arguments.Where((arg, argIndex) => !CanConvertArg(arg, bindingParameters[argIndex].Type)).Any())
                    return null;//BindingMatch.NonMatching;
            }
            return bindingMatch;
        }
    }
}
