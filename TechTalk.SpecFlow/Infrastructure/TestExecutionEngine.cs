using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using BoDi;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.Analytics;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.CommonModels;
using TechTalk.SpecFlow.Compatibility;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.CucumberMessages;
using TechTalk.SpecFlow.ErrorHandling;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.Infrastructure
{
    public class TestExecutionEngine : ITestExecutionEngine
    {
        private readonly IBindingInvoker _bindingInvoker;
        private readonly IBindingRegistry _bindingRegistry;
        private readonly IContextManager _contextManager;
        private readonly IErrorProvider _errorProvider;
        private readonly IObsoleteStepHandler _obsoleteStepHandler;
        private readonly ICucumberMessageSender _cucumberMessageSender;
        private readonly ITestResultFactory _testResultFactory;
        private readonly ITestPendingMessageFactory _testPendingMessageFactory;
        private readonly ITestUndefinedMessageFactory _testUndefinedMessageFactory;
        private readonly ITestRunResultCollector _testRunResultCollector;
        private readonly SpecFlowConfiguration _specFlowConfiguration;
        private readonly IStepArgumentTypeConverter _stepArgumentTypeConverter;
        private readonly IStepDefinitionMatchService _stepDefinitionMatchService;
        private readonly IStepFormatter _stepFormatter;
        private readonly ITestObjectResolver _testObjectResolver;
        private readonly ITestTracer _testTracer;
        private readonly IUnitTestRuntimeProvider _unitTestRuntimeProvider;
        private readonly IAnalyticsEventProvider _analyticsEventProvider;
        private readonly IAnalyticsTransmitter _analyticsTransmitter;
        private readonly ITestRunnerManager _testRunnerManager;
        private readonly IRuntimePluginTestExecutionLifecycleEventEmitter _runtimePluginTestExecutionLifecycleEventEmitter;
        private CultureInfo _defaultBindingCulture = CultureInfo.CurrentCulture;

        private ProgrammingLanguage _defaultTargetLanguage = ProgrammingLanguage.CSharp;

        private bool _testRunnerEndExecuted = false;
        private object _testRunnerEndExecutedLock = new object();
        private bool _testRunnerStartExecuted = false;
        

        public TestExecutionEngine(
            IStepFormatter stepFormatter,
            ITestTracer testTracer,
            IErrorProvider errorProvider,
            IStepArgumentTypeConverter stepArgumentTypeConverter,
            SpecFlowConfiguration specFlowConfiguration,
            IBindingRegistry bindingRegistry,
            IUnitTestRuntimeProvider unitTestRuntimeProvider,
            IContextManager contextManager,
            IStepDefinitionMatchService stepDefinitionMatchService,
            IBindingInvoker bindingInvoker,
            IObsoleteStepHandler obsoleteStepHandler,
            ICucumberMessageSender cucumberMessageSender,
            ITestResultFactory testResultFactory,
            ITestPendingMessageFactory testPendingMessageFactory,
            ITestUndefinedMessageFactory testUndefinedMessageFactory,
            ITestRunResultCollector testRunResultCollector, 
            IAnalyticsEventProvider analyticsEventProvider, 
            IAnalyticsTransmitter analyticsTransmitter, 
            ITestRunnerManager testRunnerManager,
            IRuntimePluginTestExecutionLifecycleEventEmitter runtimePluginTestExecutionLifecycleEventEmitter,
            ITestObjectResolver testObjectResolver = null,
            IObjectContainer testThreadContainer = null) //TODO: find a better way to access the container
        {
            _errorProvider = errorProvider;
            _bindingInvoker = bindingInvoker;
            _contextManager = contextManager;
            _unitTestRuntimeProvider = unitTestRuntimeProvider;
            _bindingRegistry = bindingRegistry;
            _specFlowConfiguration = specFlowConfiguration;
            _testTracer = testTracer;
            _stepFormatter = stepFormatter;
            _stepArgumentTypeConverter = stepArgumentTypeConverter;
            _stepDefinitionMatchService = stepDefinitionMatchService;
            _testObjectResolver = testObjectResolver;
            TestThreadContainer = testThreadContainer;
            _obsoleteStepHandler = obsoleteStepHandler;
            _cucumberMessageSender = cucumberMessageSender;
            _testResultFactory = testResultFactory;
            _testPendingMessageFactory = testPendingMessageFactory;
            _testUndefinedMessageFactory = testUndefinedMessageFactory;
            _testRunResultCollector = testRunResultCollector;
            _analyticsEventProvider = analyticsEventProvider;
            _analyticsTransmitter = analyticsTransmitter;
            _testRunnerManager = testRunnerManager;
            _runtimePluginTestExecutionLifecycleEventEmitter = runtimePluginTestExecutionLifecycleEventEmitter;
        }

        public FeatureContext FeatureContext => _contextManager.FeatureContext;

        public ScenarioContext ScenarioContext => _contextManager.ScenarioContext;

        public virtual void OnTestRunStart()
        {
            if (_testRunnerStartExecuted)
            {
                return;
            }

            try
            {
                var testAssemblyName = _testRunnerManager.TestAssembly.GetName().Name;
                var projectRunningEvent = _analyticsEventProvider.CreateProjectRunningEvent(testAssemblyName);
                _analyticsTransmitter.TransmitSpecFlowProjectRunningEvent(projectRunningEvent);
            }
            catch (Exception)
            {
                // catch all exceptions since we do not want to break anything
            }

            _testRunnerStartExecuted = true;
            _cucumberMessageSender.SendTestRunStarted();
            _testRunResultCollector.StartCollecting();
            FireEvents(HookType.BeforeTestRun);
        }

        public virtual void OnTestRunEnd()
        {
            lock (_testRunnerEndExecutedLock)
            {
                if (_testRunnerEndExecuted)
                {
                    return;
                }

                _testRunnerEndExecuted = true;
            }

            var testRunResultResult = _testRunResultCollector.GetCurrentResult();

            if (testRunResultResult is ISuccess<TestRunResult> success)
            {
                _cucumberMessageSender.SendTestRunFinished(success.Result);
            }

            FireEvents(HookType.AfterTestRun);
        }

        public virtual void OnFeatureStart(FeatureInfo featureInfo)
        {
            // if the unit test provider would execute the fixture teardown code 
            // only delayed (at the end of the execution), we automatically close 
            // the current feature if necessary
            if (_unitTestRuntimeProvider.DelayedFixtureTearDown && FeatureContext != null)
            {
                OnFeatureEnd();
            }


            _contextManager.InitializeFeatureContext(featureInfo);

            _defaultBindingCulture = FeatureContext.BindingCulture;
            _defaultTargetLanguage = featureInfo.GenerationTargetLanguage;
            FireEvents(HookType.BeforeFeature);
        }

        public virtual void OnFeatureEnd()
        {
            // if the unit test provider would execute the fixture teardown code 
            // only delayed (at the end of the execution), we ignore the 
            // feature-end call, if the feature has been closed already
            if (_unitTestRuntimeProvider.DelayedFixtureTearDown &&
                FeatureContext == null)
                return;

            FireEvents(HookType.AfterFeature);

            if (_specFlowConfiguration.TraceTimings)
            {
                FeatureContext.Stopwatch.Stop();
                var duration = FeatureContext.Stopwatch.Elapsed;
                _testTracer.TraceDuration(duration, "Feature: " + FeatureContext.FeatureInfo.Title);
            }

            _contextManager.CleanupFeatureContext();
        }

        public virtual void OnScenarioInitialize(ScenarioInfo scenarioInfo)
        {
            _contextManager.InitializeScenarioContext(scenarioInfo);
        }

        public virtual void OnScenarioStart()
        {
            _cucumberMessageSender.SendTestCaseStarted(_contextManager.ScenarioContext.ScenarioInfo);
            try
            {
                FireScenarioEvents(HookType.BeforeScenario);
            }
            catch (Exception ex)
            {
                if (_contextManager.ScenarioContext != null)
                {
                    _contextManager.ScenarioContext.ScenarioExecutionStatus = ScenarioExecutionStatus.TestError;
                    _contextManager.ScenarioContext.TestError = ex;
                }
            }
        }

        public virtual void OnAfterLastStep()
        {
            HandleBlockSwitch(ScenarioBlock.None);

            if (_specFlowConfiguration.TraceTimings)
            {
                _contextManager.ScenarioContext.Stopwatch.Stop();
                var duration = _contextManager.ScenarioContext.Stopwatch.Elapsed;
                _testTracer.TraceDuration(duration, "Scenario: " + _contextManager.ScenarioContext.ScenarioInfo.Title);
            }

            var testResultResult = _testResultFactory.BuildFromContext(_contextManager.ScenarioContext, _contextManager.FeatureContext);
            switch (testResultResult)
            {
                case ISuccess<TestResult> success:
                    _cucumberMessageSender.SendTestCaseFinished(_contextManager.ScenarioContext.ScenarioInfo, success.Result);
                    _testRunResultCollector.CollectTestResultForScenario(_contextManager.ScenarioContext.ScenarioInfo, success.Result);
                    break;

                case IFailure failure:
                    _testTracer.TraceWarning(failure.ToString());
                    break;
            }

            if (_contextManager.ScenarioContext.ScenarioExecutionStatus == ScenarioExecutionStatus.OK)
            {
                return;
            }

            if (_contextManager.ScenarioContext.ScenarioExecutionStatus == ScenarioExecutionStatus.Skipped)
            {
                _unitTestRuntimeProvider.TestIgnore("Scenario ignored using @Ignore tag");
                return;
            }

            if (_contextManager.ScenarioContext.ScenarioExecutionStatus == ScenarioExecutionStatus.StepDefinitionPending)
            {
                string pendingStepExceptionMessage = _testPendingMessageFactory.BuildFromScenarioContext(_contextManager.ScenarioContext);
                _errorProvider.ThrowPendingError(_contextManager.ScenarioContext.ScenarioExecutionStatus, pendingStepExceptionMessage);
                return;
            }

            if (_contextManager.ScenarioContext.ScenarioExecutionStatus == ScenarioExecutionStatus.UndefinedStep)
            {
                string undefinedStepExceptionMessage = _testUndefinedMessageFactory.BuildFromContext(_contextManager.ScenarioContext, _contextManager.FeatureContext);
                _errorProvider.ThrowPendingError(_contextManager.ScenarioContext.ScenarioExecutionStatus, undefinedStepExceptionMessage);
                return;
            }

            if (_contextManager.ScenarioContext.TestError == null)
            {
                throw new InvalidOperationException("test failed with an unknown error");
            }

            _contextManager.ScenarioContext.TestError.PreserveStackTrace();
            throw _contextManager.ScenarioContext.TestError;
        }

        public virtual void OnScenarioEnd()
        {
            if (_contextManager.ScenarioContext.ScenarioExecutionStatus != ScenarioExecutionStatus.Skipped)
            {
                FireScenarioEvents(HookType.AfterScenario);
            }

            _contextManager.CleanupScenarioContext();
        }

        public virtual void OnScenarioSkipped()
        {
            // after discussing the placement of message sending points, this placement causes far less effort than rewriting the whole logic
            _cucumberMessageSender.SendTestCaseStarted(_contextManager.ScenarioContext.ScenarioInfo);
            _contextManager.ScenarioContext.ScenarioExecutionStatus = ScenarioExecutionStatus.Skipped;
        }

        public virtual void Pending()
        {
            throw _errorProvider.GetPendingStepDefinitionError();
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
        protected virtual void OnSkipStep()
        {
            _testTracer.TraceStepSkipped();

            var skippedStepHandlers = _contextManager.ScenarioContext.ScenarioContainer.ResolveAll<ISkippedStepHandler>().ToArray();
            foreach (var skippedStepHandler in skippedStepHandlers)
            {
                skippedStepHandler.Handle(_contextManager.ScenarioContext);
            }
        }

        #region Step/event execution

        protected virtual void FireScenarioEvents(HookType bindingEvent)
        {
            FireEvents(bindingEvent);
        }

        private void FireEvents(HookType hookType)
        {
            var stepContext = _contextManager.GetStepContext();

            var matchingHooks = _bindingRegistry.GetHooks(hookType)
                .Where(hookBinding => !hookBinding.IsScoped ||
                                      hookBinding.BindingScope.Match(stepContext, out int _));

            //HACK: The InvokeHook requires an IHookBinding that contains the scope as well
            // if multiple scopes mathching for the same method, we take the first one. 
            // The InvokeHook uses only the Method anyway...
            // The only problem could be if the same method is decorated with hook attributes using different order, 
            // but in this case it is anyway impossible to tell the right ordering.
            var uniqueMatchingHooks = matchingHooks.GroupBy(hookBinding => hookBinding.Method).Select(g => g.First());
            foreach (var hookBinding in uniqueMatchingHooks.OrderBy(x => x.HookOrder))
            {
                InvokeHook(_bindingInvoker, hookBinding, hookType);
            }

            FireRuntimePluginTestExecutionLifecycleEvents(hookType);
        }

        private void FireRuntimePluginTestExecutionLifecycleEvents(HookType hookType)
        {
            //We pass a container corresponding the type of event
            var container = GetHookContainer(hookType);
            _runtimePluginTestExecutionLifecycleEventEmitter.RasiseExecutionLifecycleEvent(hookType, container);
        }

        protected IObjectContainer TestThreadContainer { get; }

        public virtual void InvokeHook(IBindingInvoker invoker, IHookBinding hookBinding, HookType hookType)
        {
            var currentContainer = GetHookContainer(hookType);
            var arguments = ResolveArguments(hookBinding, currentContainer);

            invoker.InvokeBinding(hookBinding, _contextManager, arguments, _testTracer, out _);
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

            return _testObjectResolver.ResolveBindingInstance(runtimeParameterType.Type, container);
        }

        private void ExecuteStep(IContextManager contextManager, StepInstance stepInstance)
        {
            HandleBlockSwitch(stepInstance.StepDefinitionType.ToScenarioBlock());

            _testTracer.TraceStep(stepInstance, true);

            bool isStepSkipped = contextManager.ScenarioContext.ScenarioExecutionStatus != ScenarioExecutionStatus.OK;
            bool onStepStartExecuted = false;

            BindingMatch match = null;
            object[] arguments = null;
            TimeSpan duration = TimeSpan.Zero;
            try
            {
                match = GetStepMatch(stepInstance);
                contextManager.StepContext.StepInfo.BindingMatch = match;
                contextManager.StepContext.StepInfo.StepInstance = stepInstance;
                arguments = GetExecuteArguments(match);
                

                if (isStepSkipped)
                {
                    OnSkipStep();
                }
                else
                {
                    _obsoleteStepHandler.Handle(match);

                    onStepStartExecuted = true;
                    OnStepStart();
                    ExecuteStepMatch(match, arguments, out duration);
                    if (_specFlowConfiguration.TraceSuccessfulSteps)
                        _testTracer.TraceStepDone(match, arguments, duration);
                }
            }
            catch (PendingStepException)
            {
                Debug.Assert(match != null);
                Debug.Assert(arguments != null);

                _testTracer.TraceStepPending(match, arguments);
                contextManager.ScenarioContext.PendingSteps.Add(
                    _stepFormatter.GetMatchText(match, arguments));

                if (contextManager.ScenarioContext.ScenarioExecutionStatus < ScenarioExecutionStatus.StepDefinitionPending)
                    contextManager.ScenarioContext.ScenarioExecutionStatus = ScenarioExecutionStatus.StepDefinitionPending;
            }
            catch (MissingStepDefinitionException)
            {
                if (contextManager.ScenarioContext.ScenarioExecutionStatus < ScenarioExecutionStatus.UndefinedStep)
                    contextManager.ScenarioContext.ScenarioExecutionStatus = ScenarioExecutionStatus.UndefinedStep;
            }
            catch (BindingException ex)
            {
                _testTracer.TraceBindingError(ex);
                if (contextManager.ScenarioContext.ScenarioExecutionStatus < ScenarioExecutionStatus.BindingError)
                {
                    contextManager.ScenarioContext.ScenarioExecutionStatus = ScenarioExecutionStatus.BindingError;
                    contextManager.ScenarioContext.TestError = ex;
                }
            }
            catch (Exception ex)
            {
                _testTracer.TraceError(ex, duration);

                if (contextManager.ScenarioContext.ScenarioExecutionStatus < ScenarioExecutionStatus.TestError)
                {
                    contextManager.ScenarioContext.ScenarioExecutionStatus = ScenarioExecutionStatus.TestError;
                    contextManager.ScenarioContext.TestError = ex;
                }

                if (_specFlowConfiguration.StopAtFirstError)
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
            var match = _stepDefinitionMatchService.GetBestMatch(stepInstance, FeatureContext.BindingCulture, out var ambiguityReason, out var candidatingMatches);

            if (match.Success)
                return match;

            if (candidatingMatches.Any())
            {
                if (ambiguityReason == StepDefinitionAmbiguityReason.AmbiguousSteps)
                    throw _errorProvider.GetAmbiguousMatchError(candidatingMatches, stepInstance);

                if (ambiguityReason == StepDefinitionAmbiguityReason.ParameterErrors) // ambiguouity, because of param error
                    throw _errorProvider.GetAmbiguousBecauseParamCheckMatchError(candidatingMatches, stepInstance);
            }

            _testTracer.TraceNoMatchingStepDefinition(stepInstance, FeatureContext.FeatureInfo.GenerationTargetLanguage, FeatureContext.BindingCulture, candidatingMatches);
            _contextManager.ScenarioContext.MissingSteps.Add(stepInstance);
            throw _errorProvider.GetMissingStepDefinitionError();
        }

        protected virtual void ExecuteStepMatch(BindingMatch match, object[] arguments, out TimeSpan duration)
        {
            _bindingInvoker.InvokeBinding(match.StepBinding, _contextManager, arguments, _testTracer, out duration);
        }

        private void HandleBlockSwitch(ScenarioBlock block)
        {
            if (_contextManager == null)
            {
                throw new ArgumentNullException(nameof(_contextManager));
            }
            
            if (_contextManager.ScenarioContext == null)
            {
                throw new ArgumentNullException(nameof(_contextManager.ScenarioContext));
            }

            if (_contextManager.ScenarioContext.CurrentScenarioBlock != block)
            {
                if (_contextManager.ScenarioContext.ScenarioExecutionStatus == ScenarioExecutionStatus.OK)
                    OnBlockEnd(_contextManager.ScenarioContext.CurrentScenarioBlock);

                _contextManager.ScenarioContext.CurrentScenarioBlock = block;

                if (_contextManager.ScenarioContext.ScenarioExecutionStatus == ScenarioExecutionStatus.OK)
                    OnBlockStart(_contextManager.ScenarioContext.CurrentScenarioBlock);
            }
        }

        private object[] GetExecuteArguments(BindingMatch match)
        {
            var bindingParameters = match.StepBinding.Method.Parameters.ToArray();
            if (match.Arguments.Length != bindingParameters.Length)
                throw _errorProvider.GetParameterCountError(match, match.Arguments.Length);

            var arguments = match.Arguments.Select(
                    (arg, argIndex) => ConvertArg(arg, bindingParameters[argIndex].Type))
                .ToArray();

            return arguments;
        }

        private object ConvertArg(object value, IBindingType typeToConvertTo)
        {
            Debug.Assert(value != null);
            Debug.Assert(typeToConvertTo != null);

            return _stepArgumentTypeConverter.Convert(value, typeToConvertTo, FeatureContext.BindingCulture);
        }

        #endregion

        #region Given-When-Then

        public virtual void Step(StepDefinitionKeyword stepDefinitionKeyword, string keyword, string text, string multilineTextArg, Table tableArg)
        {
            StepDefinitionType stepDefinitionType = stepDefinitionKeyword == StepDefinitionKeyword.And || stepDefinitionKeyword == StepDefinitionKeyword.But
                ? GetCurrentBindingType()
                : (StepDefinitionType) stepDefinitionKeyword;
            _contextManager.InitializeStepContext(new StepInfo(stepDefinitionType, text, tableArg, multilineTextArg));
            try
            {
                var stepInstance = new StepInstance(stepDefinitionType, stepDefinitionKeyword, keyword, text, multilineTextArg, tableArg, _contextManager.GetStepContext());
                ExecuteStep(_contextManager, stepInstance);
            }
            finally
            {
                _contextManager.CleanupStepContext();
            }
        }

        private StepDefinitionType GetCurrentBindingType()
        {
            return _contextManager.CurrentTopLevelStepDefinitionType ?? StepDefinitionType.Given;
        }

        #endregion
    }
}
