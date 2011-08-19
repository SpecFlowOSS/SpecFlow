using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.ErrorHandling
{
    public interface IErrorProvider
    {
        string GetMethodText(MethodInfo methodInfo);
        Exception GetCallError(MethodInfo methodInfo, Exception ex);
        Exception GetParameterCountError(BindingMatch match, int expectedParameterCount);
        Exception GetAmbiguousMatchError(IEnumerable<BindingMatch> matches, StepArgs stepArgs);
        Exception GetAmbiguousBecauseParamCheckMatchError(List<BindingMatch> matches, StepArgs stepArgs);
        Exception GetNoMatchBecauseOfScopeFilterError(List<BindingMatch> matches, StepArgs stepArgs);
        MissingStepDefinitionException GetMissingStepDefinitionError();
        PendingStepException GetPendingStepDefinitionError();
        void ThrowPendingError(TestStatus testStatus, string message);
        Exception GetTooManyBindingParamError(int maxParam);
        Exception GetNonStaticEventError(MethodInfo methodInfo);
    }

    internal class ErrorProvider : IErrorProvider
    {
        private readonly IStepFormatter stepFormatter;
        private readonly IUnitTestRuntimeProvider unitTestRuntimeProvider;
        private readonly RuntimeConfiguration runtimeConfiguration;

        public ErrorProvider(IStepFormatter stepFormatter, RuntimeConfiguration runtimeConfiguration, IUnitTestRuntimeProvider unitTestRuntimeProvider)
        {
            this.stepFormatter = stepFormatter;
            this.unitTestRuntimeProvider = unitTestRuntimeProvider;
            this.runtimeConfiguration = runtimeConfiguration;
        }

        public string GetMethodText(MethodInfo methodInfo)
        {
            return string.Format("{0}.{1}({2})", methodInfo.ReflectedType.Name, methodInfo.Name,
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


        public Exception GetAmbiguousBecauseParamCheckMatchError(List<BindingMatch> matches, StepArgs stepArgs)
        {
            string stepDescription = stepFormatter.GetStepDescription(stepArgs);
            return new BindingException(
                string.Format("Multiple step definitions found, but none of them have matching parameter count and type for step '{0}': {1}",
                    stepDescription,
                    string.Join(", ", matches.Select(m => GetMethodText(m.StepBinding.MethodInfo)).ToArray())));
        }

        public Exception GetNoMatchBecauseOfScopeFilterError(List<BindingMatch> matches, StepArgs stepArgs)
        {
            string stepDescription = stepFormatter.GetStepDescription(stepArgs);
            return new BindingException(
                string.Format("Multiple step definitions found, but none of them have matching scope for step '{0}': {1}",
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
            switch (runtimeConfiguration.MissingOrPendingStepsOutcome)
            {
                case MissingOrPendingStepsOutcome.Pending:
                    unitTestRuntimeProvider.TestPending(message);
                    break;
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

        public Exception GetNonStaticEventError(MethodInfo methodInfo)
        {
            throw new BindingException(
                string.Format("The binding methods for before/after feature and before/after test run events must be static! {0}",
                GetMethodText(methodInfo)));
        }
    }
}