using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using BoDi;
using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Discovery;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.Utils;
using MockRepository = Rhino.Mocks.MockRepository;
using TestStatus = TechTalk.SpecFlow.Infrastructure.TestStatus;

namespace TechTalk.SpecFlow.RuntimeTests
{
    public class StepExecutionTestsBase
    {
        protected MockRepository MockRepository;
        protected CultureInfo FeatureLanguage;
        protected IStepArgumentTypeConverter StepArgumentTypeConverterStub;

        protected IContextManager ContextManagerStub;

        #region dummy test tracer
        public class DummyTestTracer : ITestTracer
        {
            public void TraceStep(StepInstance stepInstance, bool showAdditionalArguments)
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

            public void TraceNoMatchingStepDefinition(StepInstance stepInstance, ProgrammingLanguage targetLanguage, CultureInfo bindingCulture, List<BindingMatch> matchesWithoutScopeCheck)
            {
                Console.WriteLine("TraceNoMatchingStepDefinition");
            }

            public void TraceDuration(TimeSpan elapsed, IBindingMethod method, object[] arguments)
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
        public virtual void SetUp()
        {
            TestRunnerManager.Reset();

            MockRepository = new MockRepository();

            // FeatureContext and ScenarioContext is needed, because the [Binding]-instances live there
            FeatureLanguage = GetFeatureLanguage();
            CultureInfo bindingCulture = GetBindingCulture();

            var container = new ObjectContainer();
            container.RegisterInstanceAs(new Mock<ITestRunner>().Object);
            ContextManagerStub = new ContextManager(MockRepository.Stub<ITestTracer>(), container);
            ContextManagerStub.InitializeFeatureContext(new FeatureInfo(FeatureLanguage, "test feature", null), bindingCulture);
            ContextManagerStub.InitializeScenarioContext(new ScenarioInfo("test scenario"));

            StepArgumentTypeConverterStub = MockRepository.Stub<IStepArgumentTypeConverter>();
        }

        protected TestRunner GetTestRunnerFor(params Type[] bindingTypes)
        {
            return GetTestRunnerFor(null, bindingTypes);
        }

        protected TestRunner GetTestRunnerFor(Action<IObjectContainer> registerMocks, params Type[] bindingTypes)
        {
            return TestTestRunnerFactory.CreateTestRunner(
                container =>
                    {
                        container.RegisterTypeAs<DummyTestTracer, ITestTracer>();
                        container.RegisterInstanceAs(ContextManagerStub);

                        var builder = (RuntimeBindingRegistryBuilder)container.Resolve<IRuntimeBindingRegistryBuilder>();
                        foreach (var bindingType in bindingTypes)
                            builder.BuildBindingsFromType(bindingType);

                        if (registerMocks != null)
                            registerMocks(container);
                    });
        }

        protected TestRunner GetTestRunnerFor<TBinding>(out TBinding bindingInstance)
        {
            return GetTestRunnerFor(null, out bindingInstance);
        }

        protected TestRunner GetTestRunnerFor<TBinding>(Action<IObjectContainer> registerMocks, out TBinding bindingInstance)
        {
            TestRunner testRunner = GetTestRunnerFor(registerMocks, typeof(TBinding));

            bindingInstance = MockRepository.StrictMock<TBinding>();
            testRunner.ScenarioContext.SetBindingInstance(typeof(TBinding), bindingInstance);
            return testRunner;
        }

        protected TestRunner GetTestRunnerWithConverterStub<TBinding>(out TBinding bindingInstance)
        {
            return GetTestRunnerFor(c => c.RegisterInstanceAs(StepArgumentTypeConverterStub), out bindingInstance);
        }

        protected TestStatus GetLastTestStatus()
        {
            return ContextManagerStub.ScenarioContext.TestStatus;
        }
    }
}