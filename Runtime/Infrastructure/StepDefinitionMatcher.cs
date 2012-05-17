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
        List<BindingMatch> GetMatches(StepInstance stepInstance);
        List<BindingMatch> GetMatchesWithoutScopeCheck(StepInstance stepInstance);
        List<BindingMatch> GetMatchesWithoutParamCheck(StepInstance stepInstance);
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

        public List<BindingMatch> GetMatches(StepInstance stepInstance)
        {
            var matches = bindingRegistry.GetConsideredStepDefinitions(stepInstance.StepDefinitionType, stepInstance.Text)
                .Select(binding => Match(binding, stepInstance, true, true))
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

        public List<BindingMatch> GetMatchesWithoutParamCheck(StepInstance stepInstance)
        {
            return bindingRegistry.GetConsideredStepDefinitions(stepInstance.StepDefinitionType, stepInstance.Text).Select(binding => Match(binding, stepInstance, false, true)).Where(match => match != null).ToList();
        }

        public List<BindingMatch> GetMatchesWithoutScopeCheck(StepInstance stepInstance)
        {
            return bindingRegistry.GetConsideredStepDefinitions(stepInstance.StepDefinitionType, stepInstance.Text).Select(binding => Match(binding, stepInstance, true, false)).Where(match => match != null).ToList();
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
            if (typeToConvertTo.IsAssignableTo(value.GetType()))
                return true;

            return stepArgumentTypeConverter.CanConvert(value, typeToConvertTo, contextManager.FeatureContext.BindingCulture);
        }

        private BindingMatch Match(IStepDefinitionBinding stepDefinitionBinding, StepInstance stepInstance, bool useParamMatching, bool useScopeMatching)
        {
            Match match = stepDefinitionBinding.Regex.Match(stepInstance.Text);

            // Check if regexp is a match
            if (!match.Success)
                return null;

            int scopeMatches = 0;
            if (useScopeMatching && stepDefinitionBinding.IsScoped)
            {
                if (!stepDefinitionBinding.BindingScope.Match(stepInstance.StepContext, out scopeMatches))
                    return null;
            }

            var bindingMatch = new BindingMatch(stepDefinitionBinding, scopeMatches, CalculateArguments(match, stepInstance), stepInstance.StepContext);

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
