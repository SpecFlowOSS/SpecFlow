using System;
using System.Globalization;
using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.RuntimeTests
{
    public class StepExecutionTestsBase
    {
        protected MockRepository MockRepository;
        private BindingRegistry bindingRegistry;
        protected CultureInfo Language;

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

            public void TraceNoMatchingStepDefinition(StepArgs stepArgs)
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

        protected virtual CultureInfo GetLanguage()
        {
            return new CultureInfo("en-US");
        }

        [SetUp]
        public void SetUp()
        {
            ObjectContainer.Reset();

            MockRepository = new MockRepository();

            // FeatureContext and ScenarioContext is needed, because the [Binding]-instances live there
            Language = GetLanguage();
            ObjectContainer.FeatureContext = new FeatureContext(new FeatureInfo(Language, "test feature", null));
            ObjectContainer.ScenarioContext = new ScenarioContext(new ScenarioInfo("test scenario"));

            ObjectContainer.StepFormatter = MockRepository.Stub<IStepFormatter>();
            //ObjectContainer.TestTracer = MockRepository.Stub<ITestTracer>();
            ObjectContainer.TestTracer = new DummyTestTracer();
        }

        protected TestRunner GetTestRunnerFor(params Type[] bindingTypes)
        {
            return GetTestRunnerFor(null, bindingTypes);
        }

        protected TestRunner GetTestRunnerFor(BindingAttribute bindingAttribute, Type[] bindingTypes)
        {
            bindingRegistry = new BindingRegistry();
            foreach (var bindingType in bindingTypes)
            {
                bindingRegistry.BuildBindingsFromType(bindingType, bindingAttribute);
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