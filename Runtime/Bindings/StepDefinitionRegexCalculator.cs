using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace TechTalk.SpecFlow.Bindings
{
    public interface IStepDefinitionRegexCalculator
    {
        string CalculateRegexFromMethod(BindingType bindingType, MethodInfo methodInfo);
    }

    public class StepDefinitionRegexCalculator : IStepDefinitionRegexCalculator
    {
        static private readonly Regex nonIdentifierRe = new Regex(@"[^\p{Ll}\p{Lu}\p{Lt}\p{Lm}\p{Lo}\p{Nl}\p{Nd}\p{Pc}\p{Mn}\p{Mc}]");

        public string CalculateRegexFromMethod(BindingType bindingType, MethodInfo methodInfo)
        {
            // if method name seems to contain regex, we use it as-is
            if (nonIdentifierRe.Match(methodInfo.Name).Success)
            {
                return methodInfo.Name;
            }

            string prefixToRemove = bindingType.ToString();
            string stepText = methodInfo.Name;
            if (stepText.StartsWith(prefixToRemove, StringComparison.CurrentCultureIgnoreCase))
                stepText = stepText.Substring(prefixToRemove.Length).TrimStart('_', ' ');

            var parameters = methodInfo.GetParameters();

            int processedPosition = 0;
            var reBuilder = new StringBuilder("(?i)");
            foreach (var paramPosition in parameters.Select((p, i) => CalculateParamPosition(stepText, p, i)).Where(pp => pp.Position >= 0).OrderBy(pp => pp.Position))
            {
                if (paramPosition.Position < processedPosition)
                    continue; //this is an error case -> overlapping parameters

                reBuilder.Append(CalculateRegex(stepText.Substring(processedPosition, paramPosition.Position - processedPosition)));
                reBuilder.Append(CalculateParamRegex(parameters[paramPosition.ParamIndex]));
                processedPosition = paramPosition.Position + paramPosition.Lenght;
            }

            reBuilder.Append(CalculateRegex(stepText.Substring(processedPosition, stepText.Length - processedPosition)));
            reBuilder.Append(@"\W*");

//            throw new Exception("XXX" + reBuilder);

            return reBuilder.ToString();
        }

        private string CalculateParamRegex(ParameterInfo parameterInfo)
        {
            //return string.Format(@"(?:[""](?<{0}>.*[^""])[""]|(?:['](?<{0}>.*[^'])[']|(?<{0}>.+))", parameterInfo.Name);
            return string.Format(@"(?:['""](?<{0}>.*[^'""])['""]|(?<{0}>.+))", parameterInfo.Name);
        }

        private static readonly Regex wordBoundaryRe = new Regex(@"(?<=[\d\p{L}])\p{Lu}|(?<=\p{L})\d"); //mathces on boundaries of: 0A, aA, AA, a0, A0

        private string CalculateRegex(string text)
        {
            text = wordBoundaryRe.Replace(text, match => @"\W*" + match.Value[0]);
            return text.Replace("_", @"\W+");
        }

        private class ParamSearchResult
        {
            public int Position { get; private set; }
            public int Lenght { get; private set; }
            public int ParamIndex { get; private set; }

            public ParamSearchResult(int position, int lenght, int paramIndex)
            {
                Position = position;
                Lenght = lenght;
                ParamIndex = paramIndex;
            }
        }

        private int IndexOfWithUnderscores(string text, string textToFind)
        {
            int index = ("_" + text + "_").IndexOf("_" + textToFind + "_");
            return index;
        }

        private ParamSearchResult CalculateParamPosition(string stepText, ParameterInfo parameterInfo, int paramIndex)
        {
            string paramName = parameterInfo.Name.ToUpper();
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