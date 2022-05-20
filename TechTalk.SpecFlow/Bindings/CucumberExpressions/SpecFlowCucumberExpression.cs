using System;
using CucumberExpressions;

namespace TechTalk.SpecFlow.Bindings.CucumberExpressions;

public class SpecFlowCucumberExpression : CucumberExpression
{
    public SpecFlowCucumberExpression(string expression, IParameterTypeRegistry parameterTypeRegistry) : base(expression, parameterTypeRegistry)
    {
    }

    protected override bool HandleStringType(string name, IParameterType parameterType, out string[] regexps, out bool shouldWrapWithCaptureGroup)
    {
        regexps = ParameterTypeConstants.StringParameterRegexps;
        //regexps = new[] { $"(?:{ParameterTypeConstants.StringParameterRegexDoubleQuote}|{ParameterTypeConstants.StringParameterRegexApostrophe})" };
        shouldWrapWithCaptureGroup = false;
        return true;
    }
}