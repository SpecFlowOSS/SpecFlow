using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Discovery;
using TechTalk.SpecFlow.Bindings.Reflection;
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
        private readonly IContextManager contextManager;
        private readonly IStepDefinitionMatchService stepDefinitionMatchService;
        private readonly IStepErrorHandler[] stepErrorHandlers;
        private readonly IBindingInvoker bindingInvoker;
        private readonly IRuntimeBindingRegistryBuilder bindingRegistryBuilder;
        private readonly IStepDefinitionSkeletonProvider stepDefinitionSkeletonProvider;

        private ProgrammingLanguage defaultTargetLanguage = ProgrammingLanguage.CSharp;
        private CultureInfo defaultBindingCulture = CultureInfo.CurrentCulture;

        public TestExecutionEngine(IStepFormatter stepFormatter, ITestTracer testTracer, IErrorProvider errorProvider, IStepArgumentTypeConverter stepArgumentTypeConverter, 
            RuntimeConfiguration runtimeConfiguration, IBindingRegistry bindingRegistry, IUnitTestRuntimeProvider unitTestRuntimeProvider, 
            IStepDefinitionSkeletonProvider stepDefinitionSkeletonProvider, IContextManager contextManager, IStepDefinitionMatchService stepDefinitionMatchService,
            IDictionary<string, IStepErrorHandler> stepErrorHandlers, IBindingInvoker bindingInvoker, IRuntimeBindingRegistryBuilder bindingRegistryBuilder)
        {
            this.errorProvider = errorProvider;
            this.bindingInvoker = bindingInvoker;
            this.bindingRegistryBuilder = bindingRegistryBuilder;
            this.contextManager = contextManager;
            this.unitTestRuntimeProvider = unitTestRuntimeProvider;
            this.stepDefinitionSkeletonProvider = stepDefinitionSkeletonProvider;
            this.bindingRegistry = bindingRegistry;
            this.runtimeConfiguration = runtimeConfiguration;
            this.testTracer = testTracer;
            this.stepFormatter = stepFormatter;
            this.stepArgumentTypeConverter = stepArgumentTypeConverter;
            this.stepErrorHandlers = stepErrorHandlers == null ? null : stepErrorHandlers.Values.ToArray();
            this.stepDefinitionMatchService = stepDefinitionMatchService;
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
                bindingRegistryBuilder.BuildBindingsFromAssembly(assembly);
            }
            bindingRegistry.Ready = true;

            OnTestRunnerStart();
#if !SILVERLIGHT
            AppDomain.CurrentDomain.DomainUnload += 
                delegate
                    {
                        OnTestRunEnd();
                    };
            AppDomain.CurrentDomain.ProcessExit += 
                delegate
                    {
                        OnTestRunEnd();
                    };
