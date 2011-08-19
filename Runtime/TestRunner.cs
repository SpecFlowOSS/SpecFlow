using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using MiniDi;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Compatibility;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.ErrorHandling;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow
{
    public class TestRunner : ITestRunner
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

        private IStepDefinitionSkeletonProvider currentStepDefinitionSkeletonProvider;

        [Obsolete("Use DI")]
        static internal TestRunner CreateTestRunnerForCompatibility(Action<IObjectContainer> registerMocks = null)
        {
            var container = TestRunContainerBuilder.CreateContainer();

            if (registerMocks != null)
                registerMocks(container);

            return (TestRunner)container.Resolve<ITestRunner>();
        }

        public TestRunner(IStepFormatter stepFormatter, ITestTracer testTracer, IErrorProvider errorProvider, IStepArgumentTypeConverter stepArgumentTypeConverter, 
            RuntimeConfiguration runtimeConfiguration, IBindingRegistry bindingRegistry, IUnitTestRuntimeProvider unitTestRuntimeProvider, 
            IDictionary<ProgrammingLanguage, IStepDefinitionSkeletonProvider> stepDefinitionSkeletonProviders, IContextManager contextManager)
        {
            this.errorProvider = errorProvider;
            this.contextManager = contextManager;
            this.stepDefinitionSkeletonProviders = stepDefinitionSkeletonProviders;
            this.unitTestRuntimeProvider = unitTestRuntimeProvider;
            this.bindingRegistry = bindingRegistry;
            this.runtimeConfiguration = runtimeConfiguration;
            this.testTracer = testTracer;
            this.stepFormatter = stepFormatter;
            this.stepArgumentTypeConverter = stepArgumentTypeConverter;

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

        public virtual void InitializeTestRunner(Assembly[] bindingAssemblies)
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
            FireEvents(BindingEvent.TestRunStart, null);
        }

        private bool testRunnerEndExecuted = false;

        public virtual void OnTestRunEnd()
        {
            if (testRunnerEndExecuted)
                return;

            testRunnerEndExecuted = true;
            FireEvents(BindingEvent.TestRunEnd, null);
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
            FireEvents(BindingEvent.FeatureStart, contextManager.FeatureContext.FeatureInfo.Tags);
        }

        public void OnFeatureEnd()
        {
            // if the unit test provider would execute the fixture teardown code 
            // only delayed (at the end of the execution), we ignore the 
            // feature-end call, if the feature has been closed already
            if (unitTestRuntimeProvider.DelayedFixtureTearDown &&
                contextManager.FeatureContext == null)
                return;
                
            FireEvents(BindingEvent.FeatureEnd, contextManager.FeatureContext.FeatureInfo.Tags);

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
            contextManager.InitializeScenarioontext(scenarioInfo, this);
            FireScenarioEvents(BindingEvent.ScenarioStart);
        }

        //TODO: rename this method to OnAfterLastStep (breaking change in generation!)
        public void CollectScenarioErrors()
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
        private readonly string[] emptyTagList = new string[0];

        private void FireScenarioEvents(BindingEvent bindingEvent)
        {
            var tags = (contextManager.FeatureContext.FeatureInfo.Tags ?? emptyTagList).Concat(
                contextManager.ScenarioContext.ScenarioInfo.Tags ?? emptyTagList);
            FireEvents(bindingEvent, tags);
        }

        private void FireEvents(BindingEvent bindingEvent, IEnumerable<string> tags)
        {
            foreach (EventBinding eventBinding in bindingRegistry.GetEvents(bindingEvent))
            {
                if (IsTagNeeded(eventBinding.Tags, tags))
                {
                    eventBinding.InvokeAction(null, testTracer);
                }
            }
        }

        private bool IsEmptyTagList(IEnumerable<string> tags)
        {
            return tags == null || !tags.Any();
        }

        private bool IsTagNeeded(IEnumerable<string> filterTags, IEnumerable<string> currentTags)
        {
            if (IsEmptyTagList(filterTags))
                return true;

            if (currentTags == null)
                return false;

            return filterTags.Intersect(currentTags).Any();
        }

        private BindingMatch Match(StepBinding stepBinding, StepArgs stepArgs, bool useParamMatching, bool useScopeMatching)
        {
            Match match = stepBinding.Regex.Match(stepArgs.Text);

            // Check if regexp is a match
            if (!match.Success)
                return null;

            int scopeMatches = 0;
            if (useScopeMatching && stepBinding.IsScoped)
            {
                if (!stepBinding.BindingScope.Match(stepArgs.StepContext, out scopeMatches))
                    return null;
            }

            var bindingMatch = new BindingMatch(stepBinding, match, CalculateExtraArgs(stepArgs), stepArgs, scopeMatches);

            if (useParamMatching)
            {
                // check if the regex + extra arguments match to the binding method parameters
                if (bindingMatch.Arguments.Length != stepBinding.ParameterTypes.Length)
                    return null;

                // Check if regex & extra arguments can be converted to the method parameters
                if (bindingMatch.Arguments.Where(
                    (arg, argIndex) => !CanConvertArg(arg, stepBinding.ParameterTypes[argIndex])).Any())
                    return null;
            }
            return bindingMatch;
        }

        private bool CanConvertArg(object value, Type typeToConvertTo)
        {
            Debug.Assert(value != null);
            Debug.Assert(typeToConvertTo != null);

            if (value.GetType().IsAssignableFrom(typeToConvertTo))
                return true;

            return stepArgumentTypeConverter.CanConvert(value, typeToConvertTo, FeatureContext.Current.BindingCulture);
        }

        private static readonly object[] emptyExtraArgs = new object[0];
        private object[] CalculateExtraArgs(StepArgs stepArgs)
        {
            if (stepArgs.MultilineTextArgument == null && stepArgs.TableArgument == null)
                return emptyExtraArgs;

            var extraArgsList = new List<object>();
            if (stepArgs.MultilineTextArgument != null)
                extraArgsList.Add(stepArgs.MultilineTextArgument);
            if (stepArgs.TableArgument != null)
                extraArgsList.Add(stepArgs.TableArgument);
            return extraArgsList.ToArray();
        }

        private void ExecuteStep(StepArgs stepArgs)
        {
            HandleBlockSwitch(stepArgs.Type.ToScenarioBlock());

            testTracer.TraceStep(stepArgs, true);

            BindingMatch match = null;
            object[] arguments = null;
            try
            {
                match = GetStepMatch(stepArgs);
                arguments = GetExecuteArguments(match);

                if (contextManager.ScenarioContext.TestStatus == TestStatus.OK)
                {
                    TimeSpan duration = ExecuteStepMatch(match, arguments);
                    if (runtimeConfiguration.TraceSuccessfulSteps) 
                        testTracer.TraceStepDone(match, arguments, duration);
                }
                else
                {
                    testTracer.TraceStepSkipped();
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
        }

        private BindingMatch GetStepMatch(StepArgs stepArgs)
        {
            List<BindingMatch> matches = bindingRegistry
                .Where(b => b.Type == stepArgs.Type)
                .Select(binding => Match(binding, stepArgs, true, true))
                .Where(match => match != null)
                .ToList();

            if (matches.Count > 1)
            {
                // if there are both scoped and non-scoped matches, we take the ones with the higher degree of scope matches
                int maxScopeMatches = matches.Max(m => m.ScopeMatches);
                matches.RemoveAll(m => m.ScopeMatches < maxScopeMatches);
            }

            if (matches.Count > 1)
            {
                // we remove duplicate maches for the same method (take the first from each)
                matches = matches.GroupBy(m => m.StepBinding.MethodInfo, (methodInfo, methodMatches) => methodMatches.First()).ToList();
            }

            if (matches.Count == 0)
            {
                // there were either no regex match or it was filtered out by the param/scope matching
                // to provide better error message for the param matching error, we re-run 
                // the matching without param check

                List<BindingMatch> matchesWithoutScopeCheck = GetMatchesWithoutScopeCheck(stepArgs);
//                if (matchesWithoutScopeCheck.Count > 0)
//                {
                    // no match, because of scope filter
//                    throw errorProvider.GetNoMatchBecauseOfScopeFilterError(matchesWithoutScopeCheck, stepArgs);
//                }

                if (matchesWithoutScopeCheck.Count == 0)
                {
                    List<BindingMatch> matchesWithoutParamCheck = GetMatchesWithoutParamCheck(stepArgs);
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

        private List<BindingMatch> GetMatchesWithoutParamCheck(StepArgs stepArgs)
        {
            return bindingRegistry.Where(b => b.Type == stepArgs.Type).Select(binding => Match(binding, stepArgs, false, true)).Where(match => match != null).ToList();
        }

        private List<BindingMatch> GetMatchesWithoutScopeCheck(StepArgs stepArgs)
        {
            return bindingRegistry.Where(b => b.Type == stepArgs.Type).Select(binding => Match(binding, stepArgs, true, false)).Where(match => match != null).ToList();
        }

        private TimeSpan ExecuteStepMatch(BindingMatch match, object[] arguments)
        {
            OnStepStart();
            TimeSpan duration;
            match.StepBinding.InvokeAction(arguments, testTracer, out duration);
            OnStepEnd();

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

            return stepArgumentTypeConverter.Convert(value, typeToConvertTo, FeatureContext.Current.BindingCulture);
        }

        #endregion

        #region Given-When-Then
        public StepContext GetStepContext()
        {
            return new StepContext(contextManager.FeatureContext.FeatureInfo, contextManager.ScenarioContext.ScenarioInfo);
        }

        public void Given(string text, string multilineTextArg, Table tableArg)
        {
            ExecuteStep(new StepArgs(BindingType.Given, StepDefinitionKeyword.Given, text, multilineTextArg, tableArg, GetStepContext()));
        }

        public void When(string text, string multilineTextArg, Table tableArg)
        {
            ExecuteStep(new StepArgs(BindingType.When, StepDefinitionKeyword.When, text, multilineTextArg, tableArg, GetStepContext()));
        }

        public void Then(string text, string multilineTextArg, Table tableArg)
        {
            ExecuteStep(new StepArgs(BindingType.Then, StepDefinitionKeyword.Then, text, multilineTextArg, tableArg, GetStepContext()));
        }

        public void And(string text, string multilineTextArg, Table tableArg)
        {
            BindingType bindingType = GetCurrentBindingType();
            ExecuteStep(new StepArgs(bindingType, StepDefinitionKeyword.And, text, multilineTextArg, tableArg, GetStepContext()));
        }

        public void But(string text, string multilineTextArg, Table tableArg)
        {
            BindingType bindingType = GetCurrentBindingType();
            ExecuteStep(new StepArgs(bindingType, StepDefinitionKeyword.But, text, multilineTextArg, tableArg, GetStepContext()));
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
