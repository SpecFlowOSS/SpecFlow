using System.ComponentModel;

namespace TechTalk.SpecFlow.BindingSkeletons
{
    public enum StepDefinitionSkeletonStyle
    {
        [Description("Regular expressions in attributes")]
        RegexAttribute = 0,
        [Description("Method name - underscores")]
        MethodNameUnderscores = 1,
        [Description("Method name - pascal case")]
        MethodNamePascalCase = 2,
        [Description("Method name as regulare expression (F#)")]
        MethodNameRegex = 3
    }
}