using System;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.Tracing
{
    internal abstract class StepDefinitionSkeletonProviderBase : IStepDefinitionSkeletonProvider
    {
        public const string CODEINDENT = "    ";
        public abstract string GetStepDefinitionSkeleton(StepArgs stepArgs);
        public abstract string GetBindingClassSkeleton(string stepDefinitions);

        protected static string GetAttributeName(Type attributeType)
        {
            return attributeType.Name.Substring(0, attributeType.Name.Length - "Attribute".Length);
        }

        protected static string EscapeRegex(string text)
        {
            return Regex.Escape(text).Replace("\"", "\"\"").Replace("\\ ", " ");
        }
    }
}