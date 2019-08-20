using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BoDi;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.CommonModels;
using TechTalk.SpecFlow.Compatibility;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.CucumberMessages;
using TechTalk.SpecFlow.ErrorHandling;
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
        private readonly SpecFlowConfiguration _specFlowConfiguration;
        private readonly IStepArgumentTypeConverter _stepArgumentTypeConverter;
        private readonly IStepDefinitionMatchService _stepDefinitionMatchService;
        private readonly IStepErrorHandler[] _stepErrorHandlers;
        private readonly IStepFormatter _stepFormatter;
        private readonly ITestObjectResolver _testObjectResolver;
        private readonly ITestTracer _testTracer;
        private readonly IUnitTestRuntimeProvider _unitTestRuntimeProvider;
        private CultureInfo _defaultBindingCulture = CultureInfo.CurrentCulture;

        private ProgrammingLanguage _defaultTargetLanguage = ProgrammingLanguage.CSharp;

        private bool _testRunnerEndExecuted = false;
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
            IDictionary<string, IStepErrorHandler> stepErrorHandlers,
            IBindingInvoker bindingInvoker,
            IObsoleteStepHandler obsoleteStepHandler,
            ICucumberMessageSender cucumberMessageSender,
            ITestResultFactory testResultFactory,
            ITestPendingMessageFactory testPendingMessageFactory,
            ITestUndefinedMessageFactory testUndefinedMessageFactory,
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
            _stepErrorHandlers = stepErrorHandlers?.Values.ToArray();
            _stepDefinitionMatchService = stepDefinitionMatchService;
            _testObjectResolver = testObjectResolver;
            TestThreadContainer = testThreadContainer;
            _obsoleteStepHandler = obsoleteStepHandler;
            _cucumberMessageSender = cucumberMessageSender;
            _testResultFactory = testResultFactory;
            _testPendingMessageFactory = testPendingMessageFactory;
            _testUndefinedMessageFactory = testUndefinedMessageFactory;
        }

        public FeatureContext FeatureContext => _contextManager.FeatureContext;

        public ScenarioContext ScenarioContext => _contextManager.ScenarioContext;

        public virtual async Task OnTestRunStartAsync()
        {
            if (_testRunnerStartExecuted)
            {
                return;
            }

            _testRunnerStartExecuted = true;
            _cucumberMessageSender.SendTestRunStarted();
            await FireEventsAsync(HookType.BeforeTestRun);
        }

        public virtual async Task OnTestRunEndAsync()
        {
            if (_testRunnerEndExecuted)
            {
                return;
            }

            _testRunnerEndExecuted = true;
            await FireEventsAsync(HookType.AfterTestRun);
        }

        public async Task OnFeatureStartAsync(FeatureInfo featureInfo)
        {
            // if the unit test provider would execute the fixture teardown code 
            // only delayed (at the end of the execution), we automatically close 
            // the current feature if necessary
            if (_unitTestRuntimeProvider.DelayedFixtureTearDown && FeatureContext != null)
            {
                await OnFeatureEndAsync();
            }


            _contextManager.InitializeFeatureContext(featureInfo);

            _defaultBindingCulture = FeatureContext.BindingCulture;
            _defaultTargetLanguage = featureInfo.GenerationTargetLanguage;
            await FireEventsAsync(HookType.BeforeFeature);
        }

        public async Task OnFeatureEndAsync()
        {
            // if the unit test provider would execute the fixture teardown code 
            // only delayed (at the end of the execution), we ignore the 
            // feature-end call, if the feature has been closed already
            if (_unitTestRuntimeProvider.DelayedFixtureTearDown &&
                FeatureContext == null)
                return;

            await FireEventsAsync(HookType.AfterFeature);

            if (_specFlowConfiguration.TraceTimings)
            {
                FeatureContext.Stopwatch.Stop();
                var duration = FeatureContext.Stopwatch.Elapsed;
                _testTracer.TraceDuration(duration, "Feature: " + FeatureContext.FeatureInfo.Title);
            }

            _contextManager.CleanupFeatureContext();
        }

        public void OnScenarioInitialize(ScenarioInfo scenarioInfo)
        {
            _contextManager.InitializeScenarioContext(scenarioInfo);
        }

        public async Task OnScenarioStartAsync()
        {
            _cucumberMessageSender.SendTestCaseStarted(_contextManager.ScenarioContext.ScenarioInfo);
            await FireScenarioEventsAsync(HookType.BeforeScenario);
        }

        public async Task OnAfterLastStepAsync()
        {
            await HandleBlockSwitchAsync(ScenarioBlock.None);

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

        public async Task OnScenarioEndAsync()
        {
            if (_contextManager.ScenarioContext == null)
            {
                return;
            }

            if (_contextManager.ScenarioContext.ScenarioExecutionStatus != ScenarioExecutionStatus.Skipped)
            {
                await FireScenarioEventsAsync(HookType.AfterScenario);  
            }

            _contextManager.CleanupScenarioContext();
        }

        public void OnScenarioSkipped()
        {
            // after discussing the placement of message sending points, this placement causes far less effort than rewriting the whole logic
            _cucumberMessageSender.SendTestCaseStarted(_contextManager.ScenarioContext.ScenarioInfo);
            _contextManager.ScenarioContext.ScenarioExecutionStatus = ScenarioExecutionStatus.Skipped;
        }

        public void Pending()
        {
            throw _errorProvider.GetPendingStepDefinitionError();
        }

        protected virtual async Task OnBlockStartAsync(ScenarioBlock block)
        {
            if (block == ScenarioBlock.None)
                return;

            await FireScenarioEventsAsync(HookType.BeforeScenarioBlock);
        }

        protected virtual async Task OnBlockEndAsync(ScenarioBlock block)
        {
            if (block == ScenarioBlock.None)
                return;

            await FireScenarioEventsAsync(HookType.AfterScenarioBlock);
        }

        protected virtual async Task OnStepStartAsync()
        {
            await FireScenarioEventsAsync(HookType.BeforeStep);
        }

        protected virtual async Task OnStepEndAsync()
        {
            await FireScenarioEventsAsync(HookType.AfterStep);
        }

        #region Step/event execution

        protected virtual async Task FireScenarioEventsAsync(HookType bindingEvent)
        {
            await FireEventsAsync(bindingEvent);
        }

        private async Task FireEventsAsync(HookType hookType)
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
                await InvokeHookAsync(_bindingInvoker, hookBinding, hookType);
            }
        }

        protected IObjectContainer TestThreadContainer { get; }

        public async Task InvokeHookAsync(IBindingInvoker invoker, IHookBinding hookBinding, HookType hookType)
        {
            var currentContainer = GetHookContainer(hookType);
            var arguments = ResolveArguments(hookBinding, currentContainer);

            await invoker.InvokeBindingAsync(hookBinding, _contextManager, arguments, _testTracer);
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

        private async Task ExecuteStepAsync(IContextManager contextManager, StepInstance stepInstance)
        {
            await HandleBlockSwitchAsync(stepInstance.StepDefinitionType.ToScenarioBlock());

            _testTracer.TraceStep(stepInstance, true);

            bool isStepSkipped = contextManager.ScenarioContext.ScenarioExecutionStatus != ScenarioExecutionStatus.OK;
            bool onStepStartExecuted = false;

            BindingMatch match = null;
            object[] arguments = null;
            try
            {
                match = GetStepMatch(stepInstance);
                contextManager.StepContext.StepInfo.BindingMatch = match;
                contextManager.StepContext.StepInfo.StepInstance = stepInstance;
                arguments = await GetExecuteArgumentsAsync(match);

                if (isStepSkipped)
                {
                    _testTracer.TraceStepSkipped();
                }
                else
                {
                    _obsoleteStepHandler.Handle(match);

                    onStepStartExecuted = true;
                    await OnStepStartAsync();
                    var duration = await ExecuteStepMatchAsync(match, arguments);
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
                _testTracer.TraceError(ex);

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
                    await OnStepEndAsync();
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

        protected virtual async Task<TimeSpan> ExecuteStepMatchAsync(BindingMatch match, object[] arguments)
        {
            TimeSpan duration = TimeSpan.Zero;
            try
            {
                await _bindingInvoker.InvokeBindingAsync(match.StepBinding, _contextManager, arguments, _testTracer);
            }
            catch (Exception ex)
            {
                if (_stepErrorHandlers != null)
                {
                    StepFailureEventArgs stepFailureEventArgs = new StepFailureEventArgs(match.StepBinding, match.StepContext, ex);
                    foreach (var stepErrorHandler in _stepErrorHandlers)
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

        private async Task HandleBlockSwitchAsync(ScenarioBlock block)
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
                    await OnBlockEndAsync(_contextManager.ScenarioContext.CurrentScenarioBlock);

                _contextManager.ScenarioContext.CurrentScenarioBlock = block;

                if (_contextManager.ScenarioContext.ScenarioExecutionStatus == ScenarioExecutionStatus.OK)
                    await OnBlockStartAsync(_contextManager.ScenarioContext.CurrentScenarioBlock);
            }
        }

        private async Task<object[]> GetExecuteArgumentsAsync(BindingMatch match)
        {
            var bindingParameters = match.StepBinding.Method.Parameters.ToArray();
            if (match.Arguments.Length != bindingParameters.Length)
                throw _errorProvider.GetParameterCountError(match, match.Arguments.Length);

            var arguments = new object[match.Arguments.Length];

            for (var i = 0; i < match.Arguments.Length; i++)
            {
                arguments[i] = await ConvertArg(match.Arguments[i], bindingParameters[i].Type);
            }

            return arguments;
        }

        private async Task<object> ConvertArg(object value, IBindingType typeToConvertTo)
        {
            Debug.Assert(value != null);
            Debug.Assert(typeToConvertTo != null);

            return await _stepArgumentTypeConverter.ConvertAsync(value, typeToConvertTo, FeatureContext.BindingCulture);
        }

        #endregion

        #region Given-When-Then

        public async Task StepAsync(StepDefinitionKeyword stepDefinitionKeyword, string keyword, string text, string multilineTextArg, Table tableArg)
        {
            StepDefinitionType stepDefinitionType = stepDefinitionKeyword == StepDefinitionKeyword.And || stepDefinitionKeyword == StepDefinitionKeyword.But
                ? GetCurrentBindingType()
                : (StepDefinitionType) stepDefinitionKeyword;
            _contextManager.InitializeStepContext(new StepInfo(stepDefinitionType, text, tableArg, multilineTextArg));
            try
            {
                var stepInstance = new StepInstance(stepDefinitionType, stepDefinitionKeyword, keyword, text, multilineTextArg, tableArg, _contextManager.GetStepContext());
                await ExecuteStepAsync(_contextManager, stepInstance);
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
