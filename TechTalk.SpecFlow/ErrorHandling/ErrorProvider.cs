using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.ErrorHandling
{
    public interface IErrorProvider
    {
        string GetMethodText(IBindingMethod method);
        Exception GetCallError(IBindingMethod method, Exception ex);
        Exception GetParameterCountError(BindingMatch match, int expectedParameterCount);
        Exception GetAmbiguousMatchError(List<BindingMatch> matches, StepInstance stepInstance);
        Exception GetAmbiguousBecauseParamCheckMatchError(List<BindingMatch> matches, StepInstance stepInstance);
        Exception GetNoMatchBecauseOfScopeFilterError(List<BindingMatch> matches, StepInstance stepInstance);
        MissingStepDefinitionException GetMissingStepDefinitionError();
        PendingStepException GetPendingStepDefinitionError();
        void ThrowPendingError(TestStatus testStatus, string message);
        Exception GetTooManyBindingParamError(int maxParam);
        Exception GetNonStaticEventError(IBindingMethod method);
    }

    internal class ErrorProvider : IErrorProvider
    {
        private readonly IStepFormatter stepFormatter;
        private readonly IUnitTestRuntimeProvider unitTestRuntimeProvider;
        private readonly Configuration.SpecFlowConfiguration specFlowConfiguration;

        public ErrorProvider(IStepFormatter stepFormatter, Configuration.SpecFlowConfiguration specFlowConfiguration, IUnitTestRuntimeProvider unitTestRuntimeProvider)
        {
            this.stepFormatter = stepFormatter;
            this.unitTestRuntimeProvider = unitTestRuntimeProvider;
            this.specFlowConfiguration = specFlowConfiguration;
        }

        public string GetMethodText(IBindingMethod method)
        {
            return string.Format("{0}.{1}({2})", method.Type.Name, method.Name,
                string.Join(", ", method.Parameters.Select(p => p.Type.Name).ToArray()));
        }

        public Exception GetCallError(IBindingMethod method, Exception ex)
        {
            return new BindingException(
                string.Format("Error calling binding method '{0}': {1}",
                    GetMethodText(method), ex.Message));
        }

        public Exception GetParameterCountError(BindingMatch match, int expectedParameterCount)
        {
            return new BindingException(
                string.Format("Parameter count mismatch! The binding method '{0}' should have {1} parameters",
                    GetMethodText(match.StepBinding.Method), expectedParameterCount));
        }

        public Exception GetAmbiguousMatchError(List<BindingMatch> matches, StepInstance stepInstance)
        {
            string stepDescription = stepFormatter.GetStepDescription(stepInstance);
            return new BindingException(
                string.Format("Ambiguous step definitions found for step '{0}': {1}",
                    stepDescription,
                    string.Join(", ", matches.Select(m => GetMethodText(m.StepBinding.Method)).ToArray())));
        }


        public Exception GetAmbiguousBecauseParamCheckMatchError(List<BindingMatch> matches, StepInstance stepInstance)
        {
            string stepDescription = stepFormatter.GetStepDescription(stepInstance);
            return new BindingException(
                string.Format("Multiple step definitions found, but none of them have matching parameter count and type for step '{0}': {1}",
                    stepDescription,
                    string.Join(", ", matches.Select(m => GetMethodText(m.StepBinding.Method)).ToArray())));
        }

        public Exception GetNoMatchBecauseOfScopeFilterError(List<BindingMatch> matches, StepInstance stepInstance)
        {
            string stepDescription = stepFormatter.GetStepDescription(stepInstance);
            return new BindingException(
                string.Format("Multiple step definitions found, but none of them have matching scope for step '{0}': {1}",
                    stepDescription,
                    string.Join(", ", matches.Select(m => GetMethodText(m.StepBinding.Method)).ToArray())));
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
            switch (specFlowConfiguration.MissingOrPendingStepsOutcome)
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

        public Exception GetNonStaticEventError(IBindingMethod method)
        {
            throw new BindingException(
                string.Format("The binding methods for before/after feature and before/after test run events must be static! {0}",
                GetMethodText(method)));
        }
    }
}