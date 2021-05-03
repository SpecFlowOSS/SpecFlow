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

        private bool _testRunnerEndExecuted;
        private object _testRunnerEndExecutedLock = new object();
        private bool _testRunnerStartExecuted;

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

            if (_analyticsTransmitter.IsEnabled)
            {
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
            try
            {
                if (_contextManager.ScenarioContext.ScenarioExecutionStatus != ScenarioExecutionStatus.Skipped)
                {
                    FireScenarioEvents(HookType.AfterScenario);
                }
            }
            finally
            {
                _contextManager.CleanupScenarioContext();
            }
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

            foreach (var skippedStepHandler in _contextManager.ScenarioContext.ScenarioContainer.ResolveAll<ISkippedStepHandler>())
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
            int hooksCount = 0;
            var hooks = _bindingRegistry.GetHooks(hookType);
            if (hooks is ICollection<IHookBinding> col)
            {
                hooksCount = col.Count;
                if (hooksCount == 0)
                {
                    // Early out, we got no hooks to call
                    FireRuntimePluginTestExecutionLifecycleEvents(hookType);
                    return;
                }
            }

            StepContext stepContext = null;
            var hooksToInvoke = new List<IHookBinding>(hooksCount);
            foreach (var hookBinding in hooks)
            {
                if (!hookBinding.IsScoped || hookBinding.BindingScope.Match(stepContext ??= _contextManager.GetStepContext(), out _))
                {
                    hooksToInvoke.Add(hookBinding);
                }
            }


            //Note: if a (user-)hook throws an exception the subsequent hooks of the same type are not executed
            try
            {
                if (hooksToInvoke.Count == 1)
                {
                    InvokeHook(_bindingInvoker, hooksToInvoke[0], hookType);
                }
                else
                {
                    hooksToInvoke.Sort((x, y) => x.HookOrder.CompareTo(y.HookOrder));
                    var uniqueBindingMethods = new HashSet<IBindingMethod>();
                    foreach (var hookBinding in hooksToInvoke)
                    {
                        // HACK: The InvokeHook requires an IHookBinding that contains the scope as well
                        // if multiple scopes match the same method, we take the first one.
                        // The InvokeHook uses only the Method anyway...
                        if (uniqueBindingMethods.Add(hookBinding.Method))
                        {
                            InvokeHook(_bindingInvoker, hookBinding, hookType);
                        }
                    }
                }
            }
            catch (Exception hookExceptionCaught)
            {
                SetHookError(hookType, hookExceptionCaught);
                throw;
            }
            finally
            {
                //Note: plugin-hooks are still executed even if a user-hook failed with an exception
                //A plugin-hook should not throw an exception under normal circumstances, exceptions are not handled/caught here
                FireRuntimePluginTestExecutionLifecycleEvents(hookType);
            }
        }

        private void FireRuntimePluginTestExecutionLifecycleEvents(HookType hookType)
        {
            //We pass a container corresponding the type of event
            var container = GetHookContainer(hookType);
            _runtimePluginTestExecutionLifecycleEventEmitter.RaiseExecutionLifecycleEvent(hookType, container);
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
            switch (hookType)
            {
                case HookType.BeforeTestRun:
                case HookType.AfterTestRun:
                    return TestThreadContainer;
                case HookType.BeforeFeature:
                case HookType.AfterFeature:
                    return FeatureContext.FeatureContainer;
                default: // scenario scoped hooks
                    return ScenarioContext.ScenarioContainer;
            }
        }

        private SpecFlowContext GetHookContext(HookType hookType)
        {
            switch (hookType)
            {
                case HookType.BeforeTestRun:
                case HookType.AfterTestRun:
                    return _contextManager.TestThreadContext;
                case HookType.BeforeFeature:
                case HookType.AfterFeature:
                    return _contextManager.FeatureContext;
                default: // scenario scoped hooks
                    return _contextManager.ScenarioContext;
            }
        }

        private void SetHookError(HookType hookType, Exception hookException)
        {
            var context = GetHookContext(hookType);
            if (context != null && context.TestError == null)
                context.TestError = hookException;

            if (context is ScenarioContext scenarioContext)
            {
                scenarioContext.ScenarioExecutionStatus = ScenarioExecutionStatus.TestError;
            }
        }

        private object[] ResolveArguments(IHookBinding hookBinding, IObjectContainer currentContainer)
        {
            var method = hookBinding.Method;
            if (method is null)
            {
                return null;
            }

            var paramArray = method.Parameters.AsArray();
            if (paramArray.Length == 0)
            {
                return null;
            }

            var arguments = new object[paramArray.Length];
            for (var i = 0; i < paramArray.Length; i++)
            {
                arguments[i] = ResolveArgument(currentContainer, paramArray[i]);
            }

            return arguments;
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

                if (isStepSkipped)
                {
                    OnSkipStep();
                }
                else
                {
                    arguments = GetExecuteArguments(match);
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
            var bindingParameters = match.StepBinding.Method.Parameters.AsArray();

            if (match.Arguments.Length != bindingParameters.Length)
                throw _errorProvider.GetParameterCountError(match, match.Arguments.Length);

            if (bindingParameters.Length == 0)
            {
                return Array.Empty<object>();
            }

            var bindingCulture = FeatureContext.BindingCulture;
            var arguments = new object[bindingParameters.Length];
            for (var i = 0; i < arguments.Length; i++)
            {
                object value = match.Arguments[i];
                IBindingType typeToConvertTo = bindingParameters[i].Type;

                Debug.Assert(value != null);
                Debug.Assert(typeToConvertTo != null);
                arguments[i] = _stepArgumentTypeConverter.Convert(value, typeToConvertTo, bindingCulture);
            }
            return arguments;
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
