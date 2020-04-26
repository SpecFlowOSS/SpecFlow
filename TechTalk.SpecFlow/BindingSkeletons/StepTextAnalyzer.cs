using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TechTalk.SpecFlow.BindingSkeletons
{
    public class AnalyzedStepText
    {
        public readonly List<string> TextParts = new List<string>();
        public readonly List<AnalyzedStepParameter> Parameters = new List<AnalyzedStepParameter>();
    }

    public class AnalyzedStepParameter
    {
        public readonly string Type;
        public readonly string Name;
        public readonly string RegexPattern;

        public AnalyzedStepParameter(string type, string name, string regexPattern = null)
        {
            this.Type = type;
            this.Name = name;
            this.RegexPattern = regexPattern;
        }
    }

    public interface IStepTextAnalyzer
    {
        AnalyzedStepText Analyze(string stepText, CultureInfo bindingCulture);
    }

    public class StepTextAnalyzer : IStepTextAnalyzer
    {
        private List<string> usedParameterNames = new List<string>();
        public AnalyzedStepText Analyze(string stepText, CultureInfo bindingCulture)
        {
            var result = new AnalyzedStepText();

            var paramMatches = RecognizeQuotedTexts(stepText)
                               .Concat(RecognizeDates(stepText))
                               .Concat(RecognizeIntegers(stepText))
                               .Concat(RecognizeDecimals(stepText, bindingCulture))
                               .OrderBy(m => m.Capture.Index)
                               .ThenByDescending(m => m.Capture.Length);

            int textIndex = 0;
            foreach (var paramMatch in paramMatches)
            {
                if (paramMatch.Capture.Index < textIndex)
                    continue;

                const string singleQuoteRegexPattern = "[^']*";
                const string doubleQuoteRegexPattern = "[^\"\"]*";
                const string defaultRegexPattern = ".*";

                string regexPattern = defaultRegexPattern;
                string value = paramMatch.Capture.Value;
                int index = paramMatch.Capture.Index;

                switch (value.Substring(0, 1))
                {
                    case "\"":
                        regexPattern = doubleQuoteRegexPattern;
                        value = value.Substring(1, value.Length - 2);
                        index++;
                        break;
                    case "'":
                        regexPattern = singleQuoteRegexPattern;
                        value = value.Substring(1, value.Length - 2);
                        index++;
                        break;
                }

                result.TextParts.Add(stepText.Substring(textIndex, index - textIndex));
                result.Parameters.Add(AnalyzeParameter(value, bindingCulture, result.Parameters.Count, regexPattern, paramMatch.ParameterType));
                textIndex = index + value.Length;
            }

            result.TextParts.Add(stepText.Substring(textIndex));
            return result;
        }

        private AnalyzedStepParameter AnalyzeParameter(string value, CultureInfo bindingCulture, int paramIndex, string regexPattern, ParameterType parameterType)
        {
            string paramName = StepParameterNameGenerator.GenerateParameterName(value, paramIndex, usedParameterNames);

            int intParamValue;
            if (parameterType == ParameterType.Int && int.TryParse(value, NumberStyles.Integer, bindingCulture, out intParamValue))
                return new AnalyzedStepParameter("Int32", paramName, regexPattern);

            decimal decimalParamValue;
            if (parameterType == ParameterType.Decimal && decimal.TryParse(value, NumberStyles.Number, bindingCulture, out decimalParamValue))
                return new AnalyzedStepParameter("Decimal", paramName, regexPattern);

            DateTime dateParamValue;
            if (parameterType == ParameterType.Date && DateTime.TryParse(value, bindingCulture, DateTimeStyles.AllowWhiteSpaces, out dateParamValue))
                return new AnalyzedStepParameter("DateTime", paramName, regexPattern);

            return new AnalyzedStepParameter("String", paramName, regexPattern);
        }

        private static readonly Regex quotesRe = new Regex(@"""+(?<param>.*?)""+|'+(?<param>.*?)'+|(?<param>\<.*?\>)");
        private IEnumerable<CaptureWithContext> RecognizeQuotedTexts(string stepText)
        {
            return quotesRe.Matches(stepText)
                           .Cast<Match>()
                           .Select(m => (Capture)m.Groups["param"])
                           .ToCaptureWithContext(ParameterType.Text);
        }

        private static readonly Regex intRe = new Regex(@"-?\d+");

        private IEnumerable<CaptureWithContext> RecognizeIntegers(string stepText)
        {
            return intRe.Matches(stepText).ToCaptureWithContext(ParameterType.Int);
        }

        private IEnumerable<CaptureWithContext> RecognizeDecimals(string stepText, CultureInfo bindingCulture)
        {
            Regex decimalRe = new Regex(string.Format(@"-?\d+{0}\d+", bindingCulture.NumberFormat.NumberDecimalSeparator));
            return decimalRe.Matches(stepText).ToCaptureWithContext(ParameterType.Decimal);
        }

        private static readonly Regex dateRe = new Regex(string.Join("|", GetDateFormats()));

        /// <summary>
        /// note: space separator not supported to prevent clashes
        /// </summary>
        private static IEnumerable<string> GetDateFormats()
        {
            yield return GetDateFormat("/");
            yield return GetDateFormat("-");
            yield return GetDateFormat(".");
        }

        private static string GetDateFormat(string separator)
        {
            var separatorEscaped = Regex.Escape(separator);
            return @"\d{1,4}" + separatorEscaped + @"\d{1,4}" + separatorEscaped + @"\d{1,4}";
        }

        private IEnumerable<CaptureWithContext> RecognizeDates(string stepText)
        {
            return dateRe.Matches(stepText).ToCaptureWithContext(ParameterType.Date);
        }
    }

    internal static class MatchCollectionExtensions
    {
        public static IEnumerable<CaptureWithContext> ToCaptureWithContext(this MatchCollection collection, ParameterType parameterType)
        {
            return collection.Cast<Capture>().ToCaptureWithContext(parameterType);
        }
        public static IEnumerable<CaptureWithContext> ToCaptureWithContext(this IEnumerable<Capture> collection, ParameterType parameterType)
        {
            return collection.Select(c => new CaptureWithContext(c, parameterType));
        }
    }

    internal class CaptureWithContext
    {
        public Capture Capture { get; }

        public ParameterType ParameterType { get; }

        public CaptureWithContext(Capture capture, ParameterType parameterType)
        {
            Capture = capture;
            ParameterType = parameterType;
        }
    }

    internal enum ParameterType
    {
        Text,
        Int,
        Decimal,
        Date
    }
}