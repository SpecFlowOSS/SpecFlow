using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using BoDi;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Bindings;
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
        private readonly Configuration.SpecFlowConfiguration specFlowConfiguration;
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
        private readonly IStepDefinitionSkeletonProvider stepDefinitionSkeletonProvider;
        private readonly ITestObjectResolver testObjectResolver;

        private ProgrammingLanguage defaultTargetLanguage = ProgrammingLanguage.CSharp;
        private CultureInfo defaultBindingCulture = CultureInfo.CurrentCulture;

        public TestExecutionEngine(IStepFormatter stepFormatter, ITestTracer testTracer, IErrorProvider errorProvider, IStepArgumentTypeConverter stepArgumentTypeConverter, 
            Configuration.SpecFlowConfiguration specFlowConfiguration, IBindingRegistry bindingRegistry, IUnitTestRuntimeProvider unitTestRuntimeProvider, 
            IStepDefinitionSkeletonProvider stepDefinitionSkeletonProvider, IContextManager contextManager, IStepDefinitionMatchService stepDefinitionMatchService,
            IDictionary<string, IStepErrorHandler> stepErrorHandlers, IBindingInvoker bindingInvoker, ITestObjectResolver testObjectResolver = null, IObjectContainer testThreadContainer = null) //TODO: find a better way to access the container
        {
            this.errorProvider = errorProvider;
            this.bindingInvoker = bindingInvoker;
            this.contextManager = contextManager;
            this.unitTestRuntimeProvider = unitTestRuntimeProvider;
            this.stepDefinitionSkeletonProvider = stepDefinitionSkeletonProvider;
            this.bindingRegistry = bindingRegistry;
            this.specFlowConfiguration = specFlowConfiguration;
            this.testTracer = testTracer;
            this.stepFormatter = stepFormatter;
            this.stepArgumentTypeConverter = stepArgumentTypeConverter;
            this.stepErrorHandlers = stepErrorHandlers == null ? null : stepErrorHandlers.Values.ToArray();
            this.stepDefinitionMatchService = stepDefinitionMatchService;
            this.testObjectResolver = testObjectResolver;
            this.TestThreadContainer = testThreadContainer;
        }

        public FeatureContext FeatureContext
        {
            get { return contextManager.FeatureContext; }
        }

        public ScenarioContext ScenarioContext
        {
            get { return contextManager.ScenarioContext; }
        }

        public virtual void OnTestRunStart()
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
            if (unitTestRuntimeProvider.DelayedFixtureTearDown && FeatureContext != null)
            {
                OnFeatureEnd();
            }


            contextManager.InitializeFeatureContext(featureInfo);

            defaultBindingCulture = FeatureContext.BindingCulture;
            defaultTargetLanguage = featureInfo.GenerationTargetLanguage;
            FireEvents(HookType.BeforeFeature);
        }

        public void OnFeatureEnd()
        {
            // if the unit test provider would execute the fixture teardown code 
            // only delayed (at the end of the execution), we ignore the 
            // feature-end call, if the feature has been closed already
            if (unitTestRuntimeProvider.DelayedFixtureTearDown &&
                FeatureContext == null)
                return;
                
            FireEvents(HookType.AfterFeature);

            if (specFlowConfiguration.TraceTimings)
            {
                FeatureContext.Stopwatch.Stop();
                var duration = FeatureContext.Stopwatch.Elapsed;
                testTracer.TraceDuration(duration, "Feature: " + FeatureContext.FeatureInfo.Title);
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

            if (specFlowConfiguration.TraceTimings)
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
                    contextManager.ScenarioContext.MissingSteps.ToArray(), "MyNamespace", "StepDefinitions", specFlowConfiguration.StepDefinitionSkeletonStyle, defaultBindingCulture);

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

        protected virtual void FireScenarioEvents(HookType bindingEvent)
        {
            FireEvents(bindingEvent);
        }

        private void FireEvents(HookType hookType)
        {
            var stepContext = contextManager.GetStepContext();

            int scopeMatches;
            var matchingHooks = bindingRegistry.GetHooks(hookType)
                .Where(hookBinding => !hookBinding.IsScoped ||
                                       hookBinding.BindingScope.Match(stepContext, out scopeMatches));

            //HACK: The InvokeHook requires an IHookBinding that contains the scope as well
            // if multiple scopes mathching for the same method, we take the first one. 
            // The InvokeHook uses only the Method anyway...
            // The only problem could be if the same method is decorated with hook attributes using different order, 
            // but in this case it is anyway impossible to tell the right ordering.
            var uniqueMatchingHooks = matchingHooks.GroupBy(hookBinding => hookBinding.Method).Select(g => g.First());
            foreach (var hookBinding in uniqueMatchingHooks.OrderBy(x => x.HookOrder))
            {
                InvokeHook(bindingInvoker, hookBinding, hookType);
            }
        }

        protected IObjectContainer TestThreadContainer { get; }

        public void InvokeHook(IBindingInvoker invoker, IHookBinding hookBinding, HookType hookType)
        {
            var currentContainer = GetHookContainer(hookType);
            var arguments = ResolveArguments(hookBinding, currentContainer);

            TimeSpan duration;
            invoker.InvokeBinding(hookBinding, contextManager, arguments, testTracer, out duration);
        }

        private IObjectContainer GetHookContainer(HookType hookType)
        {
            IObjectContainer currentContainer;
            switch (hookType)
            {
                case HookType.BeforeTestRun:
                case HookType.AfterTestRun:
                    currentContainer = TestThreadContainer;
                    break;
                case HookType.BeforeFeature:
                case HookType.AfterFeature:
                    currentContainer = FeatureContext.FeatureContainer;
                    break;
                default: // scenario scoped hooks
                    currentContainer = ScenarioContext.ScenarioContainer;
                    break;
            }
            return currentContainer;
        }

        private object[] ResolveArguments(IHookBinding hookBinding, IObjectContainer currentContainer)
        {
            if (hookBinding.Method == null || !hookBinding.Method.Parameters.Any())
                return null;
            return hookBinding.Method.Parameters.Select(p => ResolveArgument(currentContainer, p)).ToArray();
        }

        private object ResolveArgument(IObjectContainer container, IBindingParameter parameter)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));

            var runtimeParameterType = parameter.Type as RuntimeBindingType;
            if (runtimeParameterType == null)
                throw new SpecFlowException("Parameters can only be resolved for runtime methods.");

            return testObjectResolver.ResolveBindingInstance(runtimeParameterType.Type, container);
        }

        private void ExecuteStep(IContextManager contextManager, StepInstance stepInstance)
        {
            HandleBlockSwitch(stepInstance.StepDefinitionType.ToScenarioBlock());

            testTracer.TraceStep(stepInstance, true);

            bool isStepSkipped = contextManager.ScenarioContext.TestStatus != TestStatus.OK;
            bool onStepStartExecuted = false;

            BindingMatch match = null;
            object[] arguments = null;
            try
            {
                match = GetStepMatch(stepInstance);
                contextManager.StepContext.StepInfo.BindingMatch = match;
                contextManager.StepContext.StepInfo.StepInstance = stepInstance;
                arguments = GetExecuteArguments(match);

                if (isStepSkipped)
                {
                    testTracer.TraceStepSkipped();
                }
                else
                {
                    onStepStartExecuted = true;
                    OnStepStart();
                    TimeSpan duration = ExecuteStepMatch(match, arguments);
                    if (specFlowConfiguration.TraceSuccessfulSteps)
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
                if (specFlowConfiguration.StopAtFirstError)
                    throw;
            }
            finally
            {
                if (onStepStartExecuted)
                {
                    OnStepEnd();
                }
            }
        }

        protected virtual BindingMatch GetStepMatch(StepInstance stepInstance)
        {
            List<BindingMatch> candidatingMatches;
            StepDefinitionAmbiguityReason ambiguityReason;
            var match = stepDefinitionMatchService.GetBestMatch(stepInstance, FeatureContext.BindingCulture, out ambiguityReason, out candidatingMatches);

            if (match.Success)
                return match;

            if (candidatingMatches.Any())
            {
                if (ambiguityReason == StepDefinitionAmbiguityReason.AmbiguousSteps)
                    throw errorProvider.GetAmbiguousMatchError(candidatingMatches, stepInstance);
                
                if (ambiguityReason == StepDefinitionAmbiguityReason.ParameterErrors) // ambiguouity, because of param error
                    throw errorProvider.GetAmbiguousBecauseParamCheckMatchError(candidatingMatches, stepInstance);
            }

            testTracer.TraceNoMatchingStepDefinition(stepInstance, FeatureContext.FeatureInfo.GenerationTargetLanguage, FeatureContext.BindingCulture, candidatingMatches);
            contextManager.ScenarioContext.MissingSteps.Add(stepInstance);
            throw errorProvider.GetMissingStepDefinitionError();
        }

        protected virtual TimeSpan ExecuteStepMatch(BindingMatch match, object[] arguments)
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
            contextManager.InitializeStepContext(new StepInfo(stepDefinitionType, text, tableArg, multilineTextArg));
            try
            {
                var stepInstance = new StepInstance(stepDefinitionType, stepDefinitionKeyword, keyword, text, multilineTextArg, tableArg, contextManager.GetStepContext());
                ExecuteStep(contextManager, stepInstance);
            }
            finally
            {
                contextManager.CleanupStepContext();
            }
        }

        private StepDefinitionType GetCurrentBindingType()
        {
            return contextManager.CurrentTopLevelStepDefinitionType ?? StepDefinitionType.Given;
        }

        #endregion

        public void Pending()
        {
            throw errorProvider.GetPendingStepDefinitionError();
        }
    }
}
