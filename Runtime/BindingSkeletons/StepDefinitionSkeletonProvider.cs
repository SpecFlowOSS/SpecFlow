using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.BindingSkeletons
{
    public class StepDefinitionSkeletonProvider : IStepDefinitionSkeletonProvider2
    {
        private readonly ISkeletonTemplateProvider templateProvider;

        public StepDefinitionSkeletonProvider(ISkeletonTemplateProvider templateProvider)
        {
            this.templateProvider = templateProvider;
        }

        public string GetBindingClassSkeleton(ProgrammingLanguage language, StepInstance[] stepInstances, string namespaceName, string className, StepDefinitionSkeletonStyle style)
        {
            var template = templateProvider.GetStepDefinitionClassTemplate(language);

            var bindings = string.Join(Environment.NewLine, stepInstances.Select(si => GetStepDefinitionSkeleton(language, si, style)).ToArray());
            if (bindings.Length > 0)
                bindings = bindings.Indent("        ");

            //{namespace}/{className}/{bindings}
            return ApplyTemplate(template, new { @namespace = namespaceName, className, bindings});
        }

        public virtual string GetStepDefinitionSkeleton(ProgrammingLanguage language, StepInstance stepInstance, StepDefinitionSkeletonStyle style)
        {
            var withRegex = style == StepDefinitionSkeletonStyle.RegularExpressions;
            var template = templateProvider.GetStepDefinitionTemplate(language, withRegex);
            //{attribute}/{regex}/{methodName}/{parameters}
            return ApplyTemplate(template, new
                                               {
                                                   attribute = stepInstance.StepDefinitionType, 
                                                   regex = EscapeRegex(GetRegex(stepInstance.Text)), 
                                                   methodName = GetMethodName(stepInstance), 
                                                   parameters = string.Join(", ", GetParameters(stepInstance).Select(p => p.ToDeclaration(language)).ToArray())
                                               });
        }

        private class ParameterDescriptor
        {
            private readonly string type;
            private readonly string name;

            public ParameterDescriptor(string type, string name)
            {
                this.type = type;
                this.name = name;
            }

            public string ToDeclaration(ProgrammingLanguage language)
            {
                switch (language)
                {
                    case ProgrammingLanguage.VB:
                        return string.Format("ByVal {0} As {1}", name, type);
                    case ProgrammingLanguage.CSharp:
                        return string.Format("{1} {0}", name, GetCSharpTypeName());
                    case ProgrammingLanguage.FSharp:
                        return string.Format("{0} : {1}", name, GetFSharpTypeName());
                    default:
                        return string.Format("{1} {0}", name, type);
                }
            }

            private string GetCSharpTypeName()
            {
                switch (type)
                {
                    case "String":
                        return "string";
                    default:
                        return type;
                }
            }

            private string GetFSharpTypeName()
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

        private IEnumerable<ParameterDescriptor> GetParameters(StepInstance stepInstance)
        {
            var extraArgs = new List<ParameterDescriptor>();
            if (stepInstance.MultilineTextArgument != null)
                extraArgs.Add(new ParameterDescriptor("String", "multilineText"));
            if (stepInstance.TableArgument != null)
                extraArgs.Add(new ParameterDescriptor("Table", "table"));
            return extraArgs.ToArray();
        }

        private string GetMethodName(StepInstance stepInstance)
        {
            var keyword = LanguageHelper.GetDefaultKeyword(stepInstance.StepContext.Language, stepInstance.StepDefinitionType).ToIdentifier();
            return keyword + stepInstance.Text.ToIdentifier();
        }

        private string GetRegex(string stepText)
        {
            return stepText;
        }

        protected static string EscapeRegex(string text)
        {
            return Regex.Escape(text).Replace("\"", "\"\"").Replace("\\ ", " ");
        }

        private string ApplyTemplate(string template, object args)
        {
            return args.GetType().GetProperties().Aggregate(template, (current, propertyInfo) => current.Replace("{" + propertyInfo.Name + "}", propertyInfo.GetValue(args, null).ToString()));
        }
    }
}