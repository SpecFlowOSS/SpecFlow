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
        // The {string} type provided by the CucumberExpressionParameterTypeRegistry has a fake regex ('.*'),
        // because to be able to support both double-quote (") and apostrophe (') wrapping, there has to be
        // two capture groups that is not supported inside a parameter group in SpecFlow. 
        // The solution is to override the {string} handling here and provide the two capture groups without 
        // wrapping it to a general parameter group.
        regexps = ParameterTypeConstants.StringParameterRegexps;
        shouldWrapWithCaptureGroup = false;
        return true;
    }
}