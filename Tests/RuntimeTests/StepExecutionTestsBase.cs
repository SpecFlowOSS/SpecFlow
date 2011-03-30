using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.RuntimeTests
{
    public class StepExecutionTestsBase
    {
        protected MockRepository MockRepository;
        private BindingRegistry bindingRegistry;
        protected CultureInfo FeatureLanguage;

        #region dummy test tracer
        public class DummyTestTracer : ITestTracer
        {
            public void TraceStep(StepArgs stepArgs, bool showAdditionalArguments)
            {
            }

            public void TraceWarning(string text)
            {
                Console.WriteLine("TraceWarning: {0}", text);
            }

            public void TraceStepDone(BindingMatch match, object[] arguments, TimeSpan duration)
            {
            }

            public void TraceStepSkipped()
            {
                Console.WriteLine("TraceStepSkipped");
            }

            public void TraceStepPending(BindingMatch match, object[] arguments)
            {
                Console.WriteLine("TraceStepPending");
            }

            public void TraceBindingError(BindingException ex)
            {
                Console.WriteLine("TraceBindingError: {0}", ex);
            }

            public void TraceError(Exception ex)
            {
                Console.WriteLine("TraceError: {0}", ex);
            }

            public void TraceNoMatchingStepDefinition(StepArgs stepArgs, ProgrammingLanguage targetLanguage, List<BindingMatch> matchesWithoutScopeCheck)
            {
                Console.WriteLine("TraceNoMatchingStepDefinition");
            }

            public void TraceDuration(TimeSpan elapsed, MethodInfo methodInfo, object[] arguments)
            {
                //nop
            }

            public void TraceDuration(TimeSpan elapsed, string text)
            {
                //nop
            }
        }
        #endregion

        protected virtual CultureInfo GetFeatureLanguage()
        {
            return new CultureInfo("en-US");
        }     
        
        protected virtual CultureInfo GetBindingCulture()
        {
            return new CultureInfo("en-US");
        }        

        [SetUp]
        public void SetUp()
        {
            ObjectContainer.Reset();

            MockRepository = new MockRepository();

            // FeatureContext and ScenarioContext is needed, because the [Binding]-instances live there
            FeatureLanguage = GetFeatureLanguage();
            CultureInfo bindingCulture = GetBindingCulture();
            ObjectContainer.FeatureContext = new FeatureContext(new FeatureInfo(FeatureLanguage, "test feature", null), bindingCulture);
            ObjectContainer.ScenarioContext = new ScenarioContext(new ScenarioInfo("test scenario"));

            ObjectContainer.StepFormatter = MockRepository.Stub<IStepFormatter>();
            //ObjectContainer.TestTracer = MockRepository.Stub<ITestTracer>();
            ObjectContainer.TestTracer = new DummyTestTracer();
        }

        protected TestRunner GetTestRunnerFor(params Type[] bindingTypes)
        {
            bindingRegistry = new BindingRegistry();
            foreach (var bindingType in bindingTypes)
            {
                bindingRegistry.BuildBindingsFromType(bindingType);
            }
            ObjectContainer.BindingRegistry = bindingRegistry;

            return new TestRunner();
        }

        protected TestRunner GetTestRunnerFor<TBinding>(out TBinding bindingInstance)
        {
            TestRunner testRunner = GetTestRunnerFor(typeof(TBinding));

            bindingInstance = MockRepository.StrictMock<TBinding>();
            ObjectContainer.ScenarioContext.SetBindingInstance(typeof(TBinding), bindingInstance);
            return testRunner;
        }
    }
}