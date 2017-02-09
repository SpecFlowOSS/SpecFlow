using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        private const string nonWordRe = @"[^\w\p{Sc}]*(?!(?<=-)\d)";

        private readonly RuntimeConfiguration runtimeConfiguration;

        public StepDefinitionRegexCalculator(RuntimeConfiguration runtimeConfiguration)
        {
            this.runtimeConfiguration = runtimeConfiguration;
        }

        static private readonly Regex nonIdentifierRe = new Regex(@"[^\p{Ll}\p{Lu}\p{Lt}\p{Lm}\p{Lo}\p{Nl}\p{Nd}\p{Pc}\p{Mn}\p{Mc}]");
        public string CalculateRegexFromMethod(StepDefinitionType stepDefinitionType, IBindingMethod bindingMethod)
        {
            // if method name seems to contain regex, we use it as-is
            if (nonIdentifierRe.Match(bindingMethod.Name).Success)
            {
                return bindingMethod.Name;
            }

            string stepText = bindingMethod.Name;
            stepText = RemoveStepPrefix(stepDefinitionType, stepText);

            var parameters = bindingMethod.Parameters.ToArray();

            int processedPosition = 0;
            var reBuilder = new StringBuilder("(?i)");
            foreach (var paramPosition in parameters.Select((p, i) => CalculateParamPosition(stepText, p, i)).Where(pp => pp.Position >= 0).OrderBy(pp => pp.Position))
            {
                if (paramPosition.Position < processedPosition)
                    continue; //this is an error case -> overlapping parameters

                reBuilder.Append(CalculateRegex(stepText.Substring(processedPosition, paramPosition.Position - processedPosition)));
                reBuilder.Append(CalculateParamRegex(parameters[paramPosition.ParamIndex]));
                processedPosition = paramPosition.Position + paramPosition.Length;
            }

            reBuilder.Append(CalculateRegex(stepText.Substring(processedPosition, stepText.Length - processedPosition)));

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

            var cultureToSearch = runtimeConfiguration.BindingCulture ?? runtimeConfiguration.FeatureLanguage;

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

        private string CalculateRegex(string text)
        {
            if (string.IsNullOrEmpty(text))
                return nonWordRe;

            return wordBoundaryRe.Replace("_" + text + "_", match => nonWordRe);
        }

        private class ParamSearchResult
        {
            public int Position { get; private set; }
            public int Length { get; private set; }
            public int ParamIndex { get; private set; }

            public ParamSearchResult(int position, int length, int paramIndex)
            {
                Position = position;
                Length = length;
                ParamIndex = paramIndex;
            }
        }

        private int IndexOfWithUnderscores(string text, string textToFind)
        {
            int index = ("_" + text + "_").IndexOf("_" + textToFind + "_");
            return index;
        }

        private ParamSearchResult CalculateParamPosition(string stepText, IBindingParameter bindingParameter, int paramIndex)
        {
            string paramName = bindingParameter.ParameterName.ToUpper();
            int result = IndexOfWithUnderscores(stepText, paramName);
            if (result >= 0)
                return new ParamSearchResult(result, paramName.Length, paramIndex);
                
            result = stepText.IndexOf(paramName);
            if (result >= 0)
                return new ParamSearchResult(result, paramName.Length, paramIndex);

            string paramReference = string.Format("P{0}", paramIndex);
            result = IndexOfWithUnderscores(stepText, paramReference);
            if (result >= 0)
                return new ParamSearchResult(result, paramReference.Length, paramIndex);

            return new ParamSearchResult(-1, 0, paramIndex);
        }

    }
}