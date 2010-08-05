using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Compatibility;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.ErrorHandling;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow
{
    public class TestRunner : ITestRunner
    {
        private readonly ErrorProvider errorProvider;
        private readonly ITestTracer testTracer;
        private readonly IUnitTestRuntimeProvider unitTestRuntimeProvider;
        private readonly IStepFormatter stepFormatter;
        private readonly StepDefinitionSkeletonProvider stepDefinitionSkeletonProvider;
        private readonly BindingRegistry bindingRegistry;
        private readonly IStepArgumentTypeConverter stepArgumentTypeConverter; 

        public TestRunner()
        {
            errorProvider = ObjectContainer.ErrorProvider;
            testTracer = ObjectContainer.TestTracer;
            unitTestRuntimeProvider = ObjectContainer.UnitTestRuntimeProvider;
            stepFormatter = ObjectContainer.StepFormatter;
            stepDefinitionSkeletonProvider = ObjectContainer.StepDefinitionSkeletonProvider;
 
            bindingRegistry = ObjectContainer.BindingRegistry;
            stepArgumentTypeConverter = ObjectContainer.StepArgumentTypeConverter;
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
                        OnTestRunnerEnd();
                    };
            //TODO: Siverlight
#endif
        }

        protected virtual void OnTestRunnerStart()
        {
            FireEvents(BindingEvent.TestRunStart, null);
        }

        protected virtual void OnTestRunnerEnd()
        {
            FireEvents(BindingEvent.TestRunEnd, null);
        }

        private FeatureInfo currentFeatureInfo;

        public void OnFeatureStart(FeatureInfo featureInfo)
        {
            currentFeatureInfo = featureInfo;

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

            ObjectContainer.ScenarioContext.TestError.PreserveStackTrace();
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

        private BindingMatch Match(StepBinding stepBinding, StepArgs stepArgs, bool useParamMatching, bool isScoped)
        {
            Match match = stepBinding.Regex.Match(stepArgs.Text);

            // Check if regexp is a match
            if (!match.Success)
                return null;

            var extraArgs = CalculateExtraArgs(stepArgs);

            if (useParamMatching)
            {
                var regexArgs = match.Groups.Cast<Group>().Skip(1).Select(g => g.Value).ToArray();

                // check if the regex + extra arguments match to the binding method parameters
                if (regexArgs.Length + extraArgs.Length != stepBinding.ParameterTypes.Length)
                    return null;

                // Check if regex arguments can be converted to the method parameters
                CultureInfo cultureInfo = FeatureContext.Current.FeatureInfo.Language;
                for (int regexArgIndex = 0; regexArgIndex < regexArgs.Length; regexArgIndex++)
                {
                    Type parameterType = stepBinding.ParameterTypes[regexArgIndex];

                    if (!stepArgumentTypeConverter.CanConvert(regexArgs[regexArgIndex], parameterType, cultureInfo))
                        return null;
                }

                // Check if there are corresponting parameters defined for the extra arguments 
                for (int extraArgIndex = 0; extraArgIndex < extraArgs.Length; extraArgIndex++)
                {
                    Type parameterType = stepBinding.ParameterTypes[extraArgIndex + regexArgs.Length];
                    Type argType = extraArgs[extraArgIndex].GetType();
                    if (argType != parameterType)
                        return null;
                }
            }

            return new BindingMatch(stepBinding, match, extraArgs, stepArgs, isScoped);
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

            // look for Feature scoped steps for the current feature
            var scopedStepBindings = bindingRegistry
                .Where(b => b.Type == stepArgs.Type && b.IsScoped && b.FeatureName == currentFeatureInfo.Title);
            foreach (StepBinding binding in scopedStepBindings)
            {
                BindingMatch match = Match(binding, stepArgs, true, true);
                if (match == null)
                    continue;

                matches.Add(match);
                if (!RuntimeConfiguration.Current.DetectAmbiguousMatches)
                    break;
            }

            if (matches.Count > 1)
            {
                throw errorProvider.GetAmbiguousMatchError(matches, stepArgs);
            }

            if (matches.Count == 1)
                return matches[0];

            // if there are no scoped matches then look for the step amongst the unscoped ones.
            var unscopedStepBindings = bindingRegistry.Where(b => b.Type == stepArgs.Type && b.IsScoped == false);
            foreach (StepBinding binding in unscopedStepBindings)
            {
                BindingMatch match = Match(binding, stepArgs, true, false);
                if (match == null)
                    continue;

                matches.Add(match);
                if (!RuntimeConfiguration.Current.DetectAmbiguousMatches)
                    break;
            }

            if (matches.Count == 0)
            {
                // there were either no regex match of it was filtered out by the param matching
                // to provide better error message for the param matching error, we re-run 
                // the matching without param check
                List<BindingMatch> matchesWithoutParamCheck = GetMatchesWithoutParamCheck(stepArgs, false);
                if (matchesWithoutParamCheck.Count == 1)
                {
                    // no ambiguouity, but param error -> execute will find it out
                    return matchesWithoutParamCheck[0];
                }
                if (matchesWithoutParamCheck.Count > 1)
                {
                    // ambiguouity, because of param error
                    throw errorProvider.GetAmbiguousBecauseParamCheckMatchError(matches, stepArgs);
                }

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

        private List<BindingMatch> GetMatchesWithoutParamCheck(StepArgs stepArgs, bool isScoped)
        {
            List<BindingMatch> matches = new List<BindingMatch>();

            var stepBindings = bindingRegistry.Where(b => b.Type == stepArgs.Type && b.IsScoped == isScoped);
            foreach (StepBinding binding in stepBindings)
            {
                BindingMatch match = Match(binding, stepArgs, false, isScoped);
                if (match != null)
                    matches.Add(match);
            }

            return matches;
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
            if (ObjectContainer.ScenarioContext.CurrentScenarioBlock != block)
            {
                if (ObjectContainer.ScenarioContext.TestStatus == TestStatus.OK)
                    OnBlockEnd(ObjectContainer.ScenarioContext.CurrentScenarioBlock);

                ObjectContainer.ScenarioContext.CurrentScenarioBlock = block;

                if (ObjectContainer.ScenarioContext.TestStatus == TestStatus.OK)
                    OnBlockStart(ObjectContainer.ScenarioContext.CurrentScenarioBlock);
            }
        }

        private object[] GetExecuteArguments(BindingMatch match)
        {
            List<object> arguments = new List<object>();

            var regexArgs = match.Match.Groups.Cast<Group>().Skip(1).Select(g => g.Value).ToArray();

            if (regexArgs.Length + match.ExtraArguments.Length != match.StepBinding.ParameterTypes.Length)
                throw errorProvider.GetParameterCountError(match, regexArgs.Length + match.ExtraArguments.Length);

            CultureInfo cultureInfo = FeatureContext.Current.FeatureInfo.Language;
            for (int regexArgIndex = 0; regexArgIndex < regexArgs.Length; regexArgIndex++)
            {
                Type parameterType = match.StepBinding.ParameterTypes[regexArgIndex];

                var convertedArg = stepArgumentTypeConverter.Convert(
                    regexArgs[regexArgIndex], parameterType, cultureInfo);
                arguments.Add(convertedArg);
            }

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
