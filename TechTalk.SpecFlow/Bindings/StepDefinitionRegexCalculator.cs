using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Bindings
{
    public interface IStepDefinitionRegexCalculator
    {
        string CalculateRegexFromMethod(StepDefinitionType stepDefinitionType, IBindingMethod bindingMethod);
    }

    public class StepDefinitionRegexCalculator : IStepDefinitionRegexCalculator
    {
        // for us, a word character means:
        // \w - letters (Ll, Lu, Lt, Lo, Lm), marks (Mn), numbers (Nd), connectors (Pc, like '_')
        // \p{Sc} - currency symbols (Sc)
        // minus sign

        // words can be connected with:
        // any non-word character (including empty)
        // but - sign can only stand at the end of the connecting text if the next character is not digit

        private const string WordConnectorRe = @"[^\w\p{Sc}]*(?!(?<=-)\d)";


        public StepDefinitionRegexCalculator(Configuration.SpecFlowConfiguration specFlowConfiguration)
        {
            this.specFlowConfiguration = specFlowConfiguration;
        }

        static private readonly Regex nonIdentifierRe = new Regex(@"[^\p{Ll}\p{Lu}\p{Lt}\p{Lm}\p{Lo}\p{Nl}\p{Nd}\p{Pc}\p{Mn}\p{Mc}]");
        public string CalculateRegexFromMethod(StepDefinitionType stepDefinitionType, IBindingMethod bindingMethod)
        {
            // if method name seems to contain regex, we use it as-is
            if (nonIdentifierRe.Match(bindingMethod.Name).Success)
                return bindingMethod.Name;

            string methodName = bindingMethod.Name;
            methodName = RemoveStepPrefix(stepDefinitionType, methodName);

            var parameters = bindingMethod.Parameters.ToArray();

            int processedPosition = 0;
            var reBuilder = new StringBuilder("(?i)");
            foreach (var paramPosition in parameters.Select((p, i) => CalculateParamPosition(methodName, p, i)).Where(pp => pp.Position >= 0).OrderBy(pp => pp.Position))
            {
                if (paramPosition.Position < processedPosition)
                    continue; //this is an error case -> overlapping parameters

                if (paramPosition.Position > processedPosition)
                { 
                    reBuilder.Append(CalculateWordRegex(methodName.Substring(processedPosition, paramPosition.Position - processedPosition)));
                    reBuilder.Append(WordConnectorRe);
                }
                reBuilder.Append(CalculateParamRegex(parameters[paramPosition.ParamIndex]));
                reBuilder.Append(WordConnectorRe);
                processedPosition = paramPosition.Position + paramPosition.Length;
            }

            reBuilder.Append(CalculateWordRegex(methodName.Substring(processedPosition)));
            reBuilder.Append(WordConnectorRe);

            return reBuilder.ToString();
        }

        private string RemoveStepPrefix(StepDefinitionType stepDefinitionType, string stepText)
        {
            var prefixesToRemove = GetPrefixesToRemove(stepDefinitionType);
            foreach (var prefixToRemove in prefixesToRemove)
            {
                if (stepText.StartsWith(prefixToRemove, StringComparison.CurrentCultureIgnoreCase))
                {
                    stepText = stepText.Substring(prefixToRemove.Length).TrimStart('_', ' ');
                    break; // we only stip one prefix
                }
            }
            return stepText;
        }

        private IEnumerable<string> GetPrefixesToRemove(StepDefinitionType stepDefinitionType)
        {
            yield return stepDefinitionType.ToString();

            var cultureToSearch = specFlowConfiguration.BindingCulture ?? specFlowConfiguration.FeatureLanguage;

            foreach (var keyword in LanguageHelper.GetKeywords(cultureToSearch, stepDefinitionType))
            {
                if (keyword.Contains(' '))
                {
                    yield return keyword.Replace(" ", "_");
                    yield return keyword.Replace(" ", "");
                }
                else
                {
                    yield return keyword;
                }
            }
        }

        private string CalculateParamRegex(IBindingParameter parameterInfo)
        {
            // parameters should match to the following
            // 1. quoted text: "..."
            // 2. apostrophed text: '...'
            // 3. longer text with lazy matching (as few as possible), to avoid "eating" whitespace before/after
            return string.Format(@"(?:""(?<{0}>[^""]*)""|'(?<{0}>[^']*)'|(?<{0}>.*?))", parameterInfo.ParameterName);
        }

        private static readonly Regex wordBoundaryRe = new Regex(@"_+|(?<=[\d\p{L}])(?=\p{Lu})|(?<=\p{L})(?=\d)"); //mathces on underscores and boundaries of: 0A, aA, AA, a0, A0
        private SpecFlowConfiguration specFlowConfiguration;

        private string CalculateWordRegex(string methodNamePart)
        {
            if (string.IsNullOrEmpty(methodNamePart))
                return string.Empty;

            return wordBoundaryRe.Replace(methodNamePart.Trim('_'), match => WordConnectorRe);
        }

        private class ParamSearchResult
        {
            public int Position { get; }
            public int Length { get; }
            public int ParamIndex { get; }

            public ParamSearchResult(int position, int length, int paramIndex)
            {
                Position = position;
                Length = length;
                ParamIndex = paramIndex;
            }
        }

        private ParamSearchResult CalculateParamPosition(string methodNamePart, IBindingParameter bindingParameter, int paramIndex)
        {
            string paramName = bindingParameter.ParameterName.ToUpper();
            var paramRegex = new Regex(string.Format(@"_?{0}_?|_?P{1}_?", paramName, paramIndex));
            var match = paramRegex.Match(methodNamePart);
            if (match.Success)
                return new ParamSearchResult(match.Index, match.Length, paramIndex);
            return new ParamSearchResult(-1, 0, paramIndex);
        }
    }
}