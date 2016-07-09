using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.BindingSkeletons
{
    public class StepDefinitionSkeletonProvider : IStepDefinitionSkeletonProvider
    {
        public const string METHOD_INDENT = "        ";

        private readonly ISkeletonTemplateProvider templateProvider;
        private readonly IStepTextAnalyzer stepTextAnalyzer;

        public StepDefinitionSkeletonProvider(ISkeletonTemplateProvider templateProvider, IStepTextAnalyzer stepTextAnalyzer)
        {
            this.templateProvider = templateProvider;
            this.stepTextAnalyzer = stepTextAnalyzer;
        }

        public string GetBindingClassSkeleton(ProgrammingLanguage language, StepInstance[] stepInstances, string namespaceName, string className, StepDefinitionSkeletonStyle style, CultureInfo bindingCulture)
        {
            var template = templateProvider.GetStepDefinitionClassTemplate(language);

            var bindings = string.Join(Environment.NewLine, GetOrderedSteps(stepInstances)
                .Select(si => GetStepDefinitionSkeleton(language, si, style, bindingCulture)).Distinct().ToArray()).TrimEnd();
            if (bindings.Length > 0)
                bindings = bindings.Indent(METHOD_INDENT);

            //{namespace}/{className}/{bindings}
            return ApplyTemplate(template, new { @namespace = namespaceName, className, bindings});
        }

        private static IEnumerable<StepInstance> GetOrderedSteps(StepInstance[] stepInstances)
        {
            return stepInstances
                .Select((si, index) => new { Step = si, Index = index})
                .OrderBy(item => item.Step.StepDefinitionType)
                .ThenBy(item => item.Index)
                .Select(item => item.Step);
        }

        public virtual string GetStepDefinitionSkeleton(ProgrammingLanguage language, StepInstance stepInstance, StepDefinitionSkeletonStyle style, CultureInfo bindingCulture)
        {
            var withRegex = style == StepDefinitionSkeletonStyle.RegexAttribute;
            var template = templateProvider.GetStepDefinitionTemplate(language, withRegex);
            var analyzedStepText = Analyze(stepInstance, bindingCulture);
            //{attribute}/{regex}/{methodName}/{parameters}
            return ApplyTemplate(template, new
                                               {
                                                   attribute = stepInstance.StepDefinitionType, 
                                                   regex = withRegex ? GetRegex(analyzedStepText) : "", 
                                                   methodName = GetMethodName(stepInstance, analyzedStepText, style, language), 
                                                   parameters = string.Join(", ", analyzedStepText.Parameters.Select(p => ToDeclaration(language, p)).ToArray())
                                               });
        }

        private AnalyzedStepText Analyze(StepInstance stepInstance, CultureInfo bindingCulture)
        {
            var result = stepTextAnalyzer.Analyze(stepInstance.Text, bindingCulture);
            if (stepInstance.MultilineTextArgument != null)
                result.Parameters.Add(new AnalyzedStepParameter("String", "multilineText"));
            if (stepInstance.TableArgument != null)
                result.Parameters.Add(new AnalyzedStepParameter("Table", "table"));
            return result;
        }

        static private readonly Regex wordRe = new Regex(@"[\w]+");
        private IEnumerable<string> GetWords(string text)
        {
            return wordRe.Matches(text).Cast<Match>().Select(m => m.Value);
        }

        private string GetMethodName(StepInstance stepInstance, AnalyzedStepText analyzedStepText, StepDefinitionSkeletonStyle style, ProgrammingLanguage language)
        {
            var keyword = LanguageHelper.GetDefaultKeyword(stepInstance.StepContext.Language, stepInstance.StepDefinitionType);
            switch (style)
            {
                case StepDefinitionSkeletonStyle.RegexAttribute:
                    return keyword.ToIdentifier() + string.Concat(analyzedStepText.TextParts.ToArray()).ToIdentifier();
                case StepDefinitionSkeletonStyle.MethodNameUnderscores:
                    return GetMatchingMethodName(keyword, analyzedStepText, stepInstance.StepContext.Language, AppendWordsUnderscored, "_{0}");
                case StepDefinitionSkeletonStyle.MethodNamePascalCase:
                    return GetMatchingMethodName(keyword, analyzedStepText, stepInstance.StepContext.Language, AppendWordsPascalCase, "_{0}_");
                case StepDefinitionSkeletonStyle.MethodNameRegex:
                    if (language != ProgrammingLanguage.FSharp)
                        goto case StepDefinitionSkeletonStyle.MethodNameUnderscores;
                    return "``" + GetRegex(analyzedStepText) + "``";
                default:
                    throw new NotSupportedException();
            }
        }

        private string GetMatchingMethodName(string keyword, AnalyzedStepText analyzedStepText, CultureInfo language, Action<string, CultureInfo, StringBuilder> appendWords, string paramFormat)
        {
            StringBuilder result = new StringBuilder(keyword);

            appendWords(analyzedStepText.TextParts[0], language, result);
            for (int i = 1; i < analyzedStepText.TextParts.Count; i++)
            {
                result.AppendFormat(paramFormat, analyzedStepText.Parameters[i - 1].Name.ToUpper(CultureInfo.InvariantCulture));
                appendWords(analyzedStepText.TextParts[i], language, result);
            }

            if (result.Length > 0 && result[result.Length - 1] == '_')
                result.Remove(result.Length - 1, 1);

            return result.ToString();
        }

        private void AppendWordsUnderscored(string text, CultureInfo language, StringBuilder result)
        {
            foreach (var word in GetWords(text))
            {
                result.Append("_");
                result.Append(word);
            }
        }

        private void AppendWordsPascalCase(string text, CultureInfo language, StringBuilder result)
        {
            foreach (var word in GetWords(text))
            {
                result.Append(word.Substring(0, 1).ToUpper(language));
                result.Append(word.Substring(1));
            }
        }

        private string GetRegex(AnalyzedStepText stepText)
        {
            StringBuilder result = new StringBuilder();

            result.Append(EscapeRegex(stepText.TextParts[0]));
            for (int i = 1; i < stepText.TextParts.Count; i++)
            {
                result.AppendFormat("({0})", stepText.Parameters[i-1].RegexPattern);
                result.Append(EscapeRegex(stepText.TextParts[i]));
            }

            return result.ToString();
        }

        protected static string EscapeRegex(string text)
        {
            return Regex.Escape(text).Replace("\"", "\"\"").Replace("\\ ", " ");
        }

        private string ApplyTemplate(string template, object args)
        {
            return args.GetType().GetProperties().Aggregate(template, (current, propertyInfo) => current.Replace("{" + propertyInfo.Name + "}", propertyInfo.GetValue(args, null).ToString()));
        }

        private string ToDeclaration(ProgrammingLanguage language, AnalyzedStepParameter parameter)
        {
            switch (language)
            {
                case ProgrammingLanguage.VB:
                    return String.Format("ByVal {0} As {1}", parameter.Name, parameter.Type);
                case ProgrammingLanguage.CSharp:
                    return String.Format("{1} {0}", parameter.Name, GetCSharpTypeName(parameter.Type));
                case ProgrammingLanguage.FSharp:
                    return String.Format("{0} : {1}", parameter.Name, GetFSharpTypeName(parameter.Type));
                default:
                    return String.Format("{1} {0}", parameter.Name, parameter.Type);
            }
        }

        private string GetCSharpTypeName(string type)
        {
            switch (type)
            {
                case "String":
                    return "string";
                case "Int32":
                    return "int";
                default:
                    return type;
            }
        }

        private string GetFSharpTypeName(string type)
        {
            switch (type)
            {
                case "String":
                    return "string";
                default:
                    return type;
            }
        }
    }
}