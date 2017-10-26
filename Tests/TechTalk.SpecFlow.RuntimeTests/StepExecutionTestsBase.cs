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
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.Utils;
using MockRepository = Rhino.Mocks.MockRepository;
using ScenarioExecutionStatus = TechTalk.SpecFlow.ScenarioExecutionStatus;

namespace TechTalk.SpecFlow.RuntimeTests
{
    public class StepExecutionTestsBase
    {
        protected MockRepository MockRepository;
        protected CultureInfo FeatureLanguage;
        protected IStepArgumentTypeConverter StepArgumentTypeConverterStub;
        protected ObjectContainer TestThreadContainer;

        protected IContextManager ContextManagerStub;
        protected IContainerBuilder ContainerBuilderStub;

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
            var runtimeConfiguration = ConfigurationLoader.GetDefault();
            runtimeConfiguration.BindingCulture = GetBindingCulture();

            TestThreadContainer = new ObjectContainer();
            TestThreadContainer.RegisterInstanceAs(runtimeConfiguration);
            TestThreadContainer.RegisterInstanceAs(new Mock<ITestRunner>().Object);
            TestThreadContainer.RegisterTypeAs<TestObjectResolver, ITestObjectResolver>();
            var containerBuilderMock = new Mock<IContainerBuilder>();
            containerBuilderMock.Setup(m => m.CreateScenarioContainer(It.IsAny<IObjectContainer>(), It.IsAny<ScenarioInfo>()))
                .Returns((IObjectContainer fc, ScenarioInfo si) =>
                {
                    var scenarioContainer = new ObjectContainer(fc);
                    scenarioContainer.RegisterInstanceAs(si);
                    return scenarioContainer;
                });
            containerBuilderMock.Setup(m => m.CreateFeatureContainer(It.IsAny<IObjectContainer>(), It.IsAny<FeatureInfo>()))
                .Returns((IObjectContainer ttc, FeatureInfo fi) =>
                {
                    var featureContainer = new ObjectContainer(ttc);
                    featureContainer.RegisterInstanceAs(fi);
                    return featureContainer;
                });
            ContainerBuilderStub = containerBuilderMock.Object;
            ContextManagerStub = new ContextManager(MockRepository.Stub<ITestTracer>(), TestThreadContainer, ContainerBuilderStub);
            ContextManagerStub.InitializeFeatureContext(new FeatureInfo(FeatureLanguage, "test feature", null));
            ContextManagerStub.InitializeScenarioContext(new ScenarioInfo("test scenario"));

            StepArgumentTypeConverterStub = MockRepository.Stub<IStepArgumentTypeConverter>();
        }

        protected TestRunner GetTestRunnerFor(params Type[] bindingTypes)
        {
            return GetTestRunnerFor(null, bindingTypes);
        }

        protected TestRunner GetTestRunnerFor(Action<IObjectContainer> registerMocks, params Type[] bindingTypes)
        {
            return TestObjectFactories.CreateTestRunner(
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

        protected ScenarioExecutionStatus GetLastTestStatus()
        {
            return ContextManagerStub.ScenarioContext.ScenarioExecutionStatus;
        }
    }
}