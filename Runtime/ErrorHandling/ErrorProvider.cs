using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.ErrorHandling
{
    internal class ErrorProvider
    {
        private readonly StepFormatter stepFormatter;
        private readonly IUnitTestRuntimeProvider unitTestRuntimeProvider;

        public ErrorProvider()
        {
            stepFormatter = ObjectContainer.StepFormatter;
            unitTestRuntimeProvider = ObjectContainer.UnitTestRuntimeProvider;
        }

        private string GetMethodText(MethodInfo methodInfo)
        {
            return string.Format("{0}.{1}({2})", methodInfo.DeclaringType.Name, methodInfo.Name,
                string.Join(", ", methodInfo.GetParameters().Select(pi => pi.ParameterType.Name).ToArray()));
        }

        public Exception GetCallError(MethodInfo methodInfo, Exception ex)
        {
            return new BindingException(
                string.Format("Error calling binding method '{0}': {1}",
                    GetMethodText(methodInfo), ex.Message));
        }

        public Exception GetParameterCountError(BindingMatch match, int expectedParameterCount)
        {
            return new BindingException(
                string.Format("Parameter count mismatch! The binding method '{0}' should have {1} parameters",
                    GetMethodText(match.StepBinding.MethodInfo), expectedParameterCount));
        }

        public Exception GetAmbiguousMatchError(IEnumerable<BindingMatch> matches, StepArgs stepArgs)
        {
            string stepDescription = stepFormatter.GetStepDescription(stepArgs);
            return new BindingException(
                string.Format("Ambiguous step definitions found for step '{0}': {1}",
                    stepDescription,
                    string.Join(", ", matches.Select(m => GetMethodText(m.StepBinding.MethodInfo)).ToArray())));
        }

        public MissingStepDefinitionException GetMissingStepDefinitionError()
        {
            return new MissingStepDefinitionException();
        }

        public PendingStepException GetPendingStepDefinitionError()
        {
            return new PendingStepException();
        }

        public void ThrowPendingError(TestStatus testStatus, string message)
        {
            switch (RuntimeConfiguration.Current.MissingOrPendingStepsOutcome)
            {
                case MissingOrPendingStepsOutcome.Inconclusive:
                    unitTestRuntimeProvider.TestInconclusive(message);
                    break;
                case MissingOrPendingStepsOutcome.Ignore:
                    unitTestRuntimeProvider.TestIgnore(message);
                    break;
                default:
                    if (testStatus == TestStatus.MissingStepDefinition)
                        throw GetMissingStepDefinitionError();
                    throw GetPendingStepDefinitionError();
            }

        }

        public Exception GetTooManyBindingParamError(int maxParam)
        {
            return new BindingException(
                string.Format("Binding methods with more than {0} parameters are not supported", maxParam));
        }
    }
}