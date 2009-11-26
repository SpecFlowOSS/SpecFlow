using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.ErrorHandling;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow
{
    public class TestRunner : ITestRunner
    {
        private BindingRegistry bindingRegistry = null;
        private readonly ErrorProvider errorProvider;
        private readonly TestTracer testTracer;
        private readonly IUnitTestRuntimeProvider unitTestRuntimeProvider;
        private readonly StepFormatter stepFormatter;
        private readonly StepDefinitionSkeletonProvider stepDefinitionSkeletonProvider;
        private readonly IStepArgumentTypeConverter stepArgumentTypeConverter = new StepArgumentTypeConverter(); 

        public TestRunner()
        {
            errorProvider = ObjectContainer.ErrorProvider;
            testTracer = ObjectContainer.TestTracer;
            unitTestRuntimeProvider = ObjectContainer.UnitTestRuntimeProvider;
            stepFormatter = ObjectContainer.StepFormatter;
            stepDefinitionSkeletonProvider = ObjectContainer.StepDefinitionSkeletonProvider;
            stepArgumentTypeConverter = ObjectContainer.StepArgumentTypeConverter;
        }

        public virtual void InitializeTestRunner(Assembly[] bindingAssemblies)
        {
            bindingRegistry = new BindingRegistry();
            foreach (Assembly assembly in bindingAssemblies)
            {
                bindingRegistry.BuildBindingsFromAssembly(assembly);
            }

            OnTestRunnerStart();
            AppDomain.CurrentDomain.DomainUnload += 
                delegate
                    {
                        OnTestRunnerEnd();
                    };
        }

        protected virtual void OnTestRunnerStart()
        {
            FireEvents(BindingEvent.TestRunStart, null);
        }

        protected virtual void OnTestRunnerEnd()
        {
            FireEvents(BindingEvent.TestRunEnd, null);
        }

        public void OnFeatureStart(FeatureInfo featureInfo)
        {
            // if the unit test provider would execute the fixture teardown code 
            // only delayed (at the end of the execution), we automatically close 
            // the current feature if necessary
            if (unitTestRuntimeProvider.DelayedFixtureTearDown &&
                ObjectContainer.FeatureContext != null)
            {
                OnFeatureEnd();
            }

            ObjectContainer.FeatureContext = new FeatureContext(featureInfo);
            FireEvents(BindingEvent.FeatureStart, ObjectContainer.FeatureContext.FeatureInfo.Tags);
        }

        public void OnFeatureEnd()
        {
            // if the unit test provider would execute the fixture teardown code 
            // only delayed (at the end of the execution), we ignore the 
            // feature-end call, if the feature has been closed already
            if (unitTestRuntimeProvider.DelayedFixtureTearDown &&
                ObjectContainer.FeatureContext == null)
                return;
                
            FireEvents(BindingEvent.FeatureEnd, ObjectContainer.FeatureContext.FeatureInfo.Tags);

            if (RuntimeConfiguration.Current.TraceTimings)
            {
                ObjectContainer.FeatureContext.Stopwatch.Stop();
                var duration = ObjectContainer.FeatureContext.Stopwatch.Elapsed;
                testTracer.TraceDuration(duration, "Feature: " + ObjectContainer.FeatureContext.FeatureInfo.Title);
            }

            ObjectContainer.FeatureContext = null;
        }

        public void OnScenarioStart(ScenarioInfo scenarioInfo)
        {
            ObjectContainer.ScenarioContext = new ScenarioContext(scenarioInfo);
            FireScenarioEvents(BindingEvent.ScenarioStart);
        }

        //TODO: rename this method to OnAfterLastStep (breaking change in generation!)
        public void CollectScenarioErrors()
        {
            HandleBlockSwitch(ScenarioBlock.None);

            if (RuntimeConfiguration.Current.TraceTimings)
            {
                ObjectContainer.ScenarioContext.Stopwatch.Stop();
                var duration = ObjectContainer.ScenarioContext.Stopwatch.Elapsed;
                testTracer.TraceDuration(duration, "Scenario: " + ObjectContainer.ScenarioContext.ScenarioInfo.Title);
            }

            if (ObjectContainer.ScenarioContext.TestStatus == TestStatus.OK)
                return;

            if (ObjectContainer.ScenarioContext.TestStatus == TestStatus.StepDefinitionPending)
            {
                var pendingSteps = ObjectContainer.ScenarioContext.PendingSteps.Distinct().OrderBy(s => s);
                errorProvider.ThrowPendingError(ObjectContainer.ScenarioContext.TestStatus, string.Format("{0}{2}  {1}",
                    errorProvider.GetPendingStepDefinitionError().Message,
                    string.Join(Environment.NewLine + "  ", pendingSteps.ToArray()),
                    Environment.NewLine));
                return;
            }

            if (ObjectContainer.ScenarioContext.TestStatus == TestStatus.MissingStepDefinition)
            {
                var missingSteps = ObjectContainer.ScenarioContext.MissingSteps.Distinct().OrderBy(s => s);
                string bindingSkeleton =
                    stepDefinitionSkeletonProvider.GetBindingClassSkeleton(
                        string.Join(Environment.NewLine, missingSteps.ToArray()));
                errorProvider.ThrowPendingError(ObjectContainer.ScenarioContext.TestStatus, string.Format("{0}{2}{1}",
                    errorProvider.GetMissingStepDefinitionError().Message,
                    bindingSkeleton,
                    Environment.NewLine));
                return;
            }

            if (ObjectContainer.ScenarioContext.TestError == null)
                throw new InvalidOperationException("test failed with an unknown error");

            PreserveStackTrace(ObjectContainer.ScenarioContext.TestError);
            throw ObjectContainer.ScenarioContext.TestError;
        }

        public void OnScenarioEnd()
        {
            FireScenarioEvents(BindingEvent.ScenarioEnd);
            ObjectContainer.ScenarioContext = null;
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
            var tags = (ObjectContainer.FeatureContext.FeatureInfo.Tags ?? emptyTagList).Concat(
                ObjectContainer.ScenarioContext.ScenarioInfo.Tags ?? emptyTagList);
            FireEvents(bindingEvent, tags);
        }

        private void FireEvents(BindingEvent bindingEvent, IEnumerable<string> tags)
        {
            foreach (EventBinding eventBinding in bindingRegistry.GetEvents(bindingEvent))
            {
                if (IsTagNeeded(eventBinding.Tags, tags))
                {
                    InvokeAction(eventBinding.BindingAction, null, eventBinding.MethodInfo);
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

        private BindingMatch Match(StepBinding stepBinding, StepArgs stepArgs)
        {
            Match match = stepBinding.Regex.Match(stepArgs.Text);
            if (!match.Success)
                return null;

            object[] extraArgs = null;
            if (stepArgs.MultilineTextArgument != null || stepArgs.TableArgument != null)
            {
                List<object> extraArgsList = new List<object>();
                if (stepArgs.MultilineTextArgument != null)
                    extraArgsList.Add(stepArgs.MultilineTextArgument);
                if (stepArgs.TableArgument != null)
                    extraArgsList.Add(stepArgs.TableArgument);
                extraArgs = extraArgsList.ToArray();
            }

            return new BindingMatch(stepBinding, match, extraArgs, stepArgs);
        }

        private void ExecuteStep(StepArgs stepArgs)
        {
            testTracer.TraceStep(stepArgs, true);

            HandleBlockSwitch(stepArgs.Type.ToScenarioBlock());

            BindingMatch match = null;
            object[] arguments = null;
            try
            {
                match = GetStepMatch(stepArgs);
                arguments = GetExecuteArguments(match);

                if (ObjectContainer.ScenarioContext.TestStatus == TestStatus.OK)
                {
                    TimeSpan duration = ExecuteStepMatch(match, arguments);
                    if (RuntimeConfiguration.Current.TraceSuccessfulSteps) 
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
                ObjectContainer.ScenarioContext.PendingSteps.Add(
                    stepFormatter.GetMatchText(match, arguments));

                if (ObjectContainer.ScenarioContext.TestStatus < TestStatus.StepDefinitionPending)
                    ObjectContainer.ScenarioContext.TestStatus = TestStatus.StepDefinitionPending;
            }
            catch(MissingStepDefinitionException)
            {
                if (ObjectContainer.ScenarioContext.TestStatus < TestStatus.MissingStepDefinition)
                    ObjectContainer.ScenarioContext.TestStatus = TestStatus.MissingStepDefinition;
            }
            catch(BindingException ex)
            {
                testTracer.TraceBindingError(ex);
                if (ObjectContainer.ScenarioContext.TestStatus < TestStatus.BindingError)
                {
                    ObjectContainer.ScenarioContext.TestStatus = TestStatus.BindingError;
                    ObjectContainer.ScenarioContext.TestError = ex;
                }
            }
            catch(Exception ex)
            {
                testTracer.TraceError(ex);

                if (ObjectContainer.ScenarioContext.TestStatus < TestStatus.TestError)
                {
                    ObjectContainer.ScenarioContext.TestStatus = TestStatus.TestError;
                    ObjectContainer.ScenarioContext.TestError = ex;
                }
                if (RuntimeConfiguration.Current.StopAtFirstError)
                    throw;
            }
        }

        private BindingMatch GetStepMatch(StepArgs stepArgs)
        {
            List<BindingMatch> matches = new List<BindingMatch>();

            foreach (StepBinding binding in bindingRegistry.Where(b => b.Type == stepArgs.Type))
            {
                BindingMatch match = Match(binding, stepArgs);
                if (match == null)
                    continue;

                matches.Add(match);
                if (!RuntimeConfiguration.Current.DetectAmbiguousMatches)
                    break;
            }

            if (matches.Count == 0)
            {
                testTracer.TraceNoMatchingStepDefinition(stepArgs);
                ObjectContainer.ScenarioContext.MissingSteps.Add(
                    stepDefinitionSkeletonProvider.GetStepDefinitionSkeleton(stepArgs));
                throw errorProvider.GetMissingStepDefinitionError();
            }
            if (matches.Count > 1)
            {
                throw errorProvider.GetAmbiguousMatchError(matches, stepArgs);
            }
            return matches[0];
        }

        internal void PreserveStackTrace(Exception ex)
        {
            typeof(Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(ex, new object[0]);
        }

        private TimeSpan ExecuteStepMatch(BindingMatch match, object[] arguments)
        {
            OnStepStart();
            TimeSpan duration = InvokeAction(match.StepBinding.BindingAction, arguments, match.StepBinding.MethodInfo);
            OnStepEnd();

            return duration;
        }

        private void HandleBlockSwitch(ScenarioBlock block)
        {
            if (ObjectContainer.ScenarioContext.CurrentScenarioBlock != block)
            {
                if (ObjectContainer.ScenarioContext.TestStatus == TestStatus.OK)
                    OnBlockEnd(ObjectContainer.ScenarioContext.CurrentScenarioBlock);

                ObjectContainer.ScenarioContext.CurrentScenarioBlock = block;

                if (ObjectContainer.ScenarioContext.TestStatus == TestStatus.OK)
                    OnBlockStart(ObjectContainer.ScenarioContext.CurrentScenarioBlock);
            }
        }

        private TimeSpan InvokeAction(Delegate action, object[] arguments, MethodInfo methodInfo)
        {
            try
            {
                System.Diagnostics.Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                action.DynamicInvoke(arguments);
                stopwatch.Stop();

                if (RuntimeConfiguration.Current.TraceTimings && stopwatch.Elapsed >= RuntimeConfiguration.Current.MinTracedDuration)
                {
                    testTracer.TraceDuration(stopwatch.Elapsed, methodInfo, arguments);
                }

                return stopwatch.Elapsed;
            }
            catch (ArgumentException ex)
            {
                throw errorProvider.GetCallError(methodInfo, ex);
            }
            catch (TargetInvocationException invEx)
            {
                var ex = invEx.InnerException;
                PreserveStackTrace(ex);
                throw ex;
            }
        }

        private object[] GetExecuteArguments(BindingMatch match)
        {
            List<object> arguments = new List<object>();

            var regexArgs = match.Match.Groups.Cast<Group>().Skip(1).Select(g => g.Value).ToArray();

            if (regexArgs.Length > match.StepBinding.ParameterTypes.Length)
                throw errorProvider.GetParameterCountError(match, regexArgs.Length + (match.ExtraArguments == null ? 0 : match.ExtraArguments.Length));

            for (int argIndex = 0; argIndex < regexArgs.Length; argIndex++)
            {
                var convertedArg = stepArgumentTypeConverter.Convert(regexArgs[argIndex], match.StepBinding.ParameterTypes[argIndex],
                    FeatureContext.Current.FeatureInfo.CultureInfo);
                arguments.Add(convertedArg);
            }

            if (match.ExtraArguments != null)
                arguments.AddRange(match.ExtraArguments);

            if (arguments.Count != match.StepBinding.ParameterTypes.Length)
                throw errorProvider.GetParameterCountError(match, arguments.Count);

            return arguments.ToArray();
        }
        #endregion

        #region Given-When-Then
        public void Given(string text, string multilineTextArg, Table tableArg)
        {
            ExecuteStep(new StepArgs(BindingType.Given, StepDefinitionKeyword.Given, text, multilineTextArg, tableArg));
        }

        public void When(string text, string multilineTextArg, Table tableArg)
        {
            ExecuteStep(new StepArgs(BindingType.When, StepDefinitionKeyword.When, text, multilineTextArg, tableArg));
        }

        public void Then(string text, string multilineTextArg, Table tableArg)
        {
            ExecuteStep(new StepArgs(BindingType.Then, StepDefinitionKeyword.Then, text, multilineTextArg, tableArg));
        }

        public void And(string text, string multilineTextArg, Table tableArg)
        {
            BindingType bindingType = ObjectContainer.ScenarioContext.CurrentScenarioBlock.ToBindingType();
            ExecuteStep(new StepArgs(bindingType, StepDefinitionKeyword.And, text, multilineTextArg, tableArg));
        }

        public void But(string text, string multilineTextArg, Table tableArg)
        {
            BindingType bindingType = ObjectContainer.ScenarioContext.CurrentScenarioBlock.ToBindingType();
            ExecuteStep(new StepArgs(bindingType, StepDefinitionKeyword.But, text, multilineTextArg, tableArg));
        }
        #endregion

        public void Pending()
        {
            throw errorProvider.GetPendingStepDefinitionError();
        }
    }
}
