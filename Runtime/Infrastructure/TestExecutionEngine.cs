using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Compatibility;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.ErrorHandling;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.Infrastructure
{
    public class TestExecutionEngine : ITestExecutionEngine
    {
        private readonly RuntimeConfiguration runtimeConfiguration;
        private readonly IErrorProvider errorProvider;
        private readonly ITestTracer testTracer;
        private readonly IUnitTestRuntimeProvider unitTestRuntimeProvider;
        private readonly IStepFormatter stepFormatter;
        private readonly IBindingRegistry bindingRegistry;
        private readonly IStepArgumentTypeConverter stepArgumentTypeConverter;
        private readonly IDictionary<ProgrammingLanguage, IStepDefinitionSkeletonProvider> stepDefinitionSkeletonProviders;
        private readonly IContextManager contextManager;
        private readonly IStepDefinitionMatcher stepDefinitionMatcher;
        private readonly IStepErrorHandler[] stepErrorHandlers;

        private IStepDefinitionSkeletonProvider currentStepDefinitionSkeletonProvider;

        public TestExecutionEngine(IStepFormatter stepFormatter, ITestTracer testTracer, IErrorProvider errorProvider, IStepArgumentTypeConverter stepArgumentTypeConverter, 
            RuntimeConfiguration runtimeConfiguration, IBindingRegistry bindingRegistry, IUnitTestRuntimeProvider unitTestRuntimeProvider, 
            IDictionary<ProgrammingLanguage, IStepDefinitionSkeletonProvider> stepDefinitionSkeletonProviders, IContextManager contextManager, IStepDefinitionMatcher stepDefinitionMatcher,
            IDictionary<string, IStepErrorHandler> stepErrorHandlers)
        {
            this.errorProvider = errorProvider;
            this.stepDefinitionMatcher = stepDefinitionMatcher;
            this.contextManager = contextManager;
            this.stepDefinitionSkeletonProviders = stepDefinitionSkeletonProviders;
            this.unitTestRuntimeProvider = unitTestRuntimeProvider;
            this.bindingRegistry = bindingRegistry;
            this.runtimeConfiguration = runtimeConfiguration;
            this.testTracer = testTracer;
            this.stepFormatter = stepFormatter;
            this.stepArgumentTypeConverter = stepArgumentTypeConverter;
            this.stepErrorHandlers = stepErrorHandlers == null ? null : stepErrorHandlers.Values.ToArray();

            this.currentStepDefinitionSkeletonProvider = stepDefinitionSkeletonProviders[ProgrammingLanguage.CSharp]; // fallback if feature initialization was not proper
        }

        public FeatureContext FeatureContext
        {
            get { return contextManager.FeatureContext; }
        }

        public ScenarioContext ScenarioContext
        {
            get { return contextManager.ScenarioContext; }
        }

        public virtual void Initialize(Assembly[] bindingAssemblies)
        {
            foreach (Assembly assembly in bindingAssemblies)
            {
                bindingRegistry.BuildBindingsFromAssembly(assembly);
            }

            OnTestRunnerStart();
#if !SILVERLIGHT
            AppDomain.CurrentDomain.DomainUnload += 
                delegate
                    {
                        OnTestRunEnd();
                    };
            //TODO: Siverlight
#endif
        }

        protected virtual void OnTestRunnerStart()
        {
            FireEvents(BindingEvent.TestRunStart);
        }

        private bool testRunnerEndExecuted = false;

        public virtual void OnTestRunEnd()
        {
            if (testRunnerEndExecuted)
                return;

            testRunnerEndExecuted = true;
            FireEvents(BindingEvent.TestRunEnd);
        }

        public void OnFeatureStart(FeatureInfo featureInfo)
        {
            // if the unit test provider would execute the fixture teardown code 
            // only delayed (at the end of the execution), we automatically close 
            // the current feature if necessary
            if (unitTestRuntimeProvider.DelayedFixtureTearDown &&
                contextManager.FeatureContext != null)
            {
                OnFeatureEnd();
            }

            if (!stepDefinitionSkeletonProviders.ContainsKey(featureInfo.GenerationTargetLanguage))
                currentStepDefinitionSkeletonProvider = stepDefinitionSkeletonProviders[ProgrammingLanguage.CSharp]; // fallback case for unsupported skeleton provider
            currentStepDefinitionSkeletonProvider = stepDefinitionSkeletonProviders[featureInfo.GenerationTargetLanguage];

            // The Generator defines the value of FeatureInfo.Language: either feature-language or language from App.config or the default
            // The runtime can define the binding-culture: Value is configured on App.config, else it is null
            CultureInfo bindingCulture = runtimeConfiguration.BindingCulture ?? featureInfo.Language;
            contextManager.InitializeFeatureContext(featureInfo, bindingCulture);
            FireEvents(BindingEvent.FeatureStart);
        }

        public void OnFeatureEnd()
        {
            // if the unit test provider would execute the fixture teardown code 
            // only delayed (at the end of the execution), we ignore the 
            // feature-end call, if the feature has been closed already
            if (unitTestRuntimeProvider.DelayedFixtureTearDown &&
                contextManager.FeatureContext == null)
                return;
                
            FireEvents(BindingEvent.FeatureEnd);

            if (runtimeConfiguration.TraceTimings)
            {
                contextManager.FeatureContext.Stopwatch.Stop();
                var duration = contextManager.FeatureContext.Stopwatch.Elapsed;
                testTracer.TraceDuration(duration, "Feature: " + contextManager.FeatureContext.FeatureInfo.Title);
            }

            contextManager.CleanupFeatureContext();
        }

        public void OnScenarioStart(ScenarioInfo scenarioInfo)
        {
            contextManager.InitializeScenarioContext(scenarioInfo);
            FireScenarioEvents(BindingEvent.ScenarioStart);
        }

        public void OnAfterLastStep()
        {
            HandleBlockSwitch(ScenarioBlock.None);

            if (runtimeConfiguration.TraceTimings)
            {
                contextManager.ScenarioContext.Stopwatch.Stop();
                var duration = contextManager.ScenarioContext.Stopwatch.Elapsed;
                testTracer.TraceDuration(duration, "Scenario: " + contextManager.ScenarioContext.ScenarioInfo.Title);
            }

            if (contextManager.ScenarioContext.TestStatus == TestStatus.OK)
                return;

            if (contextManager.ScenarioContext.TestStatus == TestStatus.StepDefinitionPending)
            {
                var pendingSteps = contextManager.ScenarioContext.PendingSteps.Distinct().OrderBy(s => s);
                errorProvider.ThrowPendingError(contextManager.ScenarioContext.TestStatus, string.Format("{0}{2}  {1}",
                    errorProvider.GetPendingStepDefinitionError().Message,
                    string.Join(Environment.NewLine + "  ", pendingSteps.ToArray()),
                    Environment.NewLine));
                return;
            }

            if (contextManager.ScenarioContext.TestStatus == TestStatus.MissingStepDefinition)
            {
                var missingSteps = contextManager.ScenarioContext.MissingSteps.Distinct().OrderBy(s => s);
                string bindingSkeleton =
                    currentStepDefinitionSkeletonProvider.GetBindingClassSkeleton(
                        string.Join(Environment.NewLine, missingSteps.ToArray()));
                errorProvider.ThrowPendingError(contextManager.ScenarioContext.TestStatus, string.Format("{0}{2}{1}",
                    errorProvider.GetMissingStepDefinitionError().Message,
                    bindingSkeleton,
                    Environment.NewLine));
                return;
            }

            if (contextManager.ScenarioContext.TestError == null)
                throw new InvalidOperationException("test failed with an unknown error");

            contextManager.ScenarioContext.TestError.PreserveStackTrace();
            throw contextManager.ScenarioContext.TestError;
        }

        public void OnScenarioEnd()
        {
            FireScenarioEvents(BindingEvent.ScenarioEnd);
            contextManager.CleanupScenarioContext();
        }

        protected virtual void OnBlockStart(ScenarioBlock block)
        {
            if (block == ScenarioBlock.None)
                return;

            FireScenarioEvents(BindingEvent.BlockStart); 
        }

        protected virtual void OnBlockEnd(ScenarioBlock block)
        {
            if (block == ScenarioBlock.None)
                return;

            FireScenarioEvents(BindingEvent.BlockEnd);
        }

        protected virtual void OnStepStart()
        {
            FireScenarioEvents(BindingEvent.StepStart); 
        }

        protected virtual void OnStepEnd()
        {
            FireScenarioEvents(BindingEvent.StepEnd);
        }

        #region Step/event execution
        private void FireScenarioEvents(BindingEvent bindingEvent)
        {
            FireEvents(bindingEvent);
        }

        private void FireEvents(BindingEvent bindingEvent)
        {
            var stepContext = contextManager.GetStepContext();

            foreach (IHookBinding eventBinding in bindingRegistry.GetEvents(bindingEvent))
            {
                int scopeMatches;
                if (eventBinding.IsScoped && !eventBinding.BindingScope.Match(stepContext, out scopeMatches))
                    continue;

                eventBinding.Invoke(contextManager, testTracer);
            }
        }

        private void ExecuteStep(StepArgs stepArgs)
        {
            HandleBlockSwitch(stepArgs.Type.ToScenarioBlock());

            testTracer.TraceStep(stepArgs, true);

            bool isStepSkipped = contextManager.ScenarioContext.TestStatus != TestStatus.OK;

            BindingMatch match = null;
            object[] arguments = null;
            try
            {
                match = GetStepMatch(stepArgs);
                arguments = GetExecuteArguments(match);

                if (isStepSkipped)
                {
                    testTracer.TraceStepSkipped();
                }
                else
                {
                    OnStepStart();
                    TimeSpan duration = ExecuteStepMatch(match, arguments);
                    if (runtimeConfiguration.TraceSuccessfulSteps)
                        testTracer.TraceStepDone(match, arguments, duration);
                }
            }
            catch(PendingStepException)
            {
                Debug.Assert(match != null);
                Debug.Assert(arguments != null);

                testTracer.TraceStepPending(match, arguments);
                contextManager.ScenarioContext.PendingSteps.Add(
                    stepFormatter.GetMatchText(match, arguments));

                if (contextManager.ScenarioContext.TestStatus < TestStatus.StepDefinitionPending)
                    contextManager.ScenarioContext.TestStatus = TestStatus.StepDefinitionPending;
            }
            catch(MissingStepDefinitionException)
            {
                if (contextManager.ScenarioContext.TestStatus < TestStatus.MissingStepDefinition)
                    contextManager.ScenarioContext.TestStatus = TestStatus.MissingStepDefinition;
            }
            catch(BindingException ex)
            {
                testTracer.TraceBindingError(ex);
                if (contextManager.ScenarioContext.TestStatus < TestStatus.BindingError)
                {
                    contextManager.ScenarioContext.TestStatus = TestStatus.BindingError;
                    contextManager.ScenarioContext.TestError = ex;
                }
            }
            catch(Exception ex)
            {
                testTracer.TraceError(ex);

                if (contextManager.ScenarioContext.TestStatus < TestStatus.TestError)
                {
                    contextManager.ScenarioContext.TestStatus = TestStatus.TestError;
                    contextManager.ScenarioContext.TestError = ex;
                }
                if (runtimeConfiguration.StopAtFirstError)
                    throw;
            }
            finally
            {
                if (!isStepSkipped)
                    OnStepEnd();
            }
        }

        private BindingMatch GetStepMatch(StepArgs stepArgs)
        {
            List<BindingMatch> matches = stepDefinitionMatcher.GetMatches(stepArgs);

            if (matches.Count == 0)
            {
                // there were either no regex match or it was filtered out by the param/scope matching
                // to provide better error message for the param matching error, we re-run 
                // the matching without param check

                List<BindingMatch> matchesWithoutScopeCheck = stepDefinitionMatcher.GetMatchesWithoutScopeCheck(stepArgs);

                if (matchesWithoutScopeCheck.Count == 0)
                {
                    List<BindingMatch> matchesWithoutParamCheck = stepDefinitionMatcher.GetMatchesWithoutParamCheck(stepArgs);
                    if (matchesWithoutParamCheck.Count == 1)
                    {
                        // no ambiguouity, but param error -> execute will find it out
                        return matchesWithoutParamCheck[0];
                    }
                    if (matchesWithoutParamCheck.Count > 1)
                    {
                        // ambiguouity, because of param error
                        throw errorProvider.GetAmbiguousBecauseParamCheckMatchError(matchesWithoutParamCheck, stepArgs);
                    }
                }

                testTracer.TraceNoMatchingStepDefinition(stepArgs, contextManager.FeatureContext.FeatureInfo.GenerationTargetLanguage, matchesWithoutScopeCheck);
                contextManager.ScenarioContext.MissingSteps.Add(
                    currentStepDefinitionSkeletonProvider.GetStepDefinitionSkeleton(stepArgs));
                throw errorProvider.GetMissingStepDefinitionError();
            }
            if (matches.Count > 1)
            {
                if (runtimeConfiguration.DetectAmbiguousMatches)
                    throw errorProvider.GetAmbiguousMatchError(matches, stepArgs);
            }
            return matches[0];
        }

        private TimeSpan ExecuteStepMatch(BindingMatch match, object[] arguments)
        {
            TimeSpan duration = TimeSpan.Zero;
            try
            {
                match.StepBinding.Invoke(contextManager, testTracer, arguments, out duration);
            }
            catch(Exception ex)
            {
                if (stepErrorHandlers != null)
                {
                    StepFailureEventArgs stepFailureEventArgs = new StepFailureEventArgs(match.StepBinding, match.StepArgs.StepContext, ex);
                    foreach (var stepErrorHandler in stepErrorHandlers)
                    {
                        stepErrorHandler.OnStepFailure(this, stepFailureEventArgs);
                    }
                    if (stepFailureEventArgs.IsHandled)
                        return duration;
                }

                throw;
            }
            return duration;
        }

        private void HandleBlockSwitch(ScenarioBlock block)
        {
            if (contextManager.ScenarioContext.CurrentScenarioBlock != block)
            {
                if (contextManager.ScenarioContext.TestStatus == TestStatus.OK)
                    OnBlockEnd(contextManager.ScenarioContext.CurrentScenarioBlock);

                contextManager.ScenarioContext.CurrentScenarioBlock = block;

                if (contextManager.ScenarioContext.TestStatus == TestStatus.OK)
                    OnBlockStart(contextManager.ScenarioContext.CurrentScenarioBlock);
            }
        }

        private object[] GetExecuteArguments(BindingMatch match)
        {
            if (match.Arguments.Length != match.StepBinding.ParameterTypes.Length)
                throw errorProvider.GetParameterCountError(match, match.Arguments.Length);

            var arguments = match.Arguments.Select(
                (arg, argIndex) => ConvertArg(arg, match.StepBinding.ParameterTypes[argIndex]))
                .ToArray();

            return arguments;
        }

        private object ConvertArg(object value, Type typeToConvertTo)
        {
            Debug.Assert(value != null);
            Debug.Assert(typeToConvertTo != null);

            if (value.GetType().IsAssignableFrom(typeToConvertTo))
                return value;

            return stepArgumentTypeConverter.Convert(value, typeToConvertTo, FeatureContext.BindingCulture);
        }

        #endregion

        #region Given-When-Then
        public void Step(StepDefinitionKeyword keyword, string text, string multilineTextArg, Table tableArg)
        {
            BindingType bindingType = (keyword == StepDefinitionKeyword.And || keyword == StepDefinitionKeyword.But)
                                          ? GetCurrentBindingType()
                                          : (BindingType) keyword;
            ExecuteStep(new StepArgs(bindingType, keyword, text, multilineTextArg, tableArg, contextManager.GetStepContext()));
        }

        private BindingType GetCurrentBindingType()
        {
            ScenarioBlock currentScenarioBlock = contextManager.ScenarioContext.CurrentScenarioBlock;
            return currentScenarioBlock == ScenarioBlock.None ? BindingType.Given : currentScenarioBlock.ToBindingType();
        }

        #endregion

        public void Pending()
        {
            throw errorProvider.GetPendingStepDefinitionError();
        }
    }
}