#endif
        }

        protected virtual void OnTestRunnerStart()
        {
            FireEvents(HookType.BeforeTestRun);
        }

        private bool testRunnerEndExecuted = false;

        public virtual void OnTestRunEnd()
        {
            if (testRunnerEndExecuted)
                return;

            testRunnerEndExecuted = true;
            FireEvents(HookType.AfterTestRun);
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

            // The Generator defines the value of FeatureInfo.Language: either feature-language or language from App.config or the default
            // The runtime can define the binding-culture: Value is configured on App.config, else it is null
            CultureInfo bindingCulture = runtimeConfiguration.BindingCulture ?? featureInfo.Language;

            defaultTargetLanguage = featureInfo.GenerationTargetLanguage;
            defaultBindingCulture = bindingCulture;

            contextManager.InitializeFeatureContext(featureInfo, bindingCulture);
            FireEvents(HookType.BeforeFeature);
        }

        public void OnFeatureEnd()
        {
            // if the unit test provider would execute the fixture teardown code 
            // only delayed (at the end of the execution), we ignore the 
            // feature-end call, if the feature has been closed already
            if (unitTestRuntimeProvider.DelayedFixtureTearDown &&
                contextManager.FeatureContext == null)
                return;
                
            FireEvents(HookType.AfterFeature);

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
            FireScenarioEvents(HookType.BeforeScenario);
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
                string skeleton = stepDefinitionSkeletonProvider.GetBindingClassSkeleton(
                    defaultTargetLanguage, 
                    contextManager.ScenarioContext.MissingSteps.ToArray(), "MyNamespace", "StepDefinitions", runtimeConfiguration.StepDefinitionSkeletonStyle, defaultBindingCulture);

                errorProvider.ThrowPendingError(contextManager.ScenarioContext.TestStatus, string.Format("{0}{2}{1}",
                    errorProvider.GetMissingStepDefinitionError().Message,
                    skeleton,
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
            FireScenarioEvents(HookType.AfterScenario);
            contextManager.CleanupScenarioContext();
        }

        protected virtual void OnBlockStart(ScenarioBlock block)
        {
            if (block == ScenarioBlock.None)
                return;

            FireScenarioEvents(HookType.BeforeScenarioBlock); 
        }

        protected virtual void OnBlockEnd(ScenarioBlock block)
        {
            if (block == ScenarioBlock.None)
                return;

            FireScenarioEvents(HookType.AfterScenarioBlock);
        }

        protected virtual void OnStepStart()
        {
            FireScenarioEvents(HookType.BeforeStep); 
        }

        protected virtual void OnStepEnd()
        {
            FireScenarioEvents(HookType.AfterStep);
        }

        #region Step/event execution
        private void FireScenarioEvents(HookType bindingEvent)
        {
            FireEvents(bindingEvent);
        }

        private void FireEvents(HookType bindingEvent)
        {
            var stepContext = contextManager.GetStepContext();

            foreach (IHookBinding eventBinding in bindingRegistry.GetHooks(bindingEvent))
            {
                int scopeMatches;
                if (eventBinding.IsScoped && !eventBinding.BindingScope.Match(stepContext, out scopeMatches))
                    continue;

                bindingInvoker.InvokeHook(eventBinding, contextManager, testTracer);
            }
        }

        private void ExecuteStep(StepInstance stepInstance)
        {
            HandleBlockSwitch(stepInstance.StepDefinitionType.ToScenarioBlock());

            testTracer.TraceStep(stepInstance, true);

            bool isStepSkipped = contextManager.ScenarioContext.TestStatus != TestStatus.OK;

            BindingMatch match = null;
            object[] arguments = null;
            try
            {
                match = GetStepMatch(stepInstance);
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

        private BindingMatch GetStepMatch(StepInstance stepInstance)
        {
            List<BindingMatch> candidatingMatches;
            StepDefinitionAmbiguityReason ambiguityReason;
            var match = stepDefinitionMatchService.GetBestMatch(stepInstance, contextManager.FeatureContext.BindingCulture, out ambiguityReason, out candidatingMatches);

            if (match.Success)
                return match;

            if (candidatingMatches.Any())
            {
                if (ambiguityReason == StepDefinitionAmbiguityReason.AmbiguousSteps)
                    throw errorProvider.GetAmbiguousMatchError(candidatingMatches, stepInstance);
                
                if (ambiguityReason == StepDefinitionAmbiguityReason.ParameterErrors) // ambiguouity, because of param error
                    throw errorProvider.GetAmbiguousBecauseParamCheckMatchError(candidatingMatches, stepInstance);
            }

            testTracer.TraceNoMatchingStepDefinition(stepInstance, contextManager.FeatureContext.FeatureInfo.GenerationTargetLanguage, contextManager.FeatureContext.BindingCulture, candidatingMatches);
            contextManager.ScenarioContext.MissingSteps.Add(stepInstance);
            throw errorProvider.GetMissingStepDefinitionError();
        }

        private TimeSpan ExecuteStepMatch(BindingMatch match, object[] arguments)
        {
            TimeSpan duration = TimeSpan.Zero;
            try
            {
                bindingInvoker.InvokeBinding(match.StepBinding, contextManager, arguments, testTracer, out duration);
            }
            catch(Exception ex)
            {
                if (stepErrorHandlers != null)
                {
                    StepFailureEventArgs stepFailureEventArgs = new StepFailureEventArgs(match.StepBinding, match.StepContext, ex);
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
            var bindingParameters = match.StepBinding.Method.Parameters.ToArray();
            if (match.Arguments.Length != bindingParameters.Length)
                throw errorProvider.GetParameterCountError(match, match.Arguments.Length);

            var arguments = match.Arguments.Select(
                (arg, argIndex) => ConvertArg(arg, bindingParameters[argIndex].Type))
                .ToArray();

            return arguments;
        }

        private object ConvertArg(object value, IBindingType typeToConvertTo)
        {
            Debug.Assert(value != null);
            Debug.Assert(typeToConvertTo != null);

            if (typeToConvertTo.IsAssignableTo(value.GetType()))
                return value;

            return stepArgumentTypeConverter.Convert(value, typeToConvertTo, FeatureContext.BindingCulture);
        }

        #endregion

        #region Given-When-Then
        public void Step(StepDefinitionKeyword stepDefinitionKeyword, string keyword, string text, string multilineTextArg, Table tableArg)
        {
            StepDefinitionType stepDefinitionType = (stepDefinitionKeyword == StepDefinitionKeyword.And || stepDefinitionKeyword == StepDefinitionKeyword.But)
                                          ? GetCurrentBindingType()
                                          : (StepDefinitionType) stepDefinitionKeyword;
            ExecuteStep(new StepInstance(stepDefinitionType, stepDefinitionKeyword, keyword, text, multilineTextArg, tableArg, contextManager.GetStepContext()));
        }

        private StepDefinitionType GetCurrentBindingType()
        {
            ScenarioBlock currentScenarioBlock = contextManager.ScenarioContext.CurrentScenarioBlock;
            return currentScenarioBlock == ScenarioBlock.None ? StepDefinitionType.Given : currentScenarioBlock.ToBindingType();
        }

        #endregion

        public void Pending()
        {
            throw errorProvider.GetPendingStepDefinitionError();
        }
    }
}
